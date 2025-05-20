using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.SysFile.Command.UploadFileCommand
{
    public class UploadFileCommandHandler : IRequestHandler<UploadFileCommand, int>
    {
        private readonly ISysFileRepository _sysFileRepository;
        private readonly IHostEnvironment _hostEnvironment;

        public UploadFileCommandHandler(ISysFileRepository sysFileRepository, IHostEnvironment hostEnvironment)
        {
            _sysFileRepository = sysFileRepository;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<int> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + request.File.FileName;

            var uploadsFolder = Path.Combine(_hostEnvironment.ContentRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(fileStream, cancellationToken);
            }

            var sysFile = new E_LaptopShop.Domain.Entities.SysFile
            {
                FileName = request.File.FileName,
                FilePath = filePath,
                FileUrl = $"/uploads/{uniqueFileName}",
                FileType = Path.GetExtension(request.File.FileName).TrimStart('.'),
                FileSize = request.File.Length,
                StorageType = "local",
                UploadedBy = request.UploadedBy,
                UploadedAt = DateTime.Now,
                IsActive = true
            };

            return await _sysFileRepository.AddAsync(sysFile, cancellationToken);
        }
    }
}
