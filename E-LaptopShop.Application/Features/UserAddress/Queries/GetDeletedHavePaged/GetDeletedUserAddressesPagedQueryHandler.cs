using AutoMapper;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAddressEntity = E_LaptopShop.Domain.Entities.UserAddress;

namespace E_LaptopShop.Application.Features.UserAddress.Queries.GetDeletedHavePaged
{
    public class GetDeletedUserAddressesPagedQueryHandler : BasePagedQueryHandler<UserAddressEntity, UserAddressDto, GetDeletedUserAddressesPagedQuery>
    {
        private readonly IUserAddressRepository _userAddressRepository;
        public GetDeletedUserAddressesPagedQueryHandler(
            IMapper mapper, 
            IUserAddressRepository userAddressRepository, 
            ILogger logger) : base(mapper, logger)
        {
            _userAddressRepository = userAddressRepository;
        }

        public Task<PagedResult<UserAddressDto>> Handle(GetDeletedUserAddressesPagedQuery request, CancellationToken cancellationToken)
        {
            return ProcessQueryOptimized(request, cancellationToken);
        }

        protected override IQueryable<UserAddressEntity> ApplyDatabaseSearch(IQueryable<UserAddressEntity> queryable, SearchOptions search)
        {
            if (!search.HasSearch) return queryable;

            var term = $"%{search.SearchTerm!.Trim().ToLowerInvariant()}%";

            return queryable.Where(p =>
                EF.Functions.Like((p.FullName ?? "").ToLower(), term) ||
                EF.Functions.Like((p.Phone ?? "").ToLower(), term) ||
                EF.Functions.Like((p.AddressLine ?? "").ToLower(), term) ||
                EF.Functions.Like((p.City ?? "").ToLower(), term) ||
                EF.Functions.Like((p.District ?? "").ToLower(), term) ||
                EF.Functions.Like((p.Ward ?? "").ToLower(), term) ||
                EF.Functions.Like((p.PostalCode ?? "").ToLower(), term) ||
                EF.Functions.Like((p.CountryCode ?? "").ToLower(), term)
            );
        }

        protected override IQueryable<UserAddressEntity> ApplyDatabaseSorting(IQueryable<UserAddressEntity> queryable, SortingOptions sort)
        {
            if (!sort.HasSorting)
            {
                // Ưu tiên Recycle Bin xem cái xóa gần nhất trước
                if (typeof(UserAddressEntity).GetProperty("DeletedAt") != null)
                    return queryable.OrderByDescending(p => p.DeletedAt);

                // Fallback: nếu không có DeletedAt thì dùng default sorting của Base (Id/UpdatedAt)
                return ApplyDefaultDatabaseSorting(queryable);
            }

            return (sort.SortBy?.ToLowerInvariant(), sort.IsAscending) switch
            {
                ("id", true) => queryable.OrderBy(p => p.Id),
                ("id", false) => queryable.OrderByDescending(p => p.Id),

                ("fullname", true) => queryable.OrderBy(p => p.FullName),
                ("fullname", false) => queryable.OrderByDescending(p => p.FullName),

                ("deletedat", true) => queryable.OrderBy(p => p.DeletedAt),
                ("deletedat", false) => queryable.OrderByDescending(p => p.DeletedAt),

                ("updatedat", true) => queryable.OrderBy(p => p.UpdatedAt),
                ("updatedat", false) => queryable.OrderByDescending(p => p.UpdatedAt),

                ("createdat", true) => queryable.OrderBy(p => p.CreatedAt),
                ("createdat", false) => queryable.OrderByDescending(p => p.CreatedAt),

                ("city", true) => queryable.OrderBy(p => p.City).ThenByDescending(p => p.DeletedAt),
                ("city", false) => queryable.OrderByDescending(p => p.City).ThenByDescending(p => p.DeletedAt),

                ("isdefault", true) => queryable.OrderBy(p => p.IsDefault),
                ("isdefault", false) => queryable.OrderByDescending(p => p.IsDefault),

                _ => ApplyDefaultDatabaseSorting(queryable)
            };
        }
        protected override Task<IQueryable<UserAddressEntity>> GetFilteredQueryable(GetDeletedUserAddressesPagedQuery request, CancellationToken cancellationToken)
        {
            var q = _userAddressRepository.QueryIgnoreFilters()
                .Where(x => x.IsDeleted);
            if(request.UserId.HasValue)
            {
                q = q.Where(x => x.UserId == request.UserId.Value);
            }
            return Task.FromResult(q);
        }
    }
}
