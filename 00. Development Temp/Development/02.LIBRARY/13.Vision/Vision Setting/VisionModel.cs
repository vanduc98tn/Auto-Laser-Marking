using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class VisionModel
    {
        public ROISettings ROI { get; set; }
        public int WhitePixels { get; set; }
        public int BlackPixels { get; set; }
        public int MatchingRate { get; set; }
        public int MatchingRateMin { get; set; }
        public int Threshol { get; set; }
        public int ThresholBl { get; set; }
        public bool CirWhCntEnb { get; set; }
        public bool RoiWhCntEnb { get; set; }
        public OpenCvSharp.Point BarCodeOffSet { get; set; }
        public Boolean OffSetJigEnb { get; set; }

        public double Gamma { get; set; }
        public double GammaSld { get; set; }
        public double AlphaSld { get; set; }
        public double BetaSld { get; set; }
        public ImageLoggerSetting ImageLogger { get; set; }

        public VisionModel()
        {
            this.ROI = new ROISettings();
            this.MatchingRate = 100;
            this.MatchingRateMin = 70;
            this.BarCodeOffSet = new OpenCvSharp.Point { };
            this.WhitePixels = 100;
            this.BlackPixels = 10;
            this.Threshol = 127;
            this.ThresholBl = 40;
            this.OffSetJigEnb = false;
            this.CirWhCntEnb = true;
            this.RoiWhCntEnb = false;
            Gamma = 2.0;
            GammaSld = 1.0;
            AlphaSld = 1.5;
            BetaSld = 0.0;
            ImageLogger = new ImageLoggerSetting();
        }

        public VisionModel Clone()
        {
            return new VisionModel()
            {
                ROI = this.ROI,
                WhitePixels = this.WhitePixels,
                BlackPixels = this.BlackPixels,
                MatchingRate = this.MatchingRate,
                MatchingRateMin = this.MatchingRateMin,
                BarCodeOffSet = this.BarCodeOffSet,
                OffSetJigEnb = this.OffSetJigEnb,
                Threshol = this.Threshol,
                ThresholBl = this.ThresholBl,
                CirWhCntEnb = this.CirWhCntEnb,
                RoiWhCntEnb = this.RoiWhCntEnb,
                Gamma = this.Gamma,
                GammaSld = this.GammaSld,
                AlphaSld = this.AlphaSld,
                BetaSld = this.BetaSld,
                ImageLogger = this.ImageLogger,
            };
        }
    }
}
