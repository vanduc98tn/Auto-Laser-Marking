using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class NotifyEvenLaser
    {
        private List<IObLaser> observers = new List<IObLaser>();
        public List<IObLaser> Observers => observers;
        public void Attach(IObLaser observer)
        {
            observers.Add(observer);
        }
        public void Detach(IObLaser observer)
        {
            observers.Remove(observer);
        }
        public void NotifyResultLaser(int ResultCH1,int ResultCH2, int ResultCH3, int ResultCH4, int ResultCH5, int ResultCH6, int ResultCH7, int ResultCH8, int ResultCH9, int ResultCH10, int ResultCH11, int ResultCH12)

        {
            foreach (IObLaser ob in observers)
            {
                ob.FollowDataResultLaser(ResultCH1, ResultCH2, ResultCH3, ResultCH4, ResultCH5, ResultCH6, ResultCH7, ResultCH8, ResultCH9, ResultCH10, ResultCH11, ResultCH12);
            }
        }
        public void NotifyDataLaser(string Messenger)
        {
            foreach (IObLaser ob in observers)
            {
                ob.FollowDataLaser(Messenger);
            }
        }
    }
}
