using E_LaptopShop.Application.Features.Brands.Commands.UpdateBrand;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Brands.Validations
{
    public class UpdateBrandCommandValidator : AbstractValidator<UpdateBrandCommand>
    {
        public UpdateBrandCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("ID thương hiệu không hợp lệ.");

            RuleFor(x => x.RequestDto)
                .NotNull()
                .WithMessage("Dữ liệu cập nhật Brand không được để trống.");

            RuleFor(x => x.RequestDto.Name)
                .NotEmpty()
                .WithMessage("Tên thương hiệu là bắt buộc.")
                .MaximumLength(100)
                .WithMessage("Tên thương hiệu không được vượt quá 100 ký tự.");
        }
    }
}
