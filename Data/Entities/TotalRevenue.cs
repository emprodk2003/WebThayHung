using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class TotalRevenue
    {
        public int doanhthu_id { get; set; }
        public DateTime date { get; set; }
        public int numberofsales { get; set; }
        public decimal tongdoanhthu { get; set; }
    }
}
