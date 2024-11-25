using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myggot.Classes
{
    public class PointHistory
    {
        public int PointId { get; set; }
        public int? Point { get; set; }
        public DateTime? PointDate { get; set; }
        public int? Reward { get; set; }

        // Foreign Key
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
