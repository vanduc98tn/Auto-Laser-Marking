using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class NotifyPLCDWord_ZR: IObserverChangeDWord_ZR
    {
        private List<IObserverChangeDWord_ZR> observers = new List<IObserverChangeDWord_ZR>();
        public List<IObserverChangeDWord_ZR> Observers => observers;
        public void Attach(IObserverChangeDWord_ZR observer)
        {
            this.observers.Add(observer);
        }

        public void Detach(IObserverChangeDWord_ZR observer)
        {
            this.observers.Remove(observer);
        }

        public void NotifyChangeDWord_ZR(string key, int value)
        {
            foreach (IObserverChangeDWord_ZR ob in new List<IObserverChangeDWord_ZR>(this.observers))
            {
               ob.NotifyChangeDWord_ZR(key, value);
            }
        }
    }
}
