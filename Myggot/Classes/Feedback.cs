using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myggot.Classes
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public string FeedbackText { get; set; }
        public int? Star { get; set; }

        // Foreign Key
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
