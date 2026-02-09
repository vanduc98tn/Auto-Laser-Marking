using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class NotifyEvenDataVision
    {
        private List<IObserverDataVision> observers = new List<IObserverDataVision>();
        public List<IObserverDataVision> Observers => observers;
        public void Attach(IObserverDataVision observer)
        {
            observers.Add(observer);
        }

        public void Detach(IObserverDataVision observer)
        {
            observers.Remove(observer);
        }
        public void NotifyConnectDataVision(bool MesConnected)
        {
            foreach (IObserverDataVision ob in observers)
            {
                ob.CheckConnectionDataVision(MesConnected);
            }
        }

        public void NotifyResultDataVision(DATACheck Data)
        {
            foreach (IObserverDataVision ob in observers)
            {
                ob.FollowingDataVision(Data);
            }
        }
        public void NotifyToUI(string notify)
        {
            foreach (IObserverDataVision ob in observers)
            {
                ob.UpdateNotifyToUI(notify);
            }
        }
        public void GetInformationFromClientConnect(string clientIP, int clientPort)
        {
            foreach (IObserverDataVision ob in observers)
            {
                ob.GetInformationFromClientConnect(clientIP, clientPort);
            }
        }
    }
}
