using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class NotifyPLCDWord_R : IObserverChangeDWord_R
    {
        private List<IObserverChangeDWord_R> observers = new List<IObserverChangeDWord_R>();
        public List<IObserverChangeDWord_R> Observers => observers;
        public void Attach(IObserverChangeDWord_R observer)
        {
            this.observers.Add(observer);
        }

        public void Detach(IObserverChangeDWord_R observer)
        {
            this.observers.Remove(observer);
        }

        public void NotifyChangeDWord_R(string key, int value)
        {
            foreach (IObserverChangeDWord_R ob in new List<IObserverChangeDWord_R>(this.observers))
            {
                ob.NotifyChangeDWord_R(key, value);
            }
        }
    }
}
