using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolarSystem.Backend.Classes.Simulation;
using SolarSystem.Backend.Classes.Simulation.WareHouse;

namespace SolarSystem.SaveAndPrint
{
    public class DataSaver
    {

        public static Dictionary<Area, double> AreaStandartDeviation = new Dictionary<Area, double>();
        public static List<DataSavingOrder> Orders = new List<DataSavingOrder>();
        
        static readonly string FilePath = Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString())
                                     .ToString()) + "/SolarSystem.Backend/SolarData/DataSaver.txt";
        
        public DataSaver(Runner runner)
        {
            runner.Handler.OnOrderBoxFinished += orderBox => Orders.First(o => o.Order.OrderId == orderBox.Order.OrderId).FinishedOrderTime = TimeKeeper.CurrentDateTime;
            TimeKeeper.SimulationFinished += SaveData;
        }
        

        public static void SaveData()
        {
            FindLongestOrderTime();
            FindSmallestOrderTime();
            FindAverageOrderCompletionTime();
            FindStandardDeviation();
        }

        //Method for saving data to datafiles. This method will also calculate standart deviation based on input and previous input
        private static void FindLongestOrderTime()
        {
            using(StreamWriter writer = new StreamWriter(FilePath, false))
            {
                writer.WriteLine("Largest completion time for an order: " + Math.Abs(Orders.Max(o => o.DeltaFinishedTime).TotalMinutes) + " minutes " + Math.Abs(Orders.Max(o => o.DeltaFinishedTime).Seconds) + " seconds");
                writer.Close();
            }            
        }

        private static void FindSmallestOrderTime()
        {
            using (StreamWriter writer = new StreamWriter(FilePath, true))
            {
                //bugged, fix
                if (Math.Abs(Math.Abs(Orders.Min(o => o.DeltaFinishedTime).TotalMilliseconds)) < 0.01)
                {
                    writer.WriteLine("Smallest ordernumber: " + Orders.Find(x => x.DeltaFinishedTime.Milliseconds == 0).Order.OrderId + "\n\n");
                }
                writer.WriteLine("Smallest completion time for an order: " + Math.Abs(Orders.Min(o => o.DeltaFinishedTime).TotalMinutes) + " minutes " + Math.Abs(Orders.Min(o => o.DeltaFinishedTime).Seconds) + " seconds " + Math.Abs(Orders.Min(o => o.DeltaFinishedTime).Milliseconds));
                writer.Close();
            }
        }

        private static void FindAverageOrderCompletionTime()
        {
            using (StreamWriter writer = new StreamWriter(FilePath, true))
            {               
                writer.WriteLine("Average order completion time: " + Math.Abs(Orders.Average(o => o.DeltaFinishedTime.TotalMinutes)) + " minutes");
                writer.Close();
            }
        }

        private static void FindStandardDeviation()
        {
            double average;
            double sumOfSquaresOfDifference;
            double sd;

            average = Orders.Average(v => v.DeltaFinishedTime.TotalMinutes);
            sumOfSquaresOfDifference = Orders.Select(val => (val.DeltaFinishedTime.TotalMinutes - average) * (val.DeltaFinishedTime.TotalMinutes - average)).Sum();
            sd = Math.Abs(Math.Sqrt(sumOfSquaresOfDifference / Orders.Count));

            using (StreamWriter writer = new StreamWriter(FilePath, true))
            {
                writer.WriteLine("Standard deviation for orders over time: " + sd);
                writer.Close();
            }
        }
    }
}
