using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Entities
{
    public partial class InventoryHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int InventoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string TransactionType { get; set; } = null!; // Purchase, Sale, Return, Adjustment

        [Required]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitCost { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        public int? ReferenceId { get; set; }

        [StringLength(50)]
        public string? ReferenceType { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        // Navigation properties
        [ForeignKey("InventoryId")]
        public virtual Inventory Inventory { get; set; } = null!;
    }
}
