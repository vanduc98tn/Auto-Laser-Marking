using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
   
    public class NotifyPLCWord_ZR : IObserverChangeWord_ZR
    {
        private List<IObserverChangeWord_ZR> observers = new List<IObserverChangeWord_ZR>();
        public List<IObserverChangeWord_ZR> Observers => observers;
        public void Attach(IObserverChangeWord_ZR observer)
        {
            this.observers.Add(observer);
        }

        public void Detach(IObserverChangeWord_ZR observer)
        {
            this.observers.Remove(observer);
        }

        public void NotifyChangeWord_ZR(string key, int value)
        {
            foreach (IObserverChangeWord_ZR ob in new List<IObserverChangeWord_ZR>(this.observers))
            {
                ob.NotifyChangeWord_ZR(key, value);
            }
        }
    }
}
