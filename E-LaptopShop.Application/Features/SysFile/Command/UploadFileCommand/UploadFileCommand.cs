using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.SysFile.Command.UploadFileCommand
{
    public class UploadFileCommand : IRequest<int>
    {
        public IFormFile File { get; set; }
        public string UploadedBy { get; set; }
    }
}
