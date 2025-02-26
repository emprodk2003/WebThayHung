using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Variants_product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int product_id {  get; set; }
        public Product Product { get; set; }
        public ICollection<Size_Product>  Size { get; set; }
    }
}
