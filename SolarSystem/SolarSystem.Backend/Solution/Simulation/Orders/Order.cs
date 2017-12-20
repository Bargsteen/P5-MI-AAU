using System;
using System.Collections.Generic;

namespace SolarSystem.Backend.Solution.Simulation.Orders
{
    public class Order
    {
        public int OrderId { get; }
        public DateTime OrderTime { get; set; }
        public List<Line> Lines { get;}
        public Dictionary<AreaCode, bool> Areas { get; set; }
        public Dictionary<AreaCode, TimeInOut> AreaTimeInOutLog { get; set; }
        
        public DateTime StartPackingTime { get; set; }
        public int EstimatedPackingTimeInSeconds { get; set; }
        public Dictionary<AreaCode, decimal> EstimatedAreaFill;
        
        public double OrderFitness { get; set; }

        public Order(int orderId, DateTime orderTime, List<Line> lines)
        {
            OrderId = orderId;
            OrderTime = orderTime;
            Lines = lines ?? throw new ArgumentNullException(nameof(lines));
            AreaTimeInOutLog = new Dictionary<AreaCode, TimeInOut>();
            EstimatedAreaFill = new Dictionary<AreaCode, decimal>();
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
