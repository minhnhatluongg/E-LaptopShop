using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace E_LaptopShop.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductDto>
{
    private readonly IProductService     _productService;
    private readonly IProductRepository  _productRepo;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(
        IProductService productService,
        IProductRepository productRepo,
        IServiceScopeFactory scopeFactory,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _productService = productService;
        _productRepo    = productRepo;
        _scopeFactory   = scopeFactory;
        _logger         = logger;
    }

    public async Task<ProductDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("UpdateProductCommand — ProductId={Id}", request.RequestDto.Id);

        // Snapshot giá cũ TRƯỚC khi update
        var old = await _productRepo.GetByIdAsync(request.RequestDto.Id, cancellationToken);

        var updated = await _productService.UpdateProductAsync(request.RequestDto, cancellationToken);

        // ── Price History Audit ──────────────────────────────────────────
        if (old != null && old.Price != request.RequestDto.Price)
        {
            _logger.LogInformation(
                "[PriceHistory] Product #{Id} \"{Name}\" {Old:N0} → {New:N0} by UserId={User}",
                old.Id, old.Name, old.Price, request.RequestDto.Price, request.ChangedByUserId);

            _ = Task.Run(async () =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<Microsoft.EntityFrameworkCore.DbContext>();
                    var log = new ActivityLog
                    {
                        UserId    = request.ChangedByUserId > 0 ? request.ChangedByUserId : null,
                        EventType = "PRICE_CHANGED",
                        Metadata  = JsonSerializer.Serialize(new
                        {
                            productId   = old.Id,
                            productName = old.Name,
                            oldPrice    = old.Price,
                            newPrice    = request.RequestDto.Price,
                            oldDiscount = old.Discount,
                            newDiscount = request.RequestDto.Discount,
                        }),
                        CreatedAt = DateTime.UtcNow,
                    };
                    await db.Set<ActivityLog>().AddAsync(log);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[PriceHistory] Write failed for product {Id}", old.Id);
                }
            });
        }

        return updated;
    }
}
