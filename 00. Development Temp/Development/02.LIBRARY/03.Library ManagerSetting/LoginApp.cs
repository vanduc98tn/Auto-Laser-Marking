using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class LoginApp
    {
        public String PassWordEN { get; set; }
        public String PassWordADM { get; set; }
        public String UseName { get; set; }
        public String UseNameEN { get; set; }
        public String UseNameADM { get; set; }
        public String UseNameOPE { get; set; }
        public string LabelMesNameOPE { get; set; }
        public string LabelMesNameME { get; set; }
        public LoginApp()
        {
            UseName = "Operator";
            PassWordEN = "itm";
            PassWordADM = "itm";
            UseNameEN = "Manager";
            UseNameADM = "AutoTeam";
            UseNameOPE = "Operator";

            LabelMesNameME = "ME";
            LabelMesNameOPE = "OP";
        }
    }
}
