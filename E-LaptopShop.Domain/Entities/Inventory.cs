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
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int CurrentStock { get; set; } = 0;
        public int MinimumStock { get; set; } = 5;
        public int ReorderPoint { get; set; } = 10;
        public decimal AverageCost { get; set; } = 0;
        public decimal LastPurchasePrice { get; set; } = 0;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
        public string? Location { get; set; }
        public virtual Product Product { get; set; } = null!;
        public virtual ICollection<InventoryHistory> HistoryRecords { get; set; } = new List<InventoryHistory>();
        public InventoryStatus Status { get; set; } = InventoryStatus.Active;
    }
}
