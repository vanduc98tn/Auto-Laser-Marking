using MvCamCtrl.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class CamSettings
    {
        public String name { get; set; }
        public String fileConf { get; set; }

        public int ExposeTime { get; set; }

        public MyCamera.MV_CC_DEVICE_INFO device { get; set; }

        public int OffsetAlignJigX { get; set; }
        public int OffsetAlignJigY { get; set; }
        public int mediumGrayVal { get; set; }
        public double scale { get; set; }
        public CamSettings()
        {
            this.name = "";
            this.fileConf = "";
            this.ExposeTime = 5000;
            this.OffsetAlignJigX = 10;
            this.OffsetAlignJigY = 10;
            this.mediumGrayVal = 10;
            this.scale = 0.12;
            this.device = new MyCamera.MV_CC_DEVICE_INFO();

        }
        public CamSettings Clone()
        {
            return new CamSettings
            {
                OffsetAlignJigX = this.OffsetAlignJigX,
                OffsetAlignJigY = this.OffsetAlignJigY,
                mediumGrayVal = this.mediumGrayVal,
                name = String.Copy(this.name),
                fileConf = String.Copy(this.fileConf),
                scale = this.scale,
                device = this.device,
                ExposeTime = this.ExposeTime

            };
        }
    }
}
