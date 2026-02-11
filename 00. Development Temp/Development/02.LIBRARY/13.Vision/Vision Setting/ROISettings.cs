using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class ROISettings
    {
        public List<OpenCvSharp.Rect> listRectangle { get; set; }
        public Dictionary<int, int> listPLCBit { get; set; }

        public ROISettings()
        {
            listRectangle = new List<OpenCvSharp.Rect>();
            listPLCBit = new Dictionary<int, int>();
        }
        public ROISettings Clone()
        {
            return new ROISettings
            {
                listRectangle = this.listRectangle,
                listPLCBit = this.listPLCBit,
            };
        }
    }

    public class ROIProperty
    {
        public int StrokeThickness { get; set; }
        public int labelFontSize { get; set; }
        public OpenCvSharp.Size rectSize { get; set; }
        public ROIProperty()
        {
            StrokeThickness = 7;
            labelFontSize = 25;
            rectSize = new OpenCvSharp.Size(10, 10);

        }
        public ROIProperty Clone()
        {
            return new ROIProperty
            {
                StrokeThickness = this.StrokeThickness,
                labelFontSize = this.labelFontSize,
                rectSize = this.rectSize,
            };
        }

    }
}
