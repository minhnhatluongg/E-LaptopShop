using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Inventory.Queries.GetInventoryByProduct
{
    public class GetInventoryByProductQueryHandler : IRequestHandler<GetInventoryByProductQuery, InventoryDto>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;

        public GetInventoryByProductQueryHandler(
            IInventoryRepository inventoryRepository,
            IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
        }

        public async Task<InventoryDto> Handle(GetInventoryByProductQuery request, CancellationToken cancellationToken)
        {
            var inventory = await _inventoryRepository.GetByProductIdAsync(request.ProductId);
            if (inventory == null)
            {
                throw new KeyNotFoundException($"Inventory not found for product ID {request.ProductId}");
            }

            return _mapper.Map<InventoryDto>(inventory);
        }
    }
}
