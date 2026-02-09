using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public interface IObserverChangeDWord
    {
        void NotifyChangeDWord(string key, int value);
    }
}
