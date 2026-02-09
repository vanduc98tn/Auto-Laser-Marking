using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public interface IObserverChangeWords
    {
        void NotifyChangeWord(string key, short value);
    }
}
