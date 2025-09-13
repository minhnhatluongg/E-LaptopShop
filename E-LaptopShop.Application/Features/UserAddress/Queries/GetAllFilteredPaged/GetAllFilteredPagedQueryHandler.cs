using AutoMapper;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.Common.Pagination_Sort_Filter;
using E_LaptopShop.Application.Common.Queries;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.FilterParams;
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

namespace E_LaptopShop.Application.Features.UserAddress.Queries.GetAllFilteredPaged
{

    public class GetAllFilteredPagedQueryHandler : BasePagedQueryHandler<UserAddressEntity, UserAddressDto, GetAllFilteredPagedQuery>, IRequestHandler<GetAllFilteredPagedQuery, PagedResult<UserAddressDto>>
    {
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly ILogger<GetAllFilteredPagedQueryHandler> _logger;

        public GetAllFilteredPagedQueryHandler(
            IMapper mapper, 
            ILogger<GetAllFilteredPagedQueryHandler> logger,
            IUserAddressRepository userAddressRepository
            ) 
            : base(mapper, logger)
        {
            _userAddressRepository = userAddressRepository;
            _logger = logger;
        }

        public Task<PagedResult<UserAddressDto>> Handle(GetAllFilteredPagedQuery request, CancellationToken cancellationToken)
        {
            return ProcessQueryOptimized(request, cancellationToken);
        }

        protected override IQueryable<UserAddressEntity> ApplyDatabaseSearch(IQueryable<UserAddressEntity> queryable, SearchOptions search)
        {
            if (!search.HasSearch) return queryable;
            var searchTerm = search.SearchTerm!.ToLower();
            return queryable.Where(p =>
                EF.Functions.Like(p.AddressLine.ToLower(), searchTerm) ||
                EF.Functions.Like(p.FullName.ToLower(), searchTerm) ||
                EF.Functions.Like(p.Phone.ToLower(), searchTerm) ||  
                EF.Functions.Like(p.City.ToLower(), searchTerm) ||
                EF.Functions.Like(p.District.ToLower(), searchTerm) ||  
                EF.Functions.Like(p.Ward.ToLower(), searchTerm) ||  
                EF.Functions.Like(p.PostalCode.ToLower(), searchTerm) ||
                EF.Functions.Like(p.CountryCode.ToLower(), searchTerm)
            );
        }

        protected override IQueryable<UserAddressEntity> ApplyDatabaseSorting(IQueryable<UserAddressEntity> queryable, SortingOptions sort)
        {
            return sort.SortBy?.ToLowerInvariant() switch
            {
                "fullname" => sort.IsAscending
                    ? queryable.OrderBy(p => p.FullName) : queryable.OrderByDescending(p => p.FullName),
                "id" => sort.IsAscending
                    ? queryable.OrderBy(p => p.Id) : queryable.OrderByDescending(p => p.Id),
                _ => ApplyDefaultDatabaseSorting(queryable)
            };
        }

        protected override Task<IQueryable<UserAddressEntity>> GetFilteredQueryable(GetAllFilteredPagedQuery request, CancellationToken cancellationToken)
        {
            var q = _userAddressRepository.GetFilteredQueryable(
                new UserAddressFilterParams
                {
                    UserId = request.UserId,
                    IsDefault = request.IsDefault,
                    CountryCode = request.CountryCode,
                    IsDeleted = request.IsDeleted,
                    Ward = request.Ward,
                    City = request.City,
                    CreatedFrom = request.CreatedFrom,
                    CreatedTo = request.CreatedTo,
                    UpdatedFrom = request.UpdatedFrom,
                    UpdatedTo = request.UpdatedTo
                },
                    includeUser: request.IncludeUser
                );

            return Task.FromResult(q
        );
        }
    }
}
