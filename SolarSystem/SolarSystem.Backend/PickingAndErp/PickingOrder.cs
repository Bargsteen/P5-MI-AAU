using System;
using System.Collections.Generic;

namespace SolarSystem.Backend.PickingAndErp
{
    public class PickingOrder
    {
        public List<Line> LineList { get; private set; }
        public int OrderNumber { get; private set; }
        public DateTime OrderTime { get; set; }

        public PickingOrder(int orderNumber, List<Line> lineList)
        {
            OrderNumber = orderNumber;
            LineList = lineList;
        }

        public PickingOrder(int orderNumber, Line line)
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

        public Classes.Simulation.Order ToSimOrder()
        {
            List<Classes.Simulation.Line> simlinelist = new List<Classes.Simulation.Line>();

            foreach (Line line in LineList)
            {
                simlinelist.Add(line.ToSimLine());
            }

            return new Classes.Simulation.Order(OrderNumber, OrderTime, simlinelist);

        }
        
    }
}
