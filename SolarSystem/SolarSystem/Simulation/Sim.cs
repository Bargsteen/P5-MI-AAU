using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SolarSystem.Classes;


namespace SolarSystem.Simulation
{
    public class Sim : ISim
    {
        private uint TimeSpentRunning { get; set; }
        private List<Order> FinishedOrders { get; set; }
        private List<OrderBox> BoxesInSystem { get; set; }
        private uint MaxConcurrentBoxes { get; }
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
                Update();
                Console.WriteLine(GetSimulationState());
            }
        }

        private void Update()
        {
            Thread.Sleep(1);
            TimeSpentRunning += 1;
            if (BoxesInSystem.Count < MaxConcurrentBoxes)
            {
                BoxesInSystem.Add(new OrderBox(GetNextOrder()));
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

        private Order GetNextOrder()
        {

            string name = ((char) ('A' + Rand.Next(0, 26))).ToString();
            int timeToFinish = Rand.Next(10, 40);

            return new Order(name, timeToFinish);
        }

        private string GetSimulationState()
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