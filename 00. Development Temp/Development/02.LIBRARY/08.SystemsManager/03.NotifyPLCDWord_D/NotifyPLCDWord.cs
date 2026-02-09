using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class NotifyPLCDWord : IObserverChangeDWord
    {
        private List<IObserverChangeDWord> observers = new List<IObserverChangeDWord>();
        public List<IObserverChangeDWord> Observers => observers;
        public void Attach(IObserverChangeDWord observer)
        {
            this.observers.Add(observer);
        }

        public void Detach(IObserverChangeDWord observer)
        {
            this.observers.Remove(observer);
        }

        public void NotifyChangeDWord(string key, int value)
        {
            foreach (IObserverChangeDWord ob in new List<IObserverChangeDWord>(this.observers))
            {
                ob.NotifyChangeDWord(key, value);
            }
        }
    }
}
