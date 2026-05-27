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
        public int Id { get; set; }
        public int InventoryId { get; set; }
        public string TransactionType { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public int? ReferenceId { get; set; }
        public string? ReferenceType { get; set; }
        public string? Notes { get; set; }
        public string? CreatedBy { get; set; }
        public virtual Inventory Inventory { get; set; } = null!;
    }
}
