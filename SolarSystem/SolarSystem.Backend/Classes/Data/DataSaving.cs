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
        public static Dictionary<Area, double> areaStandartDeviation = new Dictionary<Area, double>();
        public static List<DataSavingOrder> orders = new List<DataSavingOrder>();

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
            using(StreamWriter writer = new StreamWriter(@"SimulationStatistics.txt", true))
            {
                writer.WriteLine("Largest completion time for an order: " + orders.Max(o => o.deltaFinishedTime));
                writer.Close();
            }            
        }

        private static void FindSmallestOrderTime()
        {
            using (StreamWriter writer = new StreamWriter(@"SimulationStatistics.txt", true))
            {
                writer.WriteLine("Smallest completion time for an order: " + orders.Min(o => o.deltaFinishedTime));
                writer.Close();
            }


        }

        private static void FindAverageOrderCompletionTime()
        {
            using (StreamWriter writer = new StreamWriter(@"SimulationStatistics.txt", true))
            {               
                writer.WriteLine("Average order completion time: " + orders.Average(o => o.deltaFinishedTime.TotalMinutes));
                writer.Close();
            }
        }

        private static void FindStandartDeviation()
        {
            double average;
            double sumOfSquaresOfDifference;
            double sd;

            average = orders.Average(v => v.deltaFinishedTime.TotalMinutes);
            sumOfSquaresOfDifference = orders.Select(val => (val.deltaFinishedTime.TotalMinutes - average) * (val.deltaFinishedTime.TotalMinutes - average)).Sum();
            sd = Math.Sqrt(sumOfSquaresOfDifference / orders.Count());

            using (StreamWriter writer = new StreamWriter(@"SimulationStatistics.txt", true))
            {
                writer.WriteLine("Standart deviation for orders over time: " + sd);
                writer.Close();
            }
        }

        public static void FindTimeOverLines()
        {

        }
    }
}
