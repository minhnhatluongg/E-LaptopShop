using AutoMapper;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductImage.Commands.DeleteProductImgages
{
    public class DeleteProductImageCommandHandler : IRequestHandler<DeleteProductImageCommand, int>
    {
        private readonly IProductImageRepository _productImageRepository;
        private readonly IMapper _mapper;

        public DeleteProductImageCommandHandler(IProductImageRepository productImageRepository, IMapper mapper)
        {
            _productImageRepository = productImageRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(DeleteProductImageCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var image = await _productImageRepository.GetImagesByProductIdAsync(request.Id, cancellationToken);
                if (image == null)
                {
                    throw new KeyNotFoundException($"Image with ID {request.Id} not found.");
                }
                var deleteId = await _productImageRepository.DeleteImageAsync(request.Id, cancellationToken);
                return deleteId;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while deleting the image: {ex.Message}", ex);
            }
        }
    }
}
