using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
     class DataPCB
    {
        public List<string> RESULT_PCB  { get; set; }
        public string CUR_BIN_CHAR { get; set; }
        public string PRE_BIN_CODE { get; set; }
        public string SEND_CUR_BIN_MSG  { get; set; }
        public string RECEVIE_CUR_BIN_MSG { get; set; }
        public string BARCODE_PCB  { get; set; }
        public string TRAN_TIME { get; set; }
        public string WORK_IN_RESULT { get; set; }
        public string WORK_IN_MSG { get; set; }
        public string WORK_OUT_RESULT { get; set; }
        public string WORK_OUT_MSG { get; set; }



        public DataPCB()
        {
            this.RESULT_PCB = new List<string>();
            this.CUR_BIN_CHAR = string.Empty;
            this.PRE_BIN_CODE = string.Empty;
            this.SEND_CUR_BIN_MSG = string.Empty;
            this.RECEVIE_CUR_BIN_MSG = string.Empty;
            this.BARCODE_PCB = string.Empty;
            this.TRAN_TIME = string.Empty;
            this.WORK_IN_RESULT = string.Empty;
            this.WORK_IN_MSG = string.Empty;
            this.WORK_OUT_RESULT = string.Empty;
            this.WORK_OUT_MSG = string.Empty;
        }
    }
}
