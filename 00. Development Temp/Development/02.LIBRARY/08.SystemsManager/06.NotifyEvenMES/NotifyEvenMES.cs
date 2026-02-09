using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class NotifyEvenMES
    {
        private List<IObserverMES> observers = new List<IObserverMES>();
        public List<IObserverMES> Observers => observers;
        public void Attach(IObserverMES observer)
        {
            observers.Add(observer);
        }

        public void Detach(IObserverMES observer)
        {
            observers.Remove(observer);
        }
        public void NotifyMESConnect(bool MesConnected)
        {
            foreach (IObserverMES ob in observers)
            {
                ob.CheckConnectionMES(MesConnected);
            }
        }

        public void NotifyMESResult(string MesData)
        {
            foreach (IObserverMES ob in observers)
            {
                ob.FollowingDataMES(MesData);
            }
        }
        public void NotifyToUI(string notify)
        {
            foreach (IObserverMES ob in observers)
            {
                ob.UpdateNotifyToUIMES(notify);
            }
        }
        public void GetInformationFromClientConnect(string clientIP, int clientPort)
        {
            foreach (IObserverMES ob in observers)
            {
                ob.GetInformationFromClientConnectMES(clientIP, clientPort);
            }
        }
    }
}
