using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Entities
{
    public partial class SupplierOrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SupplierOrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitCost { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalCost { get; set; }

        public int ReceivedQuantity { get; set; } = 0;

        [StringLength(500)]
        public string? Notes { get; set; }

        // Navigation properties
        [ForeignKey("SupplierOrderId")]
        public virtual SupplierOrder SupplierOrder { get; set; } = null!;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }
}
