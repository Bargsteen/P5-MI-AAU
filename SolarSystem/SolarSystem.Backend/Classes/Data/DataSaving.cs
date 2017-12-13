using SolarSystem.Backend.Classes.Simulation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;



namespace SolarSystem.Backend.Classes.Data
{
    public static class DataSaving
    {
        public static Dictionary<Area, double> AreaStandartDeviation = new Dictionary<Area, double>();
        public static List<DataSavingOrder> Orders = new List<DataSavingOrder>();

        public static void SaveData()
        {
            FindLongestOrderTime();
            FindSmallestOrderTime();
            FindAverageOrderCompletionTime();
            FindStandartDeviation();
        }

        //Method for saving data to datafiles. This method will also calculate standart deviation based on input and previous input
        private static void FindLongestOrderTime()
        {
            using(StreamWriter writer = new StreamWriter(@"SimulationStatistics.txt", false))
            {
                writer.WriteLine("Largest completion time for an order: " + Math.Abs(Orders.Max(o => o.DeltaFinishedTime).Minutes) + " minutes " + Math.Abs(Orders.Max(o => o.DeltaFinishedTime).Seconds) + " seconds");
                writer.Close();
            }            
        }

        private static void FindSmallestOrderTime()
        {
            using (StreamWriter writer = new StreamWriter(@"SimulationStatistics.txt", true))
            {
                //bugged, fix
                if (Math.Abs(Orders.Min(o => o.DeltaFinishedTime).Milliseconds) == 0)
                {
                    writer.WriteLine(Orders.Find(x => x.DeltaFinishedTime.Milliseconds == 0).Order.OrderId + "\n\n");
                }
                writer.WriteLine("Smallest completion time for an order: " + Math.Abs(Orders.Min(o => o.DeltaFinishedTime).Minutes) + " minutes " + Math.Abs(Orders.Min(o => o.DeltaFinishedTime).Seconds) + " seconds " + Math.Abs(Orders.Min(o => o.DeltaFinishedTime).Milliseconds));
                writer.Close();
            }
        }

        private static void FindAverageOrderCompletionTime()
        {
            using (StreamWriter writer = new StreamWriter(@"SimulationStatistics.txt", true))
            {               
                writer.WriteLine("Average order completion time: " + Math.Abs(Orders.Average(o => o.DeltaFinishedTime.TotalMinutes)) + " Minutter");
                writer.Close();
            }
        }

        private static void FindStandartDeviation()
        {
            double average;
            double sumOfSquaresOfDifference;
            double sd;

            average = Orders.Average(v => v.DeltaFinishedTime.TotalMinutes);
            sumOfSquaresOfDifference = Orders.Select(val => (val.DeltaFinishedTime.TotalMinutes - average) * (val.DeltaFinishedTime.TotalMinutes - average)).Sum();
            sd = Math.Abs(Math.Sqrt(sumOfSquaresOfDifference / Orders.Count));

            using (StreamWriter writer = new StreamWriter(@"SimulationStatistics.txt", true))
            {
                writer.WriteLine("Standart deviation for orders over time: " + sd);
                writer.Close();
            }
        }
    }
}
