using System.Runtime.InteropServices.ComTypes;
using C5;
using Microsoft.Win32;

namespace SolarSystem.Classes
{
    public class MainLoop
    {
        private IntervalHeap<OrderBoxProgressAndArea> priorityQueue;

        public MainLoop()
        {
            priorityQueue = new IntervalHeap<OrderBoxProgressAndArea>();
            TimeKeeper.Tick += CheckAndSend;
        }

        public void AddOrderBox(OrderBoxProgressAndArea boxProgress) => priorityQueue.Add(boxProgress);

        private void CheckAndSend()
        {
            foreach (var orderBoxProgressAndArea in priorityQueue)
            {
                
            }
        }
    }
}