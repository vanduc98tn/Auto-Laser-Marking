using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class MES01Check
    {
        public string EquipmentId { get; set; }
        public string Status { get; set; }
        public string LotNo { get; set; }
        public MESCheckLogIn MESCheckLogIn { get; set; }
        public string Config { get; set; }
        public string PCB_Code { get; set; }
        public string MES_Result { get; set; }
        public List<string> PCB_Result { get; set; }
        public string CheckSum { get; set; }
        public MES01Check()
        {
            this.PCB_Result = new List<string>();
            MESCheckLogIn = new MESCheckLogIn();
        }
    }
}
