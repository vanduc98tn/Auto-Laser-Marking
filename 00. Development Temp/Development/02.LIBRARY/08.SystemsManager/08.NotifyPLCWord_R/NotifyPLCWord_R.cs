using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{

    public class NotifyPLCWord_R : IObserverChangeWord_R
    {
        private List<IObserverChangeWord_R> observers = new List<IObserverChangeWord_R>();
        public List<IObserverChangeWord_R> Observers => observers;
        public void Attach(IObserverChangeWord_R observer)
        {
            this.observers.Add(observer);
        }

        public void Detach(IObserverChangeWord_R observer)
        {
            this.observers.Remove(observer);
        }

        public void NotifyChangeWord_R(string key, int value)
        {
            foreach (IObserverChangeWord_R ob in new List<IObserverChangeWord_R>(this.observers))
            {
                ob.NotifyChangeWord_R(key, value);
            }
        }
    }
}
