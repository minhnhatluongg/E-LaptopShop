using E_LaptopShop.Application.Common.Exceptions;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Commands.SetDefaultUserAddress
{
    public class SetDefaultUserAddressCommandHandler : IRequestHandler<SetDefaultUserAddressCommand>
    {
        private readonly IUserAddressRepository _userAddressRepository;
        public SetDefaultUserAddressCommandHandler(IUserAddressRepository userAddressRepository)
        {
            _userAddressRepository = userAddressRepository;
        }

        public async Task Handle(SetDefaultUserAddressCommand request, CancellationToken cancellationToken)
        {
            var entity = await _userAddressRepository.FindAsync(request.Id, request.UserId, cancellationToken);

            Throw.IfNull(entity, $"UserAddress with Id={request.Id}");

            await _userAddressRepository.ClearDefaultAsync(request.UserId, cancellationToken);

            entity!.IsDefault = true;
            entity.UpdatedAt = DateTime.UtcNow;

            await _userAddressRepository.UpdateAsync(entity, cancellationToken);
            await _userAddressRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
