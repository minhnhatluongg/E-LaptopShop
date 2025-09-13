using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Commands.DeleteUserAddress
{
    public class DeleteUserAddressCommandHandler : IRequestHandler<DeleteUserAddressCommand, int>
    {
        private readonly IUserAddressRepository _userAddressRepository;
        public DeleteUserAddressCommandHandler(IUserAddressRepository userAddressRepository)
        {
            _userAddressRepository = userAddressRepository;
        }
        public async Task<int> Handle(DeleteUserAddressCommand request, CancellationToken cancellationToken)
        {
            return await _userAddressRepository.DeleteAsync(request.Id, cancellationToken);
        }
    }
}
