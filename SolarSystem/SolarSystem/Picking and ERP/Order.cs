using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Picking_and_ERP
{
    [Serializable]
    public class Order
    {
        public List<Line> lineList { get; private set; }
        public int orderNumber { get; private set; }
        
        public Order(int orderNumber, List<Line> lineList)
        {
            this.orderNumber = orderNumber;
            this.lineList = lineList;
        }

        public Order(int orderNumber, Line line)
        {
            this.orderNumber = orderNumber;
            this.lineList.Add(line);
        }


        public override string ToString()
        {
            string returnString = "";
            foreach (var line in lineList)
            {
                returnString += line;
            }
            return "OrderNumber: " + orderNumber + returnString;
        }
    }
}
