using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public interface IObserverDataVision
    {
        void CheckConnectionDataVision(bool connected);
        void FollowingDataVision(DATACheck Result);
        void GetInformationFromClientConnect(string clientIP, int clientPort);
        void UpdateNotifyToUI(string Notify);
    }
}
