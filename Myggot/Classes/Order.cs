using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myggot.Classes
{
    public class Order
    {
        public int OrderId { get; set; }
        public string OrderAddress { get; set; }
        public DateTime OrderDate { get; set; }
        public int? Weight { get; set; }
        public int? Point { get; set; }
        public bool InitializeOrder { get; set; } = false;
        public bool EndOrder { get; set; } = false;

        // Foreign Keys
        public int? OrderUserId { get; set; }
        public User OrderUser { get; set; }
        public int? OrderFarmerId { get; set; }
        public Farmer OrderFarmer { get; set; }
    }

}
