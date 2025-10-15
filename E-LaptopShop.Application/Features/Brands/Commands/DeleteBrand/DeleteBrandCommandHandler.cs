using E_LaptopShop.Application.Services.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Brands.Commands.DeleteBrand
{
    public class DeleteBrandCommandHandler : IRequestHandler<DeleteBrandCommand, bool>
    {
        private readonly IBrandService _brandService;
        private readonly ILogger<DeleteBrandCommandHandler> _logger;
        public DeleteBrandCommandHandler(
            IBrandService brandService,
            ILogger<DeleteBrandCommandHandler> logger)

        {
            _brandService = brandService;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
        {
            return await _brandService.DeleteAsync(request.Id, cancellationToken);
        }
    }
}
