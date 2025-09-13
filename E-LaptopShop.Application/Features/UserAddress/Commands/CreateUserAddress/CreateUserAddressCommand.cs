using E_LaptopShop.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Commands.CreateUserAddress
{
    public class CreateUserAddressCommand : IRequest<UserAddressDto>
    {
        public int UserId { get; set; }
        public string AddressLine { get; set; } = null!;
        public string City { get; set; } = null!;
        public string District { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Ward { get; set; } = null!;
        public bool IsDefault { get; set; } = false;
        public string CountryCode { get; set; } = "VN";
        public string? PostalCode { get; set; }

    }
}
