using E_LaptopShop.Application.Features.User.Commands.CreateUser;
using E_LaptopShop.Domain.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.User.Validations
{
    public class CreateUserCommandValidations : AbstractValidator<CreateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public CreateUserCommandValidations(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;


            //kiểm tra first name
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Họ không được để trống")
                .MinimumLength(2).WithMessage("Họ phải có ít nhất 2 ký tự")
                .MaximumLength(50).WithMessage("Họ không được vượt quá 50 ký tự")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Họ chỉ được chứa chữ cái và khoảng trắng");

            //kiểm tra last name
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Tên không được để trống")
                .MinimumLength(2).WithMessage("Tên phải có ít nhất 2 ký tự")
                .MaximumLength(50).WithMessage("Tên không được vượt quá 50 ký tự")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("Tên chỉ được chứa chữ cái và khoảng trắng");

            //Kiểm tra email
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Email không hợp lệ")
                .MaximumLength(100).WithMessage("Email không được vượt quá 100 ký tự")
                .MustAsync(async (email, cancellation) =>
                {
                    var user = await _userRepository.GetByEmailAsync(email, cancellation);
                    return user == null;
                }).WithMessage("Email đã tồn tại trong hệ thống");

            //kiểm tra password
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống.")
                .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự.")
                .MaximumLength(50).WithMessage("Mật khẩu không được vượt quá 50 ký tự.")
                .Matches("[A-Z]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ hoa.")
                .Matches("[a-z]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ thường.")
                .Matches("[0-9]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ số.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt.");


            When(x => !string.IsNullOrWhiteSpace(x.Phone), () =>
            {
                RuleFor(x => x.Phone)
                    .Matches("^[0-9]{10,11}$").WithMessage("Số điện thoại không hợp lệ.")
                    .MaximumLength(13).WithMessage("Số điện thoại không được vượt quá 13 ký tự.");
            });

            // RoleId validations
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("Vai trò không được để trống.")
                .MustAsync(async (roleId, CancellationToken) =>
                {
                    var role = await _roleRepository.GetByIdAsync(roleId, CancellationToken);
                    return role != null && role.IsActive ;
                }).WithMessage("Vai trò không tồn tại.");

            // Gender validations (optional)
            When(x => !string.IsNullOrWhiteSpace(x.Gender), () =>
            {
                RuleFor(x => x.Gender)
                    .MaximumLength(50).WithMessage("Giới tính không được vượt quá 50 ký tự.")
                    .Must(gender => gender.ToLower() == "nam" || gender.ToLower() == "nữ" || gender.ToLower() == "male" || gender.ToLower() == "female" || gender.ToLower() == "other" || gender.ToLower() == "khác")
                    .WithMessage("Giới tính không hợp lệ.");
            });

            // DateOfBirth validations (optional)
            When(x => x.DateOfBirth.HasValue, () =>
            {
                RuleFor(x => x.DateOfBirth)
                    .LessThan(DateTime.Now).WithMessage("Ngày sinh không thể là ngày trong tương lai.")
                    .GreaterThan(DateTime.Now.AddYears(-120)).WithMessage("Ngày sinh không hợp lệ.");
            });

            // AvatarUrl validations (optional)
            When(x => !string.IsNullOrWhiteSpace(x.AvatarUrl), () =>
            {
                RuleFor(x => x.AvatarUrl)
                    .MaximumLength(255).WithMessage("URL ảnh đại diện không được vượt quá 255 ký tự.")
                    .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _)).WithMessage("URL ảnh đại diện không hợp lệ.");
            });
        }


    }
}
