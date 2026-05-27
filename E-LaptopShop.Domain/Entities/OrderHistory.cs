using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Entities
{
    public partial class OrderHistory
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string OldStatus { get; set; } = null!;
        public string NewStatus { get; set; } = null!;
        public DateTime ChangedAt { get; set; } = DateTime.Now;
        public string? ChangedBy { get; set; }
        public string? Notes { get; set; }
        public virtual Order Order { get; set; } = null!;
    }
}
