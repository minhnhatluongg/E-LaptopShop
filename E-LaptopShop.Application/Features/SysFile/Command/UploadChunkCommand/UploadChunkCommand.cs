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
    }

    public class ChunkUploadResult
    {
        public bool IsCompleted { get; set; }
        public int? SysFileId { get; set; }
        public string? FileUrl { get; set; }
    }
}
