using E_LaptopShop.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.User.Queries.CheckEmailExistsQuery
{
    public class CheckEmailExistsQueryHandler : IRequestHandler<CheckEmailExistsQuery, bool>
    {
        private readonly IUserRepository _userRepository;

        public CheckEmailExistsQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(CheckEmailExistsQuery request, CancellationToken cancellationToken)
        {
            // Nếu email là duy nhất, phương thức này trả về true
            // Ngược lại, trả về false nếu email đã tồn tại
            // Phải đảo logic vì chúng ta muốn trả về true nếu email tồn tại
            bool isUnique = await _userRepository.IsEmailUniqueAsync(request.Email, request.ExcludeId, cancellationToken);
            return !isUnique; // true nếu email đã tồn tại, false nếu email chưa tồn tại
        }
    }
}
