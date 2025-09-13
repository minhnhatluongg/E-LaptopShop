using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_LaptopShop.Domain.Enums;

namespace E_LaptopShop.Domain.Entities
{
    public partial class Inventory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int CurrentStock { get; set; } = 0;

        public int MinimumStock { get; set; } = 5;

        public int ReorderPoint { get; set; } = 10;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal AverageCost { get; set; } = 0;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal LastPurchasePrice { get; set; } = 0;

        [Column(TypeName = "datetime")]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string? Location { get; set; }
        // Navigation properties
        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        [InverseProperty("Inventory")]
        public virtual ICollection<InventoryHistory> HistoryRecords { get; set; } = new List<InventoryHistory>();
        [Required]
        public InventoryStatus Status { get; set; } = InventoryStatus.Active;
    }
}
