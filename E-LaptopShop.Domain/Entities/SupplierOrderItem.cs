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
        public int Id { get; set; }
        public int SupplierOrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public int ReceivedQuantity { get; set; } = 0;
        public string? Notes { get; set; }
        public virtual SupplierOrder SupplierOrder { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
