using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace E_LaptopShop.Application.Services.Implementations
{
    public class ProductDetailService : IProductDetailService
    {
        private readonly IProductRepository _productRepo;
        private readonly IProductSpecificationRepository _specRepo;
        private readonly IProductImageRepository _imageRepo;
        private readonly IProductCommentRepository _commentRepo;
        private readonly IProductReviewRepository _reviewRepo;
        private readonly IUserRepository _userRepo;
        private readonly IBrandRepositoy _brandRepo;

        public ProductDetailService(
            IProductRepository productRepo,
            IProductSpecificationRepository specRepo,
            IProductImageRepository imageRepo,
            IProductCommentRepository commentRepo,
            IProductReviewRepository reviewRepo,
            IUserRepository userRepo,
            IBrandRepositoy brandRepo)
        {
            _productRepo = productRepo;
            _specRepo = specRepo;
            _imageRepo = imageRepo;
            _commentRepo = commentRepo;
            _reviewRepo = reviewRepo;
            _userRepo = userRepo;
            _brandRepo = brandRepo;
        }

        public async Task<int> ResolveSlugToIdAsync(string slug, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(slug))
                throw new KeyNotFoundException("Slug không hợp lệ");

            var s = slug.Trim();
            var product = await _productRepo.GetQueryable()
                .Where(p => p.Slug == s)
                .Select(p => new { p.Id })
                .FirstOrDefaultAsync(ct);

            if (product == null)
                throw new KeyNotFoundException($"Không tìm thấy sản phẩm với slug '{slug}'");

            return product.Id;
        }

        public async Task<ProductDetailDto> GetProductDetailAsync(int productId, CancellationToken ct = default)
        {
            var product = await _productRepo.GetByIdAsync(productId, ct);
            if (product == null)
                throw new KeyNotFoundException($"Product {productId} not found");

            var specs = await _specRepo.GetByProductIdAsync(productId, ct);
            var images = await _imageRepo.GetImagesByProductIdAsync(productId, ct);
            Brand? brand = null;
            if (product.BrandId.HasValue)
                brand = await _brandRepo.GetByIdAsync(product.BrandId.Value, ct);

            var avg = await _reviewRepo.AverageRatingByProductIdAsync(productId, ct);
            var totalReviews = await _reviewRepo.CountByProductIdAsync(productId, ct);
            var totalComments = await _commentRepo.CountByProductIdAsync(productId, ct);
            var breakdown = await _reviewRepo.RatingBreakdownByProductIdAsync(productId, ct);

            var spec = specs?.FirstOrDefault();
            return new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Description = product.Description,
                Price = product.Price,
                Discount = product.Discount,
                InStock = product.InStock,
                IsActive = product.IsActive,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                BrandId = product.BrandId,
                BrandName = brand?.Name,
                Images = images?.Select(i => new ProductImageDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    SysFileId = i.SysFileId,
                    ImageUrl = i.ImageUrl ?? string.Empty,
                    FileType = i.FileType,
                    FileSize = i.FileSize,
                    IsMain = i.IsMain,
                    DisplayOrder = i.DisplayOrder,
                    AltText = i.AltText,
                    Title = i.Title,
                    CreatedAt = i.CreatedAt,
                    UploadedAt = i.UploadedAt,
                    IsActive = i.IsActive
                }).ToList() ?? new List<ProductImageDto>(),
                Specification = spec == null ? null : new ProductSpecificationDto
                {
                    Id = spec.Id,
                    ProductId = spec.ProductId,
                    CPU = spec.CPU,
                    RAM = spec.RAM,
                    Storage = spec.Storage,
                    GPU = spec.GPU,
                    Screen = spec.Screen,
                    OS = spec.OS,
                    Ports = spec.Ports,
                    Weight = spec.Weight,
                    Battery = spec.Battery
                },
                AverageRating = avg,
                TotalReviews = totalReviews,
                TotalComments = totalComments,
                RatingBreakdown = breakdown
            };
        }

        public async Task<CurrentUserContextDto> GetCurrentUserContextAsync(int? userId, int productId, CancellationToken ct = default)
        {
            if (!userId.HasValue || userId.Value <= 0)
                return new CurrentUserContextDto { IsAuthenticated = false };

            var user = await _userRepo.GetByIdAsync(userId.Value);
            if (user == null)
                return new CurrentUserContextDto { IsAuthenticated = false };

            var hasPurchased = await _reviewRepo.HasUserPurchasedProductAsync(userId.Value, productId, ct);
            var tierName = await _reviewRepo.GetUserLoyaltyTierNameAsync(userId.Value, ct);

            return new CurrentUserContextDto
            {
                IsAuthenticated = true,
                UserId = user.Id,
                FullName = user.FullName,
                AvatarUrl = user.AvatarUrl,
                Role = user.Role?.Name,
                LoyaltyTierName = tierName,
                HasPurchasedThisProduct = hasPurchased
            };
        }

        public async Task<List<ProductCommentDto>> GetCommentsAsync(int productId, CancellationToken ct = default)
        {
            var items = await _commentRepo.GetByProductIdAsync(productId, ct);
            var result = new List<ProductCommentDto>();
            foreach (var c in items)
            {
                var hasPurchased = c.UserId > 0 && await _reviewRepo.HasUserPurchasedProductAsync(c.UserId, productId, ct);
                var tier = c.UserId > 0 ? await _reviewRepo.GetUserLoyaltyTierNameAsync(c.UserId, ct) : null;
                result.Add(new ProductCommentDto
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    UserId = c.UserId,
                    UserFullName = c.User?.FullName,
                    UserAvatarUrl = c.User?.AvatarUrl,
                    Content = c.Content,
                    ParentCommentId = c.ParentCommentId,
                    CreatedAt = c.CreatedAt,
                    HasPurchasedThisProduct = hasPurchased,
                    LoyaltyTierName = tier
                });
            }
            return result;
        }

        public async Task<ProductCommentDto> AddCommentAsync(int userId, CreateProductCommentDto dto, CancellationToken ct = default)
        {
            if (userId <= 0)
                throw new UnauthorizedAccessException("Vui lòng đăng nhập để bình luận");

            var product = await _productRepo.GetByIdAsync(dto.ProductId, ct);
            if (product == null)
                throw new KeyNotFoundException($"Product {dto.ProductId} not found");

            var entity = new ProductComment
            {
                ProductId = dto.ProductId,
                UserId = userId,
                Content = dto.Content.Trim(),
                ParentCommentId = dto.ParentCommentId,
                CreatedAt = DateTime.UtcNow
            };
            entity = await _commentRepo.AddAsync(entity, ct);

            var user = await _userRepo.GetByIdAsync(userId);
            var hasPurchased = await _reviewRepo.HasUserPurchasedProductAsync(userId, dto.ProductId, ct);
            var tier = await _reviewRepo.GetUserLoyaltyTierNameAsync(userId, ct);

            return new ProductCommentDto
            {
                Id = entity.Id,
                ProductId = entity.ProductId,
                UserId = entity.UserId,
                UserFullName = user?.FullName,
                UserAvatarUrl = user?.AvatarUrl,
                Content = entity.Content,
                ParentCommentId = entity.ParentCommentId,
                CreatedAt = entity.CreatedAt,
                HasPurchasedThisProduct = hasPurchased,
                LoyaltyTierName = tier
            };
        }

        public async Task<int> DeleteCommentAsync(int currentUserId, string currentUserRole, int commentId, CancellationToken ct = default)
        {
            var c = await _commentRepo.GetByIdAsync(commentId, ct);
            if (c == null) throw new KeyNotFoundException($"Comment {commentId} not found");

            var isOwner = c.UserId == currentUserId;
            var isAdmin = string.Equals(currentUserRole, "Admin", StringComparison.OrdinalIgnoreCase)
                       || string.Equals(currentUserRole, "Manager", StringComparison.OrdinalIgnoreCase);

            if (!isOwner && !isAdmin)
                throw new UnauthorizedAccessException("Bạn không có quyền xóa bình luận này");

            return await _commentRepo.SoftDeleteAsync(commentId, ct);
        }

        public async Task<List<ProductReviewDto>> GetReviewsAsync(int productId, CancellationToken ct = default)
        {
            var items = await _reviewRepo.GetByProductIdAsync(productId, ct);
            var result = new List<ProductReviewDto>();
            foreach (var r in items)
            {
                var uid = r.UserId ?? 0;
                var hasPurchased = uid > 0 && await _reviewRepo.HasUserPurchasedProductAsync(uid, productId, ct);
                var tier = uid > 0 ? await _reviewRepo.GetUserLoyaltyTierNameAsync(uid, ct) : null;
                result.Add(new ProductReviewDto
                {
                    Id = r.Id,
                    ProductId = r.ProductId ?? productId,
                    UserId = r.UserId,
                    UserFullName = r.User?.FullName,
                    UserAvatarUrl = r.User?.AvatarUrl,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    IsVerifiedPurchase = hasPurchased,
                    LoyaltyTierName = tier
                });
            }
            return result;
        }

        public async Task<ProductReviewDto> AddReviewAsync(int userId, CreateProductReviewDto dto, CancellationToken ct = default)
        {
            if (userId <= 0)
                throw new UnauthorizedAccessException("Vui lòng đăng nhập để đánh giá");

            var product = await _productRepo.GetByIdAsync(dto.ProductId, ct);
            if (product == null)
                throw new KeyNotFoundException($"Product {dto.ProductId} not found");

            // BUSINESS RULE: chỉ user đã mua chính sản phẩm này mới được review.
            var purchased = await _reviewRepo.HasUserPurchasedProductAsync(userId, dto.ProductId, ct);
            if (!purchased)
                throw new UnauthorizedAccessException("Bạn cần mua và nhận sản phẩm này trước khi đánh giá");

            // Một user chỉ review một lần / sản phẩm
            var existed = await _reviewRepo.ExistsByUserAndProductAsync(userId, dto.ProductId, ct);
            if (existed)
                throw new InvalidOperationException("Bạn đã đánh giá sản phẩm này rồi");

            var entity = new ProductReview
            {
                ProductId = dto.ProductId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };
            entity = await _reviewRepo.AddAsync(entity, ct);

            var user = await _userRepo.GetByIdAsync(userId);
            var tier = await _reviewRepo.GetUserLoyaltyTierNameAsync(userId, ct);

            return new ProductReviewDto
            {
                Id = entity.Id,
                ProductId = entity.ProductId ?? dto.ProductId,
                UserId = entity.UserId,
                UserFullName = user?.FullName,
                UserAvatarUrl = user?.AvatarUrl,
                Rating = entity.Rating,
                Comment = entity.Comment,
                CreatedAt = entity.CreatedAt,
                IsVerifiedPurchase = true,
                LoyaltyTierName = tier
            };
        }

        public async Task<int> DeleteReviewAsync(int currentUserId, string currentUserRole, int reviewId, CancellationToken ct = default)
        {
            var r = await _reviewRepo.GetByIdAsync(reviewId, ct);
            if (r == null) throw new KeyNotFoundException($"Review {reviewId} not found");

            var isOwner = r.UserId == currentUserId;
            var isAdmin = string.Equals(currentUserRole, "Admin", StringComparison.OrdinalIgnoreCase)
                       || string.Equals(currentUserRole, "Manager", StringComparison.OrdinalIgnoreCase);

            if (!isOwner && !isAdmin)
                throw new UnauthorizedAccessException("Bạn không có quyền xóa đánh giá này");

            return await _reviewRepo.DeleteAsync(reviewId, ct);
        }
    }
}
