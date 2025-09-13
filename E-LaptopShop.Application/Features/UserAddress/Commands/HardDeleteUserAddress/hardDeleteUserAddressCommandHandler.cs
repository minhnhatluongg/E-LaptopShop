using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Commands.HardDeleteUserAddress
{
    public class hardDeleteUserAddressCommandHandler : IRequestHandler<hardDeleteUserAddressCommand, int>
    {
        private readonly IUserAddressRepository _userAddressRepository;
        public hardDeleteUserAddressCommandHandler(IUserAddressRepository userAddressRepository)
        {
            _userAddressRepository = userAddressRepository;
        }
        public async Task<int> Handle(hardDeleteUserAddressCommand request, CancellationToken cancellationToken)
        {
            return await _userAddressRepository.HardDeleteAsync(request.Id, cancellationToken);
        }
    }
}
