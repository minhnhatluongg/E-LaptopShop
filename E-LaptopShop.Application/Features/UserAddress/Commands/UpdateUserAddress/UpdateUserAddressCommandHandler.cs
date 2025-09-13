using AutoMapper;
using E_LaptopShop.Application.Common.Exceptions;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Commands.UpdateUserAddress
{
    public class UpdateUserAddressCommandHandler : IRequestHandler<UpdateUserAddressCommand, UserAddressDto>
    {
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly IMapper _mapper;
        public UpdateUserAddressCommandHandler(IUserAddressRepository userAddressRepository, IMapper mapper)
        {
            _userAddressRepository = userAddressRepository;
            _mapper = mapper;
        }
        public async Task<UserAddressDto> Handle(UpdateUserAddressCommand request, CancellationToken cancellationToken)
        {
            //1. Lấy entity 
            var entity = await _userAddressRepository.FindByIdAsync(request.Id, cancellationToken);
            Throw.IfNull(entity, nameof(Domain.Entities.UserAddress), request.Id);
            //2. Không cho update nếu đã soft delete (không tồn tại)
            if (entity!.IsDeleted)
                Throw.NotFound(nameof(Domain.Entities.UserAddress), request?.Id);
            _mapper.Map(request, entity);
            entity.UpdatedAt = DateTimeOffset.UtcNow;
            //Đảm bảo isDefault không bị update
            if (request.IsDefault && entity.IsDefault)
            {
                await _userAddressRepository.UnsetDefaultForUserAsync(request.UserId, skipId: entity.Id, cancellationToken);
                entity.IsDefault = true;
            }
            else if (!request.IsDefault && entity.IsDefault)
            {
                // Nếu address hiện tại là mặc định, mà user muốn bỏ mặc định
                entity.IsDefault = false;
            }
            await _userAddressRepository.SaveChangesAsync(cancellationToken);
            return _mapper.Map<UserAddressDto>(entity);
        }
    }
}
