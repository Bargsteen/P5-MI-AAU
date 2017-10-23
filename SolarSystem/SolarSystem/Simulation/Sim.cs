using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SolarSystem.Classes;


namespace SolarSystem.Simulation
{
    public class Sim : ISim
    {
        public uint TimeSpentRunning { get; set; }
        public List<Order> FinishedOrders { get; set; }
        public List<OrderBox> BoxesInSystem { get; set; }
        public uint MaxConcurrentBoxes { get; private set; }
        private static readonly Random Rand = new Random();
        
        
        public Sim(uint maxConcurrentBoxes)
        {
            TimeSpentRunning = 0;
            FinishedOrders = new List<Order>();
            BoxesInSystem = new List<OrderBox>();
            MaxConcurrentBoxes = maxConcurrentBoxes;
        }

        
        public void Run(uint timeUnits)
        {
            Console.WriteLine($"----START SIMULATION----\nMax Concurrent Boxes: {MaxConcurrentBoxes}");
            for (int i = 0; i < timeUnits; i++)
            {
                _Update(_GetNextOrder);
                Console.WriteLine(_GetSimulationState());
            }
        }
        
        public void _Update(Func<Order> getNextOrder)
        {
            Thread.Sleep(1);
            TimeSpentRunning += 1;
            if (BoxesInSystem.Count < MaxConcurrentBoxes)
            {
                BoxesInSystem.Add(new OrderBox(getNextOrder.Invoke()));
            }
            
            List<OrderBox> boxesToBeDeleted = new List<OrderBox>();
            
            foreach (OrderBox orderBox in BoxesInSystem)
            {
                orderBox.TimeRemaining -= 1;
                
                if (orderBox.TimeRemaining == 0)
                {
                    FinishedOrders.Add(orderBox.Order);
                    boxesToBeDeleted.Add(orderBox);
                }
            }
            
            boxesToBeDeleted.ForEach(b => BoxesInSystem.Remove(b));
        }

        public Order _GetNextOrder()
        {

            string name = ((char) ('A' + Rand.Next(0, 26))).ToString();
            int timeToFinish = Rand.Next(10, 40);

            return new Order(name, timeToFinish, DateTime.Now);
        }

        

        public string _GetSimulationState()
        {
            string boxesInSystem = BoxesInSystem.Aggregate("", (current, orderBox) => current + orderBox.ToString());
            string finishedOrders = FinishedOrders.Aggregate("", (current, order) => current + order.ToString());
            
            return $"----Time: {TimeSpentRunning}-----\n"
                   + $"\nBoxes in System:\n"
                   + boxesInSystem
                   + "\nFinished Orders: \n"
                   + finishedOrders + "\n\n";
        }
    }
}