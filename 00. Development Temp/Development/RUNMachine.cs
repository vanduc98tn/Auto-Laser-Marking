using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
     class RUNMachine
    {
        public int Patern{ get; set; }
        public bool MESOnline { get; set; }
        public bool CheckScanner { get; set; }
        public string[] MES_EXCLUSION { get; set; }

        public RUNMachine()
        {
            this.Patern = 1;
            this.MESOnline = false;
            this.CheckScanner = false;
            this.MES_EXCLUSION = new string[] { };
        }
        public RUNMachine Clone()
        {
            return new RUNMachine()
            {
                Patern = this.Patern,
                MESOnline = this.MESOnline,
                CheckScanner = this.CheckScanner,
                MES_EXCLUSION = this.MES_EXCLUSION,

            };
        }
    }
}
