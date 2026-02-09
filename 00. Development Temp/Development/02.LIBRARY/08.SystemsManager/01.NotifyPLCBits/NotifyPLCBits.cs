using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class NotifyPLCBits : IObserverChangeBits
    {
        private List<IObserverChangeBits> observers = new List<IObserverChangeBits>();
        public List<IObserverChangeBits> Observers => observers;
        public void Attach(IObserverChangeBits observer)
        {
            this.observers.Add(observer);
        }
        public void Detach(IObserverChangeBits observer)
        {
            this.observers.Remove(observer);
        }
        public void NotifyChangeBits(string key, bool status)
        {
            foreach (IObserverChangeBits ob in new List<IObserverChangeBits>(this.observers))
            {
                ob.NotifyChangeBits(key, status);
            }
        }
    }
}
