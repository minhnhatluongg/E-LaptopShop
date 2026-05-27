using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Entities
{
    public partial class ShoppingCart
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; } = 0;

        public virtual User User { get; set; } = null!;

        public virtual ICollection<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
    }
}
