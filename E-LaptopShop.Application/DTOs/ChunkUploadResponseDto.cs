using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.DTOs
{
    public class ChunkUploadResponseDto
    {
        public bool IsCompleted { get; set; }
        public int? SysFileId { get; set; }
        public string? FileUrl { get; set; }
    }
}
