using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Entities
{
    public partial class SupplierOrder
    {
        public int Id { get; set; }

        public string PurchaseNumber { get; set; } = null!;

        public int SupplierId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pending";

        public decimal TotalAmount { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public virtual Supplier Supplier { get; set; } = null!;

        public virtual ICollection<SupplierOrderItem> Items { get; set; } = new List<SupplierOrderItem>();
    }
}
