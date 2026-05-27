using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Entities
{
    public partial class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public string? ContactName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }

        public string? Website { get; set; }

        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public virtual ICollection<SupplierOrder> Orders { get; set; } = new List<SupplierOrder>();
    }
}
