using System.Threading;
using System.Threading.Tasks;
using MediatR;
using E_LaptopShop.Domain.Repositories;
using E_LaptopShop.Application.Services.Interfaces;

namespace E_LaptopShop.Application.Features.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly ICategoryService _categoryService;
        public DeleteCategoryCommandHandler(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var deletedId = await _categoryService.DeleteAsync(request.Id, cancellationToken);
            return deletedId;
        }
    }
} 