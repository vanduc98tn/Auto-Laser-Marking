using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class NotifyPLCWord : IObserverChangeWords
    {
        private List<IObserverChangeWords> observers = new List<IObserverChangeWords>();
        public List<IObserverChangeWords> Observers => observers;
        public void Attach(IObserverChangeWords observer)
        {
            this.observers.Add(observer);
        }

        public void Detach(IObserverChangeWords observer)
        {
            this.observers.Remove(observer);
        }

        public void NotifyChangeWord(string key, short value)
        {
            foreach (IObserverChangeWords ob in new List<IObserverChangeWords>(this.observers))
            {
                ob.NotifyChangeWord(key, value);
            }
        }
    }
}
