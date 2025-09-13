using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Application.DTOs
{
    public class SysFileDto
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? FileUrl { get; set; }
        public string? FileType { get; set; }
        public long FileSize { get; set; }
        public string? StorageType { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
