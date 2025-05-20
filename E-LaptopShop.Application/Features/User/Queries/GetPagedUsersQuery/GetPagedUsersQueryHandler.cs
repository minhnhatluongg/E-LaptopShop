using AutoMapper;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.FilterParams;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.User.Queries.GetPagedUsersQuery
{
    public class GetPagedUsersQueryHandler : IRequestHandler<GetPagedUsersQuery, PagedResult<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetPagedUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<PagedResult<UserDto>> Handle(GetPagedUsersQuery request, CancellationToken cancellationToken)
        {
            var filterParams = new UserFilterParams
            {
                Id = request.Id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                RoleId = request.RoleId,
                IsActive = request.IsActive,
                EmailConfirmed = request.EmailConfirmed,
                Gender = request.Gender,
                CreatedAtFrom = request.CreatedAtFrom,
                CreatedAtTo = request.CreatedAtTo,
                SortBy = request.SortBy,
                IsAscending = request.IsAscending,
                SearchTerm = request.SearchTerm
            };

            var (users, totalCount) = await _userRepository.GetPagedAsync(
                request.PageNumber,
                request.PageSize,
                filterParams,
                cancellationToken);

            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

            return new PagedResult<UserDto>(userDtos, request.PageNumber, request.PageSize, totalCount);
        }
    }
}
