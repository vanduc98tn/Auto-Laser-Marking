using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    internal class FpcbData
    {
        public int TotalCountFpcb { get; set; }
        public int NGmarked { get; set; }
        public int NGmarking { get; set; }
        public int OKCount { get; set; }
        public double NGmarkedPer { get; set; }
        public double NGmarkingPer { get; set; }
        public double OKCountPer { get; set; }

        public FpcbData()
        {
            this.TotalCountFpcb = 0;
            this.NGmarked = 0;
            this.NGmarking = 0;
            this.OKCount = 0;
            this.NGmarkedPer = 0;
            this.NGmarkingPer = 0;
            this.OKCountPer = 0;
        }
    }
}
