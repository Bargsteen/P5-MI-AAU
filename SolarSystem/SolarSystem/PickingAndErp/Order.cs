using System;
using System.Collections.Generic;

namespace SolarSystem.PickingAndErp
{
    public class Order
    {
        public List<Line> LineList { get; private set; }
        public int OrderNumber { get; private set; }
        public DateTime OrderTime { get; set; }

        public Order(int orderNumber, List<Line> lineList)
        {
            this.OrderNumber = orderNumber;
            this.LineList = lineList;
        }

        public Order(int orderNumber, Line line)
        {
            this.OrderNumber = orderNumber;
            this.LineList.Add(line);
        }


        public override string ToString()
        {
            string returnString = "";
            foreach (var line in LineList)
            {
                returnString += line;
            }
            return "OrderNumber: " + OrderNumber + returnString;
        }
    }
}
