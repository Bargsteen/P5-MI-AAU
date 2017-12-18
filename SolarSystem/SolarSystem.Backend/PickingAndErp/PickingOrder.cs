﻿using System;
using System.Collections.Generic;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend.PickingAndErp
{
    public class PickingOrder
    {
        public List<Line> LineList { get; }
        public int OrderNumber { get; }
        public DateTime OrderTime { get; set; }

        public PickingOrder(int orderNumber, List<Line> lineList)
        {
            OrderNumber = orderNumber;
            LineList = lineList;
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

        public override bool Equals(object obj)
        {
            if (obj is PickingOrder pickingOrder)
            {
                return Equals(pickingOrder);
            }
            return false;
        }

        protected bool Equals(PickingOrder other)
        {
            return OrderNumber == other.OrderNumber;
        }

        public override int GetHashCode()
        {
            return OrderNumber;
        }

        public Order ToSimOrder()
        {
            List<Classes.Simulation.Line> simlinelist = new List<Classes.Simulation.Line>();

            foreach (Line line in LineList)
            {
                simlinelist.Add(line.ToSimLine());
            }
            return new Order(OrderNumber, OrderTime, simlinelist);
        }
    }
}
