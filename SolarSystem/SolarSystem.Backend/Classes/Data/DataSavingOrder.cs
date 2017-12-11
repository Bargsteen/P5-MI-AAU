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
            _order = order;
        }

        public Order _order { get; }
        public List<DataSavingLine> lines { get; }
        public DateTime finishedOrderTime { get; set; }

        
    }
}
