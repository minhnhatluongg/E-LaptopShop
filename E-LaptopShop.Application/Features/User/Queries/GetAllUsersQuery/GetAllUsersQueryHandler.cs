using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.FilterParams;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.User.Queries.GetAllUsersQuery
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
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
                SortBy = request.SortBy,
                IsAscending = request.IsAscending,
                SearchTerm = request.SearchTerm
            };

            var users = await _userRepository.GetFilteredAsync(filterParams, cancellationToken);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
    }
}
