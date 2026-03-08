using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    class ModelStore
    {
        private static String MODEL_SETINGS_FILE_NAME = "03.model_settings.json";
        private static MyLogger logger = new MyLogger("ModelStore");
        private static object lockObject = new object();

        public static ModelSettings GetModelSettings(String modelName)
        {
            ModelSettings ret = null;

            lock (lockObject)
            {
                try
                {
                    String filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), MODEL_SETINGS_FILE_NAME);
                    if (File.Exists(filePath))
                    {
                        using (StreamReader file = File.OpenText(filePath))
                        {
                            var js = file.ReadToEnd();
                            var specList = JsonConvert.DeserializeObject<ModelSettings[]>(js);
                            foreach (var x in specList)
                            {
                                if (x.modelName.Equals(modelName))
                                {
                                    ret = x.Clone();

                                    if (ret.updatedTime == null) { ret.updatedTime = DateTime.Now; }
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Create("Get Model Settings Error: " + ex.Message, LogLevel.Error);
                }
            }
            return ret;
        }
        public static void UpdateModelSettings(ModelSettings newSettings)
        {
            lock (lockObject)
            {
                try
                {
                    List<ModelSettings> modelList = new List<ModelSettings>(0);

                    String filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), MODEL_SETINGS_FILE_NAME);
                    if (File.Exists(filePath))
                    {
                        try
                        {
                            using (StreamReader file = File.OpenText(filePath))
                            {
                                var js = file.ReadToEnd();
                                modelList.AddRange(JsonConvert.DeserializeObject<ModelSettings[]>(js));
                            }
                        }
                        catch (Exception ex1)
                        {
                            logger.Create("Load JSON from File Error: " + ex1.Message, LogLevel.Error);
                        }
                    }

                    // Update Existing:
                    var hasUpdated = false;
                    for (int i = 0; i < modelList.Count; i++)
                    {
                        if (modelList[i].HasSameModel(newSettings))
                        {
                            modelList[i] = newSettings;
                            hasUpdated = true;
                            break;
                        }
                    }

                    // Add New:
                    if (!hasUpdated)
                    {
                        modelList.Add(newSettings);
                    }

                    // Store:
                    var jsNew = JsonConvert.SerializeObject(modelList, Formatting.Indented);
                    File.WriteAllText(filePath, jsNew);
                }
                catch (Exception ex)
                {
                    logger.Create("UpdateModelSettings Error: " + ex.Message, LogLevel.Error);
                }
            }
        }
        public static List<ModelInfo> GetModelInfoList()
        {
            List<ModelInfo> ret = new List<ModelInfo>();

            lock (lockObject)
            {
                try
                {
                    ModelInfo.ResetIndex();

                    String filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), MODEL_SETINGS_FILE_NAME);
                    if (File.Exists(filePath))
                    {
                        using (StreamReader file = File.OpenText(filePath))
                        {
                            var js = file.ReadToEnd();
                            var models = JsonConvert.DeserializeObject<ModelSettings[]>(js);
                            foreach (var x in models)
                            {
                                var modelInfo = new ModelInfo(x.indexModel, x.modelName, x.updatedTime);
                                ret.Add(modelInfo);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Create("GetModelInfoList Error: " + ex.Message, LogLevel.Error);
                }
            }
            return ret;
        }
        public static void DeleteModel(String model)
        {
            lock (lockObject)
            {
                try
                {
                    List<ModelSettings> modelList = new List<ModelSettings>(0);

                    String filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), MODEL_SETINGS_FILE_NAME);
                    if (File.Exists(filePath))
                    {
                        try
                        {
                            using (StreamReader file = File.OpenText(filePath))
                            {
                                var js = file.ReadToEnd();
                                modelList.AddRange(JsonConvert.DeserializeObject<ModelSettings[]>(js));
                            }
                        }
                        catch (Exception ex1)
                        {
                            logger.Create("Load JSON from Model Setting File Error: " + ex1.Message, LogLevel.Error);
                        }
                    }
                    var newList = new List<ModelSettings>();
                    var hasDelete = false;
                    for (int i = 0; i < modelList.Count; i++)
                    {
                        if (!modelList[i].modelName.Equals(model))
                        {
                            newList.Add(modelList[i]);
                        }
                        else
                        {
                            hasDelete = true;
                        }
                    }
                    if (hasDelete)
                    {
                        var jsNew = JsonConvert.SerializeObject(newList, Formatting.Indented);
                        File.WriteAllText(filePath, jsNew);
                    }
                }
                catch (Exception ex)
                {
                    logger.Create("Delete Model Error: " + ex.Message, LogLevel.Error);
                }
            }
        }
    }
    class ModelInfo
    {
        private static int _index = 0;

        public int Index { get; set; }
        public String Name { get; set; }
        public String Time { get; set; }

        public static void ResetIndex()
        {
            _index = 0;
        }
        public ModelInfo(int index, String name, DateTime updatedTime)
        {
            this.Index = index;
            this.Name = name;
            this.Time = updatedTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
