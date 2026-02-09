using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    class Mes00Check
    {
        public MESCheckLogIn MESCheckLogIn { get; set; }
        public bool SelectSendMes { get; set; }
        public string EquipmentId { get; set; }
        public string Status { get; set; }
        public string DIV { get; set; }
        public string CheckSum { get; set; }
        public string MES_Result { get; set; }
        public string MES_MSG { get; set; }


        public FormatS000 FormatS000 { get; set; }
        public FormatS001 FormatS001 { get; set; }
        public FormatS010 FormatS010 { get; set; }
        public FormatS011 FormatS011 { get; set; }
        public FormatS020 FormatS020 { get; set; }
        public FormatS021 FormatS021 { get; set; }

        public Mes00Check()
        {
            FormatS000 = new FormatS000();
            FormatS001 = new FormatS001();
            FormatS010 = new FormatS010();
            FormatS011 = new FormatS011();
            FormatS020 = new FormatS020();
            FormatS021 = new FormatS021();
            MESCheckLogIn = new MESCheckLogIn();
        }
    }
}
