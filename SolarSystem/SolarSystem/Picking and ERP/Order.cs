using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Picking_and_ERP
{
    public class Order
    {
        public Order(int OrderNumber, Line line)
        {
            _ordernumber = OrderNumber;
            LineList.Add(line);
        }

        public List<Line> LineList = new List<Line>();
        public int _ordernumber;

        public override string ToString()
        {
            return "OrderNumber: " + _ordernumber + "\n";
        }
    }
}
