using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Classes
{
    class Order
    {
        string _orderName;
        int _timeToFinish;

        public Order(string orderName, int timeToFinish)
        {
            _orderName = orderName;
            _timeToFinish = timeToFinish;
        }

        public string OrderName { get { return _orderName; } }
        public int TimeToFinish { get { return _timeToFinish; } }

        public override string ToString()
        {
            return String.Format(@"({0}, {1})", _orderName, _timeToFinish);
        }
    }
}
