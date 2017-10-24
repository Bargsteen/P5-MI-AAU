using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SolarSystem.Classes;
using SolarSystem.Simulation;
using SolarSystem.Simulation.Stations;


namespace SolarSystem
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Station Area27 = new Station(Area.Area27, ImmutableArray<ItemType>.Empty, 10, 10);
            Station Area25 = new Station(Area.Area25, ImmutableArray<ItemType>.Empty, 10 , 10);
            Station Area27 = new Station(Area.Area27, ImmutableArray<ItemType>.Empty, 10, 10);
            Station Area27 = new Station(Area.Area27, ImmutableArray<ItemType>.Empty, 10, 10);
            Station Area27 = new Station(Area.Area27, ImmutableArray<ItemType>.Empty, 10, 10);
            ISim sim = new Sim(3);
            sim.Run(50);
            
            
            bool run = true;
            Random RNG = new Random();

            List<Classes.Order> orders = new List<Classes.Order>();
            for (char c = 'A'; c < 'F'; c++)
            {
                orders.Add(new Classes.Order(c.ToString(), RNG.Next(1000), DateTime.Now));
            }


            while (run)
            {
                Console.WriteLine("How To Sort? \"FIFO\" or \"LIFO\"?");
                string input = Console.ReadLine();


                switch (input)
                {
                    case "quit":

                        run = false;

                    break;


                    case "FIFO":

                        foreach(Classes.Order o in orders)
                        {
                            Console.WriteLine(o.ToString());
                        }
                        Console.WriteLine("FIFO ORDER IS:");
                        Console.WriteLine(Classes.OrderBoxPicker.GetNextOrder(Classes.OrderBoxPicker.PickingOrder.FirstInFirstOut, orders));

                        break;



                    case "LIFO":

                        foreach (Classes.Order o in orders)
                        {
                            Console.WriteLine(o.ToString());
                        }
                        Console.WriteLine("LIFO ORDER IS:");
                        Console.WriteLine(Classes.OrderBoxPicker.GetNextOrder(Classes.OrderBoxPicker.PickingOrder.LastInFirstOut, orders));

                        break;
                        
                     default:
                         throw new ArgumentOutOfRangeException();
                }

            }

        }

        public static int SolarAdd(int x, int y)
        {
            return x + y;
        }


    }
}