using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_LaptopShop.Domain.Entities
{
    public partial class ShoppingCartItem
    {
        public int Id { get; set; }

        public int ShoppingCartId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; } = 1;

        public DateTime AddedAt { get; set; } = DateTime.Now;

        public decimal UnitPrice { get; set; }

        public virtual ShoppingCart ShoppingCart { get; set; } = null!;

        public virtual Product Product { get; set; } = null!;
    }
}
