using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public interface IObserverChangeBits
    {
        void NotifyChangeBits(string key, bool status);
    }
}
