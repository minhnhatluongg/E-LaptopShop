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
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; } = 0;

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [InverseProperty("ShoppingCart")]
        public virtual ICollection<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
    }
}
