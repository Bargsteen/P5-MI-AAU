﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using SolarSystem.Classes;
using SolarSystem.Simulation;


namespace SolarSystem
{
    public static class Program
    {
        private static void PrintList(List<int> list)
        {
            string p = "[ ";
            foreach (var i in list)
            {
                p += $"{i} ";
            }
            p += "]";
            Console.WriteLine(p);
        }
        
        public static void Main(string[] args)
        {
            var timeKeeper = new TimeKeeper(DateTime.Now);
            TimeKeeper.Tick += () => Console.WriteLine(timeKeeper.CurrentDateTime);
            timeKeeper.StartTicking(5);
            
            
            /*  Station area27 = new Station(Area.Area27, ImmutableArray<ItemType>.Empty, 10, 10);
              Station area25 = new Station(Area.Area25, ImmutableArray<ItemType>.Empty, 10 , 10);
              Station area27 = new Station(Area.Area27, ImmutableArray<ItemType>.Empty, 10, 10);
              Station area27 = new Station(Area.Area27, ImmutableArray<ItemType>.Empty, 10, 10);
              Station area27 = new Station(Area.Area27, ImmutableArray<ItemType>.Empty, 10, 10);
              ISim sim = new Sim(3);
              sim.StartTicking(50);
              
              
              bool run = true;
              Random rng = new Random();
  
              List<Classes.Order> orders = new List<Classes.Order>();
              for (char c = 'A'; c < 'F'; c++)
              {
                  orders.Add(new Classes.Order(c.ToString(), rng.Next(1000), CurrentDateTime.Now));
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
                          Console.WriteLine(OrderBoxPicker.GetNextOrder(OrderBoxPicker.PickingOrder.FirstInFirstOut, orders));
  
                          break;
  
  
  
                      case "LIFO":
  
                          foreach (Classes.Order o in orders)
                          {
                              Console.WriteLine(o.ToString());
                          }
                          Console.WriteLine("LIFO ORDER IS:");
                          Console.WriteLine(OrderBoxPicker.GetNextOrder(OrderBoxPicker.PickingOrder.LastInFirstOut, orders));
  
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
  */

        }
    }
}