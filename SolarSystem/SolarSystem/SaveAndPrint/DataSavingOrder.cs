using System;
using System.Collections.Generic;
using SolarSystem.Backend.Classes.Simulation.Orders;

namespace SolarSystem.SaveAndPrint
{
    public class DataSavingOrder
    {
        public DataSavingOrder(Order order)
        {
            Order = order;
        }

        public Order Order { get; set; }
        public List<DataSavingLine> Lines { get; }
        public TimeSpan DeltaFinishedTime;
        public DateTime FinishedOrderTime { get => _finishedOrderTime;
            set { DeltaFinishedTime =  value - Order.OrderTime; _finishedOrderTime = value;}}
        private DateTime _finishedOrderTime;
    }
}
