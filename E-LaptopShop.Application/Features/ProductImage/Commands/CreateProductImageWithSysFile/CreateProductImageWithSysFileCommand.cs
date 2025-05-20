using E_LaptopShop.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.ProductImage.Commands.CreateProductImageWithSysFile
{
    public class CreateProductImageWithSysFileCommand : IRequest<ProductImageDto>
    {
        public int ProductId { get; set; }
        public int SysFileId { get; set; }
        public bool IsMain { get; set; }
        public string? AltText { get; set; }
        public string? Title { get; set; }
        public string? CreatedBy { get; set; }
    }
}
