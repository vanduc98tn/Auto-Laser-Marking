using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    class ModelSettings
    {
        public const int DEFAULT_MODEL_NO = 1;
        public const String DEFAULT_MODEL_NAME = "default";
        public DateTime updatedTime { get; set; }
        public String modelName { get; set; }
        public int indexModel { get; set; }
        public LaserModel LaserModel { get; set; }
        public VisionModel VisionModel { get; set; }
        public ModelSettings()
        {
            this.indexModel = 0;
            this.updatedTime = DateTime.Now;
            this.modelName = DEFAULT_MODEL_NAME;
            this.LaserModel = new LaserModel();
            this.VisionModel = new VisionModel();
            
        }
        public ModelSettings Appsetting()
        {
            return new ModelSettings
            {
                indexModel = this.indexModel,
                updatedTime = this.updatedTime,
                modelName = this.modelName,
                LaserModel = UiManager.appSetting.laserModel != null ? UiManager.appSetting.laserModel.Clone() : new LaserModel(),
                VisionModel = UiManager.appSetting.visionModel != null ? UiManager.appSetting.visionModel.Clone() : new VisionModel(),

            };
        }
        public ModelSettings Clone()
        {
            return new ModelSettings
            {
                indexModel = this.indexModel,
                updatedTime = this.updatedTime,
                modelName = string.Copy(this.modelName),
                LaserModel = this.LaserModel.Clone(),
                VisionModel = this.VisionModel.Clone(),

            };
        }
        public Boolean HasSameModel(ModelSettings x)
        {
            if (x == null) return false;
            // Prefer matching by indexModel when it's set (> 0), otherwise fall back to modelName
            if (this.indexModel > 0 && x.indexModel > 0)
            {
                return this.indexModel == x.indexModel;
            }
            if (!string.IsNullOrEmpty(this.modelName) && !string.IsNullOrEmpty(x.modelName))
            {
                return this.modelName.Equals(x.modelName);
            }
            return false;
        }
    }
}
