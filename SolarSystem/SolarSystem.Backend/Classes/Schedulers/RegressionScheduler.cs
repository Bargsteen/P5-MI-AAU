using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend.Classes.Simulation;
using System.IO;

namespace SolarSystem.Backend.Classes.Schedulers
{
    class RegressionScheduler : Scheduler
    {
        enum TPRProps
        {
            AreasToVisit,
            DifferentLines,
            QuantityPerLine
        }

        private Handler _handler;
        private List<Tuple<float, float>> _tprProperties;
        private Dictionary<TPRProps, float> TPRPropertyWeigths;
        private float _learningRate = 0.00000001f;
        private Dictionary<int, Tuple<float, float>> sentOrders;

        
        private static StreamWriter graphFile = new StreamWriter(Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString())
                                           .ToString()) + "/SolarSystem.Backend/SolarData/Graph.csv");



        public RegressionScheduler(OrderGenerator orderGenerator, Handler handler, double poolMoverTime) : base(orderGenerator, handler, poolMoverTime)
        {

            _handler = handler;

            float _sumOfAllErrors = 0;
            sentOrders = new Dictionary<int, Tuple<float, float>>();

            graphFile.AutoFlush = true;
            Random rng = new Random();

            _handler.OnOrderBoxFinished += delegate (OrderBox box)
            {
                sentOrders[box.Order.OrderId] = new Tuple<float, float>(sentOrders[box.Order.OrderId].Item1, (float)(TimeKeeper.CurrentDateTime - box.Order.OrderTime).TotalMinutes);

                float error = sentOrders[box.Order.OrderId].Item2 - sentOrders[box.Order.OrderId].Item1;
                //Console.WriteLine($"Our guess: {sentOrders[box.Order.OrderId].Item1:000.000}, Actual time: {sentOrders[box.Order.OrderId].Item2:000.000}," +
                //                  $" Delta: {error:000.000}");
                _sumOfAllErrors += error;
                TPRPropertyWeigths[(TPRProps)rng.Next(0, 3)] += error * _learningRate;
            };


            //Fetch the weights from last run, stored in the Weights.txt file
            TPRPropertyWeigths = UpdateWeightsFromFile();

            TimeKeeper.SimulationFinished += delegate
            {
                SaveToLogFile(TPRPropertyWeigths, _sumOfAllErrors);
                PrintWeightsToConsole(TPRPropertyWeigths, _sumOfAllErrors);
                SaveWeightsToFile(TPRPropertyWeigths);

                Console.ReadKey();

            };

        }

        



        void SaveToLogFile(Dictionary<TPRProps, float> weights, float sumOfAllErrors)
        {

            using (StreamWriter sw = File.AppendText(Directory.GetParent(Directory
                                                         .GetParent(Directory
                                                             .GetParent(Environment.CurrentDirectory).ToString())
                                                         .ToString()) + "/SolarSystem.Backend/SolarData/Log.txt"))
            {
                sw.WriteLine($"AreasToVisit: {weights[TPRProps.AreasToVisit]}\r\n" +
                             $"DifferentLines: {weights[TPRProps.DifferentLines]}\r\n" +
                             $"Quantity: {weights[TPRProps.QuantityPerLine]}\r\n" +
                             $"Sum Of Total Errors: {sumOfAllErrors}\r\n" +
                             "---------------------------------------------------------------------------\r\n");
            }
        }


        void PrintWeightsToConsole(Dictionary<TPRProps, float> weights, float sumOfAllErrors)
        {
            Console.WriteLine($"AreasToVisit: {weights[TPRProps.AreasToVisit]}\r\n" +
                              $"DifferentLines: {weights[TPRProps.DifferentLines]}\r\n" +
                              $"Quantity: {weights[TPRProps.QuantityPerLine]}\r\n" +
                              $"Sum Of Total Errors: {sumOfAllErrors}\r\n" +
                              "---------------------------------------------------------------------------\r\n");

        }

        void SaveWeightsToFile(Dictionary<TPRProps, float> weights)
        {
            StreamWriter weightsFileWriter = new StreamWriter(Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString())
                                                                  .ToString()) + "/SolarSystem.Backend/SolarData/Weights.txt");


            weightsFileWriter.WriteLine($"{weights[TPRProps.AreasToVisit]};" +
                                        $"{weights[TPRProps.DifferentLines]};" +
                                        $"{weights[TPRProps.QuantityPerLine]}");
            weightsFileWriter.Close();


        }



        Dictionary<TPRProps, float> UpdateWeightsFromFile()
        {

            StreamReader weightsFileReader = new StreamReader(Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString())
                                                                  .ToString()) + "/SolarSystem.Backend/SolarData/Weights.txt");

            string weightLine = weightsFileReader.ReadLine();
            weightsFileReader.Close();
            TPRPropertyWeigths = new Dictionary<TPRProps, float>();
            TPRPropertyWeigths.Add(TPRProps.AreasToVisit, float.Parse(weightLine.Split(';')[0]));
            TPRPropertyWeigths.Add(TPRProps.DifferentLines, float.Parse(weightLine.Split(';')[1]));
            TPRPropertyWeigths.Add(TPRProps.QuantityPerLine, float.Parse(weightLine.Split(';')[2]));
            return TPRPropertyWeigths;
        }

        

        float GuessTimeForOrder(Order order)
        {
            return order.Areas.Count * TPRPropertyWeigths[TPRProps.AreasToVisit] +
                    order.Lines.Distinct().Count() * TPRPropertyWeigths[TPRProps.DifferentLines] +
                    order.Lines.Sum(l => l.Quantity) * TPRPropertyWeigths[TPRProps.QuantityPerLine];
            
        }

        
        
        protected override Order ChooseNextOrder()
        {
            if(sentOrders.Count == 5287)
                throw new Exception();
            int chosenOrder = ActualOrderPool.OrderBy(o => o.OrderTime).First().OrderId;
            int a;
            if (chosenOrder == 150350 || ActualOrderPool.Any(o => o.OrderId == 150350))
                a = 2;
            sentOrders.Add(chosenOrder, 
                new Tuple<float, float>(GuessTimeForOrder(ActualOrderPool.OrderBy(o => o.OrderTime).First()), 0));
            return ActualOrderPool.OrderBy(o => o.OrderTime).First();
        }
    }
}
