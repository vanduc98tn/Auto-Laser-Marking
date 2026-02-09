using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class LotInData
    {

        public bool IsConfigOK { get; set; }
        public String WorkGroup { get; set; }
        public String DeviceId { get; set; }
        public String LotId { get; set; }

        public int LotQty { get; set; }
        public int TotalCount { get; set; }

        public double Yield { get; set; }
        public int OKCount { get; set; }
        public int NGCount { get; set; }
        public LotInData()
        {
            this.WorkGroup = "ITM Group";
            this.DeviceId = string.Empty;
            this.LotId = string.Empty;
            this.LotQty = 0;
            this.TotalCount = 0;
            this.Yield = 0;
            this.OKCount = 0;
            this.NGCount = 0;
            this.IsConfigOK = false;
        }
    }
}
