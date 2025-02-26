using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }

        public int product_id { get; set; }

        public string ImagePath { get; set; }

        public string Caption { get; set; }

        public DateTime DateCreated { get; set; }

        public Product Product { get; set; }
    }
}
