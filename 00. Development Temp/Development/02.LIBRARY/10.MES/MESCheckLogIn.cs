using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class MESCheckLogIn
    {
        public string Type { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public MESCheckLogIn()
        {
            Type = "ME";
        }
    }
}
