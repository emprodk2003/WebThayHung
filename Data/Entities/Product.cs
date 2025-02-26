using Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Product
    {
        public int product_id { get; set; }
        public string product_name { get; set; }

        public decimal price { get; set; }
        public int discount { get; set; }
        public StatusProduct status { get; set; }
        public int IsFeatured { get; set; }
        public int type_id { get; set; }
        public string Desdescription {  get; set; }
        public int advertisement_id { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }
        public Category Category { get; set; }
        public List<Cart> Carts { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public List<Variants_product> variants { get; set; }
        public Advertisement advertisement { get; set; }
    }
}
