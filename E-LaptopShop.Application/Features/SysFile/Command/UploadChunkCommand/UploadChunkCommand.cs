using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.Features.SysFile.Command.UploadChunkCommand
{
    public class UploadChunkCommand : IRequest<ChunkUploadResult>
    {
        public IFormFile Chunk { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public int ChunkNumber { get; set; }
        public int TotalChunks { get; set; }
        public string UploadedBy { get; set; } = null!;

        /// <summary>
        /// Subfolder context — IIS static middleware map /image/* → uploads/image/*.
        /// Ví dụ: "products/23" → file lưu tại uploads/image/products/23/{guid}_name.jpg
        ///                        URL trả về: /image/products/23/{guid}_name.jpg
        /// Để null/empty → flat uploads/image/ (backward compat với avatar cũ).
        /// </summary>
        public string? UploadContext { get; set; }
    }

    public class ChunkUploadResult
    {
        public bool IsCompleted { get; set; }
        public int? SysFileId { get; set; }
        public string? FileUrl { get; set; }
    }
}
