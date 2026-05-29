using E_LaptopShop.Application.Common;
using E_LaptopShop.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.SysFile.Command.UploadChunkCommand
{
    public class UploadChunkCommandHandler : IRequestHandler<UploadChunkCommand, ChunkUploadResult>
    {
        private readonly ISysFileRepository _sysFileRepository;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly string _tempUploadsFolder;
        private readonly string _uploadsFolder;

        public UploadChunkCommandHandler(ISysFileRepository sysFileRepository, IHostEnvironment hostEnvironment)
        {
            _sysFileRepository = sysFileRepository;
            _hostEnvironment = hostEnvironment;
            _tempUploadsFolder = Path.Combine(_hostEnvironment.ContentRootPath, "temp-uploads");
            _uploadsFolder = Path.Combine(_hostEnvironment.ContentRootPath, "uploads");

            // Đảm bảo thư mục tồn tại
            if (!Directory.Exists(_tempUploadsFolder))
                Directory.CreateDirectory(_tempUploadsFolder);

            if (!Directory.Exists(_uploadsFolder))
                Directory.CreateDirectory(_uploadsFolder);
        }

        public async Task<ChunkUploadResult> Handle(UploadChunkCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Tạo thư mục cho file chunks nếu chưa tồn tại
                var safeFileName = StringHelper.SlugifyFileName(request.FileName);
                var fileFolder = Path.Combine(_tempUploadsFolder, safeFileName);
                if (!Directory.Exists(fileFolder))
                    Directory.CreateDirectory(fileFolder);

                // Lưu chunk
                var chunkPath = Path.Combine(fileFolder, $"chunk_{request.ChunkNumber}");
                using (var stream = new FileStream(chunkPath, FileMode.Create))
                {
                    await request.Chunk.CopyToAsync(stream, cancellationToken);
                }

                // Kiểm tra xem tất cả chunks đã được upload chưa
                var uploadedChunks = Directory.GetFiles(fileFolder).Length;
                if (uploadedChunks != request.TotalChunks)
                    return new ChunkUploadResult { IsCompleted = false };

                // Xác định loại file và tạo thư mục tương ứng
                string fileExtension = Path.GetExtension(request.FileName).ToLowerInvariant();
                string fileType = DetermineFileType(fileExtension);

                // Tạo thư mục theo loại file
                string targetFolder = Path.Combine(_uploadsFolder, fileType);
                string fileUrlPrefix = $"/{fileType}";
          
                // Đảm bảo thư mục tồn tại
                if (!Directory.Exists(targetFolder))
                    Directory.CreateDirectory(targetFolder);
                // Ghép các chunks
                var slugifiedFileName = StringHelper.Slugify(request.FileName);
                var uniqueId = Guid.NewGuid().ToString("N")[..8];

                // Dùng context làm prefix tên file thay vì tạo subdirectory.
                // Tránh lỗi permission khi IIS không có quyền tạo folder mới.
                // "products/1" → "products-1-" | "avatars" → "avatars-" | null → ""
                var contextPrefix = string.IsNullOrWhiteSpace(request.UploadContext)
                    ? string.Empty
                    : request.UploadContext
                        .Replace("/", "-")
                        .Replace("\\", "-")
                        .Trim('-') + "-";

                var combinedFileName = $"{contextPrefix}{uniqueId}_{slugifiedFileName}";
                var finalFilePath    = Path.Combine(targetFolder, combinedFileName);
                var fileUrl          = $"{fileUrlPrefix}/{combinedFileName}";

                await CombineChunksAsync(fileFolder, finalFilePath, request.TotalChunks);

                // Xóa thư mục tạm
                Directory.Delete(fileFolder, true);

                // Lấy thông tin file
                var fileInfo = new FileInfo(finalFilePath);

                // Tạo bản ghi SysFile
                var sysFile = new Domain.Entities.SysFile
                {
                    FileName = request.FileName,
                    FilePath = finalFilePath,
                    FileUrl = fileUrl,   // ← có guid, không conflict
                    FileType = Path.GetExtension(request.FileName).TrimStart('.'),
                    FileSize = fileInfo.Length,
                    StorageType = "local",
                    UploadedBy = request.UploadedBy,
                    UploadedAt = DateTime.Now,
                    IsActive = true
                };

                var savedSysFile = await _sysFileRepository.AddAsync(sysFile, cancellationToken);
                return new ChunkUploadResult
                {
                    IsCompleted = true,
                    SysFileId = savedSysFile.Id,
                    FileUrl = savedSysFile.FileUrl
                };
            }
            catch (Exception ex)
            {
                // Log exception
                throw new ApplicationException("Error processing file chunks", ex);
            }
        }

        private string DetermineFileType(string extension)
        {
            if (new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" }.Contains(extension))
                return "image";
            else if (new[] { ".mp4", ".avi", ".mov", ".wmv", ".flv", ".mkv" }.Contains(extension))
                return "video";
            else if (new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx" }.Contains(extension))
                return "document";
            else
                return "other";
        }

        private async Task CombineChunksAsync(string chunksFolder, string finalFilePath, int totalChunks)
        {
            using (var destStream = new FileStream(finalFilePath, FileMode.Create))
            {
                for (int i = 1; i <= totalChunks; i++)
                {
                    var chunkPath = Path.Combine(chunksFolder, $"chunk_{i}");
                    using (var sourceStream = new FileStream(chunkPath, FileMode.Open))
                    {
                        await sourceStream.CopyToAsync(destStream);
                    }
                }
            }
        }
    }
}
