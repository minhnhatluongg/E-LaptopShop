using E_LaptopShop.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.UserAddress.Commands.UpdateUserAddress
{
    public class UpdateUserAddressCommand : IRequest<UserAddressDto>
    {
        public int Id { get; set; }
        public int UserId { get; set; }


        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? AddressLine { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }

        public bool IsDefault { get; set; } = false;
        public string CountryCode { get; set; } = "VN";
        public string? PostalCode { get; set; }
    }
}
