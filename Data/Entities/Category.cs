using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Category
    {
        public int type_id { get; set; }

        public string type_name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
