using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class ImageLoggerSetting
    {
        public string fileName { get; set; }
        public string folderPath { get; set; }
        public string imageFormat { get; set; }
        public bool isAddDateTime { get; set; }
        public double imageStorage { get; set; }
        public bool isUseFolderOK { get; set; }
        public bool isUseFolderNG { get; set; }
        public ImageLoggerSetting()
        {
            this.fileName = "Default";
            this.folderPath = @"C:\";
            this.imageFormat = "BMP";
            this.isAddDateTime = false;
            this.imageStorage = 5d;
            this.isUseFolderOK = false;
            this.isUseFolderNG = false;
        }
        public ImageLoggerSetting Clone()
        {
            return new ImageLoggerSetting()
            {
                fileName = this.fileName,
                folderPath = this.folderPath,
                imageFormat = this.imageFormat,
                isAddDateTime = this.isAddDateTime,
                imageStorage = this.imageStorage,
                isUseFolderOK = this.isUseFolderOK,
                isUseFolderNG = this.isUseFolderNG,
            };
        }
    }
}
