using System.Linq.Expressions;
using AutoMapper;
using E_LaptopShop.Application.Common.Exceptions;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Base;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Application.Services.Implementations
{
    public class CouponService
        : BaseService<Coupon, CouponDto, CreateCouponRequestDto, UpdateCouponRequestDto, CouponQueryParams>,
          ICouponService
    {
        private readonly ICouponRepository _couponRepository;

        public CouponService(
            ICouponRepository couponRepository,
            IMapper mapper,
            ILogger<CouponService> logger) : base(mapper, logger)
        {
            _couponRepository = couponRepository;
        }

        #region BaseService bindings

        protected override async Task<Coupon?> GetEntityByIdAsync(int id, CancellationToken ct)
            => await _couponRepository.GetByIdAsync(id, ct);

        protected override async Task<Coupon> CreateEntityAsync(Coupon entity, CancellationToken ct)
            => await _couponRepository.AddAsync(entity, ct);

        protected override async Task<Coupon> UpdateEntityAsync(Coupon entity, CancellationToken ct)
            => await _couponRepository.UpdateAsync(entity, ct);

        protected override async Task<bool> DeleteEntityAsync(int id, CancellationToken ct)
            => await _couponRepository.DeleteAsync(id, ct);

        protected override IQueryable<Coupon> GetBaseQueryable(CouponQueryParams queryParams)
            => _couponRepository.GetQueryable();

        #endregion

        #region Query composition

        protected override IQueryable<Coupon> ApplyBusinessFilters(IQueryable<Coupon> q, CouponQueryParams p)
        {
            if (!string.IsNullOrWhiteSpace(p.Code))
                q = q.Where(c => EF.Functions.Like(c.Code, $"%{p.Code}%"));

            if (!string.IsNullOrWhiteSpace(p.DiscountType))
                q = q.Where(c => c.DiscountType == p.DiscountType);

            if (p.IsActive.HasValue)
                q = q.Where(c => c.IsActive == p.IsActive.Value);

            if (p.StartsFrom.HasValue)
                q = q.Where(c => c.StartsAt >= p.StartsFrom.Value);

            if (p.EndsBefore.HasValue)
                q = q.Where(c => c.EndsAt == null || c.EndsAt <= p.EndsBefore.Value);

            if (p.OnlyAvailable == true)
            {
                var now = DateTime.UtcNow;
                q = q.Where(c =>
                    c.IsActive
                    && c.StartsAt <= now
                    && (c.EndsAt == null || c.EndsAt > now)
                    && (c.UsageLimit == null || c.UsedCount < c.UsageLimit));
            }

            return q;
        }

        protected override IQueryable<Coupon> ApplyDomainSearch(IQueryable<Coupon> q, CouponQueryParams p)
        {
            if (string.IsNullOrWhiteSpace(p.Search)) return q;
            var term = p.Search.Trim();
            return q.Where(c =>
                EF.Functions.Like(c.Code, $"%{term}%") ||
                (c.Description != null && EF.Functions.Like(c.Description, $"%{term}%")));
        }

        protected override bool HasSearchCriteria(CouponQueryParams p)
            => !string.IsNullOrWhiteSpace(p.Search);

        protected override IQueryable<Coupon> ApplyDomainSorting(IQueryable<Coupon> q, CouponQueryParams p)
            => ApplySortingByMap(q, p.SortBy, p.IsAscending);

        protected override IReadOnlyDictionary<string, Expression<Func<Coupon, object>>> SortMap =>
            new Dictionary<string, Expression<Func<Coupon, object>>>
            {
                ["id"] = c => c.Id,
                ["code"] = c => c.Code,
                ["discountvalue"] = c => c.DiscountValue,
                ["startsat"] = c => c.StartsAt,
                ["endsat"] = c => (object?)c.EndsAt ?? DateTime.MaxValue,
                ["usedcount"] = c => c.UsedCount,
                ["createdat"] = c => c.CreatedAt,
            };

        protected override IQueryable<Coupon> ApplyDefaultSorting(IQueryable<Coupon> q)
            => q.OrderByDescending(c => c.CreatedAt);

        #endregion

        #region Business rules

        protected override async Task ValidateCreateDto(CreateCouponRequestDto dto, CancellationToken ct)
        {
            Throw.IfNullOrEmpty(dto.Code, nameof(dto.Code));
            var exists = await _couponRepository.CodeExistsAsync(dto.Code, null, ct);
            if (exists) Throw.Conflict($"Mã coupon '{dto.Code}' đã tồn tại");

            if (dto.DiscountType == "percent" && (dto.DiscountValue <= 0 || dto.DiscountValue > 100))
                Throw.BadRequest("DiscountValue cho percent phải nằm trong khoảng (0, 100]");

            if (dto.EndsAt.HasValue && dto.EndsAt <= dto.StartsAt)
                Throw.BadRequest("EndsAt phải sau StartsAt");
        }

        protected override async Task ValidateUpdateDto(int id, UpdateCouponRequestDto dto, Coupon existing, CancellationToken ct)
        {
            if (dto.DiscountType == "percent" && dto.DiscountValue.HasValue
                && (dto.DiscountValue <= 0 || dto.DiscountValue > 100))
                Throw.BadRequest("DiscountValue cho percent phải nằm trong khoảng (0, 100]");

            var newStarts = dto.StartsAt ?? existing.StartsAt;
            var newEnds = dto.EndsAt ?? existing.EndsAt;
            if (newEnds.HasValue && newEnds <= newStarts)
                Throw.BadRequest("EndsAt phải sau StartsAt");

            await Task.CompletedTask;
        }

        protected override Task ApplyCreateBusinessRules(Coupon entity, CreateCouponRequestDto dto, CancellationToken ct)
        {
            entity.Code = entity.Code.Trim().ToUpper();
            entity.UsedCount = 0;
            entity.CreatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        #endregion

        #region Domain-specific operations

        public async Task<CouponDto?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
        {
            var coupon = await _couponRepository.GetByCodeAsync(code, cancellationToken);
            return coupon == null ? null : _mapper.Map<CouponDto>(coupon);
        }

        public async Task<ApplyCouponResultDto> ValidateAsync(
            string code, decimal orderAmount, int userId, CancellationToken cancellationToken = default)
        {
            var (coupon, discount) = await ValidateInternalAsync(code, orderAmount, userId, cancellationToken);
            return BuildResult(coupon, orderAmount, discount, "Coupon hợp lệ");
        }

        public async Task<ApplyCouponResultDto> RedeemAsync(
            string code, decimal orderAmount, int userId, int? orderId, CancellationToken cancellationToken = default)
        {
            var (coupon, discount) = await ValidateInternalAsync(code, orderAmount, userId, cancellationToken);

            // Record usage + increment counter atomically (simple non-transactional version)
            await _couponRepository.AddUsageAsync(new CouponUsage
            {
                CouponId = coupon.Id,
                UserId = userId,
                OrderId = orderId,
                AmountSaved = discount,
                UsedAt = DateTime.UtcNow,
            }, cancellationToken);

            await _couponRepository.IncrementUsedCountAsync(coupon.Id, cancellationToken);

            return BuildResult(coupon, orderAmount, discount, "Áp dụng mã thành công");
        }

        private async Task<(Coupon coupon, decimal discount)> ValidateInternalAsync(
            string code, decimal orderAmount, int userId, CancellationToken ct)
        {
            Throw.IfNullOrEmpty(code, "Coupon code");
            if (orderAmount <= 0) Throw.BadRequest("Giá trị đơn hàng không hợp lệ");

            var coupon = await _couponRepository.GetByCodeAsync(code, ct);
            if (coupon == null) Throw.NotFound("Coupon", code);

            var now = DateTime.UtcNow;
            if (!coupon!.IsActive) Throw.BusinessRule("CouponInactive", "Mã không còn hoạt động");
            if (coupon.StartsAt > now) Throw.BusinessRule("CouponNotStarted", "Mã chưa đến thời gian sử dụng");
            if (coupon.EndsAt.HasValue && coupon.EndsAt <= now) Throw.BusinessRule("CouponExpired", "Mã đã hết hạn");
            if (orderAmount < coupon.MinOrderAmount)
                Throw.BusinessRule("MinOrderAmount", $"Đơn hàng phải tối thiểu {coupon.MinOrderAmount:N0}đ");

            if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit)
                Throw.BusinessRule("CouponOutOfStock", "Mã đã hết lượt sử dụng");

            if (coupon.UsageLimitPerUser.HasValue)
            {
                var used = await _couponRepository.CountUserUsageAsync(coupon.Id, userId, ct);
                if (used >= coupon.UsageLimitPerUser)
                    Throw.BusinessRule("UserLimit", "Bạn đã hết lượt dùng mã này");
            }

            var discount = CalculateDiscount(coupon, orderAmount);
            return (coupon, discount);
        }

        private static decimal CalculateDiscount(Coupon coupon, decimal orderAmount)
        {
            decimal raw = coupon.DiscountType switch
            {
                "percent" => orderAmount * coupon.DiscountValue / 100m,
                "fixed" => coupon.DiscountValue,
                _ => 0m,
            };

            if (coupon.MaxDiscountAmount.HasValue && raw > coupon.MaxDiscountAmount.Value)
                raw = coupon.MaxDiscountAmount.Value;

            if (raw > orderAmount) raw = orderAmount;
            return Math.Round(raw, 2);
        }

        private static ApplyCouponResultDto BuildResult(Coupon c, decimal orderAmount, decimal discount, string msg)
            => new()
            {
                CouponId = c.Id,
                Code = c.Code,
                OrderAmount = orderAmount,
                DiscountAmount = discount,
                FinalAmount = orderAmount - discount,
                Message = msg,
            };

        #endregion
    }
}
