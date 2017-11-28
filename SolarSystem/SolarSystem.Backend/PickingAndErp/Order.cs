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
            OrderNumber = orderNumber;
            LineList = lineList;
        }

        public Order(int orderNumber, Line line)
        {
            OrderNumber = orderNumber;
            LineList.Add(line);
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

        public Backend.Classes.Order ToSimOrder()
        {
            List<Backend.Classes.Line> simlinelist = new List<Backend.Classes.Line>();

            foreach (Line line in LineList)
            {
                simlinelist.Add(line.ToSimLine());
            }

            return new Backend.Classes.Order(this.OrderNumber, this.OrderTime, simlinelist);

        }
    }
}
