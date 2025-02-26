using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Enum;


namespace Data.Entities
{
    public class Order
    {
        public int bill_id { set; get; }
        public DateTime OrderDate { set; get; }
       
        public string ShipName { set; get; }
        public string ShipAddress { set; get; }
        public string ShipEmail { set; get; }
        public string ShipPhoneNumber { set; get; }
        public decimal Totalprice {  set; get; }
        public decimal ShippingFee {  set; get; }
        public string Note {  set; get; }
        public Status Status { set; get; }
        public TransactionStatus transactionStatus {  set; get; }
        public List<OrderDetail> OrderDetails { get; set; }
        public User AppUser { set; get; }
        public Guid UserId { set; get; }
    }
}
