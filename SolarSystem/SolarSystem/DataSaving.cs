using SolarSystem.Backend.Classes.Simulation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SolarSystem
{
    public static class DataSaving
    {
        private static bool firstIteration = true;
        private static double average;
        private static double sumOfSquaresOfDifferences;
        private static double sd;
        public static Dictionary<Area, double> areaStandartDeviation = new Dictionary<Area, double>();
        public static List<DataSavingOrder> orders = new List<DataSavingOrder>();


        //Method for saving data to datafiles. This method will also calculate standart deviation based on input and previous input
        public static void SaveData(Dictionary<Area, List<Tuple<DateTime, int>>> _linesInArea)
        {
            foreach (var _a in _linesInArea.Keys)
            {
                //using (StreamWriter dataWriter = new StreamWriter(@"Data/" + _a.Key.ToString() + ".xml"))
                using (StreamWriter dataWriter = new StreamWriter("Data/" + _a + ".xml", firstIteration))
                {
                    _linesInArea[_a].ForEach(x => dataWriter.WriteLine(x.Item1 + ", " + x.Item2));
                    dataWriter.Close();
                }

                using (StreamWriter dataWriter = new StreamWriter("Data/StandartDeviation" + _a + ".txt", firstIteration))
                {
                    average = 0;
                    sumOfSquaresOfDifferences = 0;
                    if (!areaStandartDeviation.ContainsKey(_a))
                    {
                        average = _linesInArea[_a].Average(v => v.Item2);
                        sumOfSquaresOfDifferences = _linesInArea[_a].Select(val => (val.Item2 - average) * (val.Item2 - average)).Sum();
                        sd = Math.Sqrt(sumOfSquaresOfDifferences / _linesInArea[_a].Count());
                        areaStandartDeviation[_a] = sd;
                    }
                    else
                    {
                        average = _linesInArea[_a].Average(v => v.Item2);
                        sumOfSquaresOfDifferences = _linesInArea[_a].Select(val => (val.Item2 - average) * (val.Item2 - average)).Sum();
                        sd = Math.Sqrt(sumOfSquaresOfDifferences / _linesInArea[_a].Count());
                        areaStandartDeviation[_a] = ((areaStandartDeviation[_a] + sd) / 2);
                    }

                    dataWriter.WriteLine("This is the sd: " + areaStandartDeviation[_a]);
                    dataWriter.Close();
                }
            }
            firstIteration = false;
        }
    }
}
