using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSystem.Backend.Classes.Simulation;
using System.IO;

namespace SolarSystem.Backend.Classes.Data
{
    public class DataSavingOrder
    {
        public DataSavingOrder(Order order)
        {
            Order = order;
        }

        public Order Order { get; set; }
        public List<DataSavingLine> lines { get; }
        public TimeSpan deltaFinishedTime;
        public DateTime finishedOrderTime { get { return finishedOrderTime; } set { deltaFinishedTime = value -Order.OrderTime; } }

        
    }
}
