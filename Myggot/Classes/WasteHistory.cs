using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myggot.Classes
{
    public class WasteHistory
    {
        public int WasteId { get; set; }
        public int? Waste { get; set; }
        public DateTime? WasteDate { get; set; }

        // Foreign Key (ambiguity resolved in app logic)
        public int? UserId { get; set; }
        public User User { get; set; }
        public Farmer Farmer { get; set; }
    }
}
