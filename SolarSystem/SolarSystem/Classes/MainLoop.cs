using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices.ComTypes;
using C5;
using Microsoft.Win32;

namespace SolarSystem.Classes
{
    public class MainLoop
    {
        private Dictionary<int, IntervalHeap<OrderBoxProgress>> areaQueues;

        public MainLoop()
        {
            areaQueues = new Dictionary<int, IntervalHeap<OrderBoxProgress>>();
            areaQueues.Add(21, new IntervalHeap<OrderBoxProgress>());
            areaQueues.Add(25, new IntervalHeap<OrderBoxProgress>());
            areaQueues.Add(27, new IntervalHeap<OrderBoxProgress>());
            areaQueues.Add(28, new IntervalHeap<OrderBoxProgress>());
            areaQueues.Add(29, new IntervalHeap<OrderBoxProgress>());
            
            TimeKeeper.Tick += CheckAndSend;
        }

        public void AddOrderBox((OrderBox, int) orderBoxAndArea)
        {
            
            //areaQueues[orderBoxAndArea.Item2].Add();
        }

        private OrderBoxProgress PackToOrderboxProgress(OrderBox orderBox, int area)
        {
            // Estimate time based on Loop Flow and areas
            int timeToSpend = EstimateTime(area);
            // Create new OrderBoxProgress based on orderbox and time.
            //var orderBoxProgress = new OrderBoxProgress(orderBox, TimeKeeper.);
            
            // Return the new OrderBoxProgress.
            return null;
        }

        private int EstimateTime(int area)
        {
            return 10;
        }

        private void CheckAndSend()
        {
                
        }
    }
}