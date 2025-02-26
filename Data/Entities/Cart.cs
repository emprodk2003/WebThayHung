using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Cart
    {
        public int Id { set; get; }
        public int product_id { set; get; }
        public int Quantity { set; get; }
        public decimal Price { set; get; }
        public string Size { set; get; }
        public string Loai { set; get; }
        public DateTime DateCreated { get; set; }
        public Product Product { get; set; }
        public Guid UserId { set; get; }
        public User AppUser { get; set; }
    }
}
