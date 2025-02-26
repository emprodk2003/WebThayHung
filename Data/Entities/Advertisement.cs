using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Advertisement
    {
        public int id {  get; set; }
        public string title {  get; set; }
        public string content {  get; set; }
        public DateTime create_at { get; set; }
        public int click {  get; set; }
        public DateTime start_date {  get; set; }
        public DateTime end_date { get; set; }
        public bool status { get; set; }
        public string img_path { get; set; }
        public List<Product> products { get; set; }
    }
}
