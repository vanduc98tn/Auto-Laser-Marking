using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    class ModelSettings
    {
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
                modelName = string.Copy(this.modelName),
                LaserModel = UiManager.appSetting.laserModel,
                VisionModel = UiManager.appSetting.visionModel,

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
            if (this.modelName != null && x != null && (this.modelName.Equals(x.modelName) || this.indexModel == x.indexModel))
            {
                return true;
            }
            return false;
        }
    }
}
