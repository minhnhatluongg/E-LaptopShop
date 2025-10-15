using E_LaptopShop.Application.Features.Brands.Commands.CreateBrand;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.Brands.Validations
{
    public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
    {
        public CreateBrandCommandValidator()
        {
            RuleFor(x => x.RequestDto)
                .NotNull()
                .WithMessage("Dữ liệu tạo Brand không được để trống.");

            RuleFor(x => x.RequestDto.Name)
                .NotEmpty()
                .WithMessage("Tên thương hiệu là bắt buộc.")
                .MaximumLength(100)
                .WithMessage("Tên thương hiệu không được vượt quá 100 ký tự.");

            When(x => !string.IsNullOrEmpty(x.RequestDto.Slug), () =>
            {
                RuleFor(x => x.RequestDto.Slug)
                    .MaximumLength(200)
                    .WithMessage("Slug không được vượt quá 200 ký tự.")
                    .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$") // Regex cho định dạng slug chuẩn
                    .WithMessage("Slug không hợp lệ. Chỉ chấp nhận chữ thường, số, và dấu gạch ngang.");
            });
        }
    }
}
