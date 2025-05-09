using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Roles.Command.CreateRole
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand,RoleDto>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IMapper _mapper;
        public CreateRoleCommandHandler(IRoleRepository roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }
        public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = _mapper.Map<Role>(request);
            if(role.IsActive == default)
            {
                role.IsActive = true;
            }
            await _roleRepository.AddAsync(role,cancellationToken);
            return _mapper.Map<RoleDto>(role);
        }
    }
}
