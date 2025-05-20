using E_LaptopShop.Application.Features.User.Commands.UpdateUser;
using E_LaptopShop.Domain.Repositories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.User.Validations
{
    public class UpdateUserCommandValidations : AbstractValidator<UpdateUserCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        public UpdateUserCommandValidations(IUserRepository userRepository, IRoleRepository roleRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;

            // ID validation
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID không được để trống.")
                .GreaterThan(0).WithMessage("ID phải lớn hơn 0.")
                .MustAsync(UserExists).WithMessage("Người dùng không tồn tại.");

            // FirstName validations
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Họ không được để trống.")
                .MinimumLength(2).WithMessage("Họ phải có ít nhất 2 ký tự.")
                .MaximumLength(50).WithMessage("Họ không được vượt quá 50 ký tự.")
                .Matches("^[a-zA-ZÀ-ỹà-ỹ\\s]+$").WithMessage("Họ chỉ được chứa chữ cái và khoảng trắng.");

            // LastName validations
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Tên không được để trống.")
                .MinimumLength(2).WithMessage("Tên phải có ít nhất 2 ký tự.")
                .MaximumLength(50).WithMessage("Tên không được vượt quá 50 ký tự.")
                .Matches("^[a-zA-ZÀ-ỹà-ỹ\\s]+$").WithMessage("Tên chỉ được chứa chữ cái và khoảng trắng.");

            // Email validations (optional for update)
            When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
            {
                RuleFor(x => x.Email)
                    .EmailAddress().WithMessage("Email không đúng định dạng.")
                    .MaximumLength(100).WithMessage("Email không được vượt quá 100 ký tự.")
                    .MustAsync((command, email, cancellationToken) =>
                        BeUniqueEmail(email, command.Id, cancellationToken))
                    .WithMessage("Email đã tồn tại trong hệ thống.");
            });

            // Password validations (optional for update)
            When(x => !string.IsNullOrWhiteSpace(x.Password), () =>
            {
                RuleFor(x => x.Password)
                    .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 ký tự.")
                    .MaximumLength(100).WithMessage("Mật khẩu không được vượt quá 100 ký tự.")
                    .Matches("[A-Z]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ hoa.")
                    .Matches("[a-z]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ thường.")
                    .Matches("[0-9]").WithMessage("Mật khẩu phải chứa ít nhất 1 chữ số.")
                    .Matches("[^a-zA-Z0-9]").WithMessage("Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt.");
            });

            // Phone validations (optional)
            When(x => !string.IsNullOrWhiteSpace(x.Phone), () =>
            {
                RuleFor(x => x.Phone)
                    .Matches("^[0-9]{10,11}$").WithMessage("Số điện thoại không hợp lệ.")
                    .MaximumLength(20).WithMessage("Số điện thoại không được vượt quá 20 ký tự.");
            });

            // RoleId validations (optional for update)
            When(x => x.RoleId.HasValue, () =>
            {
                RuleFor(x => x.RoleId.Value)
                    .GreaterThan(0).WithMessage("ID vai trò phải lớn hơn 0.")
                    .MustAsync(RoleExists).WithMessage("Vai trò không tồn tại.");
            });

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
                RuleFor(x => x.DateOfBirth.Value)
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
            private async Task<bool> UserExists(int id, CancellationToken cancellationToken)
            {
                try
                {
                    var user = await _userRepository.GetByIdAsync(id, cancellationToken);
                    return user != null;
                }
                catch
                {
                    return false;
                }
            }

            private async Task<bool> BeUniqueEmail(string email, int excludeId, CancellationToken cancellationToken)
            {
                return await _userRepository.IsEmailUniqueAsync(email, excludeId, cancellationToken);
            }

            private async Task<bool> RoleExists(int roleId, CancellationToken cancellationToken)
            {
                try
                {
                    var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
                    return role != null && role.IsActive;
                }
                catch
                {
                    return false;
                }
            }
    }
}
