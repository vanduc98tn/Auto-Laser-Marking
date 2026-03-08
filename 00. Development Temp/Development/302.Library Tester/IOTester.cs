using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Development
{
    public interface IObTester
    {
        void FollowDataResultTester(int ResultCH1, int ResultCH2, int ResultCH3, int ResultCH4, int ResultCH5, int ResultCH6
                                     , int ResultCH7, int ResultCH8, int ResultCH9, int ResultCH10, int ResultCH11, int ResultCH12);
        void FollowDataTester(string Result);
    }
}
