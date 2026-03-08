using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class MESSetting : TCPSetting
    {
        public string AUTHORITY { get; set; }
        public string ID { get; set; }
        public string EquimentID { get; set; }
        public string Repice { get; set; }
        public MESSetting()
        {
           
            this.EquimentID = "AUTO1234";
            this.Repice = "recipe1234";
            this.AUTHORITY = "No AUTHORITY";
            this.ID = "No ID";
        }
    }
}
