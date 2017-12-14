using System;
using System.Collections;
using System.Collections.Generic;
using Accord.Math;

namespace SolarSystem.Backend.Classes.Simulation
{
    public class Order
    {
        public int OrderId { get; }
        public DateTime OrderTime { get; set; }
        public List<Line> Lines { get;}
        public Dictionary<AreaCode, bool> Areas { get; set; }
        public Dictionary<AreaCode, TimeInOut> AreaTimeInOutLog { get; set; }

        public Order(int orderId, DateTime orderTime, List<Line> lines)
        {
            OrderId = orderId;
            OrderTime = orderTime;
            Lines = lines ?? throw new ArgumentNullException(nameof(lines));
            AreaTimeInOutLog = new Dictionary<AreaCode, TimeInOut>();
        }

        public override string ToString()
        {
            return $"{OrderId}";
        }

    }

    public class TimeInOut
    {
        public TimeInOut(DateTime inTime)
        {
            InTime = inTime;
        }

        public DateTime InTime { get; set; }
        public DateTime OutTime { get; set; }
    }
    
}
