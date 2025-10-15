using AutoMapper;
using AutoMapper.QueryableExtensions;
using E_LaptopShop.Application.Common.Exceptions;
using E_LaptopShop.Application.Common.Helpers;
using E_LaptopShop.Application.Common.Pagination;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.DTOs.QueryParams;
using E_LaptopShop.Application.Services.Base;
using E_LaptopShop.Application.Services.Interfaces;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Services.Implementations
{
    public class ProductService : BaseService<Product, ProductDto, CreateProductRequestDto, UpdateProductRequestDto, ProductQueryParams>
                                , IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISlugGenerator _slugGenerator;
        private readonly IBrandRepositoy _brandRepositoy;
        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IMapper mapper,
            ISlugGenerator slugGenerator,
            IBrandRepositoy brandRepository,
            ILogger<ProductService> logger) : base(mapper, logger)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _slugGenerator = slugGenerator;
            _brandRepositoy = brandRepository;
        }

        public async Task<ProductDto> CreateProductAsync(CreateProductRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(requestDto.Slug))
            {
                var slug = _slugGenerator.GenerateSlugAsync(requestDto.Name, entitySet: "Product", excludeId: (int?)null, cancellationToken);
                requestDto.Slug = await slug;
            }
            return await CreateAsync(requestDto, cancellationToken);
        }

        public async Task<ProductDto> UpdateProductAsync(UpdateProductRequestDto requestDto, CancellationToken cancellationToken = default)
        {
            return await UpdateAsync(requestDto.Id, requestDto, cancellationToken);
        }

        public async Task<int> DeleteProductAsync(int id, CancellationToken cancellationToken = default)
        {
            var result = await DeleteAsync(id, cancellationToken);
            return result ? id : throw new InvalidOperationException($"Failed to delete product with ID {id}");
        }

        public async Task<bool> ValidateProductAsync(int id, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            return product != null && product.IsActive;
        }

        public async Task<bool> IsProductInStockAsync(int id, int requiredQuantity = 1, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            return product != null && product.IsActive && product.InStock >= requiredQuantity;
        }

        public async Task<decimal> CalculateDiscountedPriceAsync(int productId, decimal? discountPercentage = null, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found");
            }

            if (!discountPercentage.HasValue || discountPercentage <= 0)
            {
                return product.Price;
            }

            var discount = Math.Min(discountPercentage.Value, 100); // Max 100% discount
            return product.Price * (1 - discount / 100);
        }

        public async Task<IEnumerable<ProductDto>> GetRelatedProductsAsync(int productId, int count = 5, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
            {
                return Enumerable.Empty<ProductDto>();
            }

            var relatedProducts = await _productRepository.GetFilteredAsync(
                categoryId: product.CategoryId,
                inStock: true,
                cancellationToken: cancellationToken);

            var result = relatedProducts
                .Where(p => p.Id != productId && p.IsActive)
                .Take(count);

            return _mapper.Map<IEnumerable<ProductDto>>(result);
        }

        public async Task UpdateProductStockAsync(int productId, int quantity, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Updating stock for product {ProductId}: {Quantity}", productId, quantity);

            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {productId} not found");
            }

            // Validate stock update
            if (product.InStock + quantity < 0)
            {
                Throw.Equals(product.InStock + quantity >= 0, "Insufficient stock for the operation");
            }

            product.InStock += quantity;

            await _productRepository.UpdateAsync(product, cancellationToken);
            
            _logger.LogInformation("Stock updated for product {ProductId}. New stock: {NewStock}", productId, product.InStock);
        }

        #region Query Operations

        public async Task<ProductDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await GetByIdOrThrowAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            var products = await _productRepository.GetByCategoryAsync(categoryId, cancellationToken);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
        public IQueryable<Product> GetProductsQueryable(
            int? categoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? inStock = null)
        {
            return _productRepository.GetProductsQueryable(categoryId, minPrice, maxPrice, inStock);
        }
        public async Task<PagedResult<ProductDto>> GetAllProductsAsync(
            ProductQueryParams queryParams,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting all products with filters - CategoryId: {CategoryId}, Search: {Search}",
                queryParams.CategoryId, queryParams.Search);

            queryParams.ValidateAndNormalize();
            queryParams.ValidateBusinessRules();

            return await GetAllAsync(queryParams, cancellationToken);
        }

        #endregion

        #region BaseService Implementation

        protected override async Task<Product?> GetEntityByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _productRepository.GetByIdAsync(id, cancellationToken);
        }

        protected override async Task<Product> CreateEntityAsync(Product entity, CancellationToken cancellationToken)
        {
            return await _productRepository.AddAsync(entity, cancellationToken);
        }

        protected override async Task<Product> UpdateEntityAsync(Product entity, CancellationToken cancellationToken)
        {
            return await _productRepository.UpdateAsync(entity, cancellationToken);
        }

        protected override async Task<bool> DeleteEntityAsync(int id, CancellationToken cancellationToken)
        {
            return await _productRepository.DeleteAsync(id, cancellationToken);
        }

        protected override IQueryable<Product> GetBaseQueryable(ProductQueryParams queryParams)
        {
            return _productRepository.GetProductsQueryable(
                queryParams.CategoryId,
                queryParams.MinPrice,
                queryParams.MaxPrice,
                queryParams.InStock);
        }

        protected override IQueryable<Product> ApplyBusinessFilters(IQueryable<Product> q, ProductQueryParams p)
        {
            if (p.IsActive.HasValue)
                q = q.Where(x => x.IsActive == p.IsActive.Value);

            if (p.MinDiscount.HasValue)
                q = q.Where(x => x.Discount >= p.MinDiscount.Value);

            if (p.MaxDiscount.HasValue)
                q = q.Where(x => x.Discount <= p.MaxDiscount.Value);

            if (p.BrandId.HasValue)
            {
                q = q.Where(x => x.BrandId == p.BrandId.Value);
            }
            return q;
        }

        protected override IQueryable<Product> ApplyDomainSearch(IQueryable<Product> q, ProductQueryParams p)
        {
            var searchTerm = p.Search!;

            // Domain knowledge: Which fields are searchable for Products
            return q.Where(x =>
                EF.Functions.Like(x.Name, $"%{searchTerm}%") ||
                EF.Functions.Like(x.Description ?? "", $"%{searchTerm}%") ||
                EF.Functions.Like(x.Category.Name ?? "", $"%{searchTerm}%") ||
                x.ProductSpecifications.Any(spec =>
                    EF.Functions.Like(spec.CPU ?? "", $"%{searchTerm}%") ||
                    EF.Functions.Like(spec.RAM ?? "", $"%{searchTerm}%") ||
                    EF.Functions.Like(spec.Storage ?? "", $"%{searchTerm}%") ||
                    EF.Functions.Like(spec.GPU ?? "", $"%{searchTerm}%"))
            );
        }

        protected override IQueryable<Product> ApplyDomainSorting(IQueryable<Product> q, ProductQueryParams p)
        {
            return ApplySortingByMap(q, p.SortBy, p.IsAscending);
        }

        protected override IReadOnlyDictionary<string, Expression<Func<Product, object>>> SortMap =>
            new Dictionary<string, Expression<Func<Product, object>>>
            {
                ["name"] = x => x.Name,
                ["price"] = x => x.Price,
                ["discount"] = x => x.Discount,
                ["createdat"] = x => x.CreatedAt,
                ["stock"] = x => x.InStock,
                ["categoryname"] = x => x.Category.Name ?? "",
                ["id"] = x => x.Id,
                ["brandname"] = x => x.Brand.Name ?? "",
                ["brandid"] = x => x.BrandId ?? 0
            };

        protected override async Task ValidateCreateDto(CreateProductRequestDto dto, CancellationToken ct)
        {
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, ct);
            if (category == null)
                throw new KeyNotFoundException($"Category with ID {dto.CategoryId} not found");
            if (dto.BrandId.HasValue)
            {
                var brand = await _brandRepositoy.GetByIdAsync(dto.BrandId.Value, ct);
                if (brand == null)
                    throw new KeyNotFoundException($"Brand with ID {dto.BrandId} not found");
            }
        }

        protected override async Task ValidateUpdateDto(int id, UpdateProductRequestDto dto, Product existing, CancellationToken ct)
        {
            if (existing.CategoryId != dto.CategoryId)
            {
                var category = await _categoryRepository.GetByIdAsync(dto.CategoryId, ct);
                if (category == null)
                    throw new KeyNotFoundException($"Category with ID {dto.CategoryId} not found");
            }
            if (existing.BrandId != dto.BrandId && dto.BrandId.HasValue) 
            {
                var brand = await _brandRepositoy.GetByIdAsync(dto.BrandId.Value, ct);
                if (brand == null)
                    throw new KeyNotFoundException($"Brand with ID {dto.BrandId} not found");
            }
        }

        protected override async Task ValidateDeleteRules(Product entity, CancellationToken ct)
        {
            await ValidateProductDeletionRules(entity);
        }

        protected override async Task ApplyCreateBusinessRules(Product entity, CreateProductRequestDto dto, CancellationToken ct)
        {
            entity.CreatedAt = DateTime.UtcNow;
            await ValidateProductBusinessRules(entity);
        }

        protected override async Task ApplyUpdateBusinessRules(Product entity, UpdateProductRequestDto dto, CancellationToken ct)
        {
            await ValidateProductBusinessRules(entity);
        }

        protected override async Task ValidateQueryParams(ProductQueryParams p, CancellationToken ct)
        {
            // Call base method for pagination normalization
            await base.ValidateQueryParams(p, ct);

            // Validate category exists if specified
            if (p.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(p.CategoryId.Value, ct);
                if (category == null)
                {
                    _logger.LogWarning("Requested category {CategoryId} does not exist", p.CategoryId);
                    throw new KeyNotFoundException($"Category with ID {p.CategoryId} not found");
                }
            }
        }

        protected override bool HasSearchCriteria(ProductQueryParams p)
        {
            return !string.IsNullOrWhiteSpace(p.Search);
        }

        protected override IQueryable<Product> ApplyDefaultSorting(IQueryable<Product> queryable)
        {
            // Business rule: Default sort by most recent first, then by name
            return queryable
                .OrderByDescending(p => p.CreatedAt)
                .ThenBy(p => p.Name);
        }

        #endregion

        #region Business Validation Methods

        private async Task ValidateProductBusinessRules(Product product)
        {
            // Validate price
            if (product.Price <= 0)
            {
                throw new ArgumentException("Product price must be greater than zero");
            }

            // Validate stock
            if (product.InStock < 0)
            {
                throw new ArgumentException("Product stock cannot be negative");
            }

            // Validate name uniqueness within category (optional business rule)
            var existingProducts = await _productRepository.GetFilteredAsync(categoryId: product.CategoryId);
            var duplicateName = existingProducts.Any(p => 
                p.Id != product.Id && 
                p.Name.Equals(product.Name, StringComparison.OrdinalIgnoreCase));

            if (duplicateName)
            {
                _logger.LogWarning("Product with similar name already exists in category {CategoryId}: {ProductName}", 
                    product.CategoryId, product.Name);
                
            }
        }

        private async Task ValidateProductDeletionRules(Product product)
        {
            var categoryProducts = await _productRepository.GetFilteredAsync(categoryId: product.CategoryId);
            var activeProductsInCategory = categoryProducts.Count(p => p.IsActive && p.Id != product.Id);

            if (activeProductsInCategory == 0)
            {
                _logger.LogWarning("Deleting the last active product in category {CategoryId}", product.CategoryId);
            }
        }
        #endregion
    }
}
