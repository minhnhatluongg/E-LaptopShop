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
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        [StringLength(50)]
        public string OldStatus { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string NewStatus { get; set; } = null!;

        [Column(TypeName = "datetime")]
        public DateTime ChangedAt { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string? ChangedBy { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;
    }
}
