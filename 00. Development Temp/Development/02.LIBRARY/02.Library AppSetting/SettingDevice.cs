using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class SettingDevice
    {
        #region Setting PLC
        public TCPSetting MC_TCP_Binary { get; set; }
        public SC09Setting sc09Setting { get; set; }
        public ModbusCOMSetting XGTServerCOMSetting { get; set; }
        public TCPSetting LSXGTServerTCPSetting { get; set; }
        public COMSetting COMsetting { get; set; }
        #endregion


        public TCPSetting settingTCPTranferVision { get; set; }
        public TCPSetting settingTCPMES { get; set; }
        public COMSetting ScannerCOM { get; set; }
        public TCPSetting ScannerTCP { get; set; }
        public COMSetting COMTestter { get; set; }

        public SettingDevice()
        {
            #region Setting PLC
            XGTServerCOMSetting = new ModbusCOMSetting();
            MC_TCP_Binary = new TCPSetting();
            sc09Setting = new SC09Setting();
            COMsetting = new COMSetting();
            LSXGTServerTCPSetting = new TCPSetting();
            #endregion

            settingTCPTranferVision = new TCPSetting();
            settingTCPMES = new TCPSetting();
            ScannerCOM = new COMSetting();
            ScannerTCP = new TCPSetting();
            COMTestter = new COMSetting();

        }
    }
}
