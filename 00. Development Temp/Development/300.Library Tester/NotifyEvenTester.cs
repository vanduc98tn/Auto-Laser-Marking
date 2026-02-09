using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public class NotifyEvenTester
    {
        private List<IObTester> observers = new List<IObTester>();
        public List<IObTester> Observers => observers;
        public void Attach(IObTester observer)
        {
            observers.Add(observer);
        }
        public void Detach(IObTester observer)
        {
            observers.Remove(observer);
        }
        public void NotifyResultTester(int ResultCH1,int ResultCH2, int ResultCH3, int ResultCH4, int ResultCH5, int ResultCH6, int ResultCH7, int ResultCH8, int ResultCH9, int ResultCH10, int ResultCH11, int ResultCH12)

        {
            foreach (IObTester ob in observers)
            {
                ob.FollowDataResultTester(ResultCH1, ResultCH2, ResultCH3, ResultCH4, ResultCH5, ResultCH6, ResultCH7, ResultCH8, ResultCH9, ResultCH10, ResultCH11, ResultCH12);
            }
        }
        public void NotifyDataTester(string Messenger)
        {
            foreach (IObTester ob in observers)
            {
                ob.FollowDataTester(Messenger);
            }
        }
    }
}
