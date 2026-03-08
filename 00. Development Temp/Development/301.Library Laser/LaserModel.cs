using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    internal class LaserModel
    {
        public PatternSetting pattern { get; set; }
        public RUNMachine run { get; set; }
        

        public LaserModel()
        {
            this.pattern = new PatternSetting();
            this.run = new RUNMachine();
        }

        public LaserModel Clone()
        {
            return new LaserModel()
            {
                pattern = this.pattern,
                run = this.run,
            };
        }

    }
}
