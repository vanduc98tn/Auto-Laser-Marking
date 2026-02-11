using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class ConnectionSettings
    {
        public CamSettings camSettings { get; set; }
        public ConnectionSettings()
        {
            this.camSettings = new CamSettings();
        }

        public ConnectionSettings Clone()
        {
            return new ConnectionSettings
            {
                camSettings = this.camSettings.Clone()
            };
        }
    }
}
