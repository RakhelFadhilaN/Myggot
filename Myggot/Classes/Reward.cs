using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myggot.Classes
{
    public class Reward
    {
        public int Id { get; set; }
        public string RewardName { get; set; }
        public int RewardPoints { get; set; }

        public ICollection<PointHistory> PointHistories { get; set; }
    }
}
