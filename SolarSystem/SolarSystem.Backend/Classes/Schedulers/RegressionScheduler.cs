using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend.Classes.Simulation;
using System.IO;

namespace SolarSystem.Backend.Classes.Schedulers
{

    //Find ud af hvorfor gennemsnit er lavt, men per ordre er høj
    //Skriv den hurtigste og langsomste ordre ud, fordi vi vil se hvad der sænker den
    //Udksriv hvor mange der var 0-1 min, 1-5min osv
    //



    class RegressionScheduler : Scheduler
    {
        enum TPRProps
        {
            AreasToVisit,
            DifferentLines,
            QuantityPerLine,
            Bias,
            Fill
        }


        private Handler _handler;
        private int _maxAmountOfLines = 60;
        private int _MaxQuantityPerLine = 900;
        private SimulationInformation _simInfo;
        private List<Tuple<double, double>> _tprProperties;
        private Dictionary<TPRProps, double> TPRPropertyWeigths;
        private double _learningRate = 0.00001f;
        private Dictionary<int, Tuple<double, double>> sentOrders;
        private double Bias = 5;
        int[] timings = new int[3];

        
        private static StreamWriter graphFile = new StreamWriter(Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString())
                                           .ToString()) + "/SolarSystem.Backend/SolarData/Graph.csv");



        public RegressionScheduler(OrderGenerator orderGenerator, Handler handler, double poolMoverTime, SimulationInformation simInfo) : base(orderGenerator, handler, poolMoverTime)
        {
            _handler = handler;
            _simInfo = simInfo;
            double _sumOfAllErrors = 0;
            sentOrders = new Dictionary<int, Tuple<double, double>>();

            graphFile.AutoFlush = true;
            Random rng = new Random();

            double averageError = 0;
            int count = 0;
            int printCounter = 0;

            _handler.OnOrderBoxFinished += delegate (OrderBox box)
            {
                sentOrders[box.Order.OrderId] = new Tuple<double, double>(sentOrders[box.Order.OrderId].Item1, (double)(TimeKeeper.CurrentDateTime - box.Order.OrderTime).TotalSeconds);
                double error = sentOrders[box.Order.OrderId].Item2 - sentOrders[box.Order.OrderId].Item1;
                count++;
                averageError += error;

                if (Math.Abs(error) < 1)
                    timings[0]++;
                else if (Math.Abs(error) < 3)
                    timings[1]++;
                else
                    timings[2]++;


                Learn(error, box.Order);

                if (printCounter++ > 100)
                {

                    Console.WriteLine($"Our guess: {sentOrders[box.Order.OrderId].Item1:000.000}, Actual time: {sentOrders[box.Order.OrderId].Item2:000.000}," +
                                      $" Delta: {error:000.000}");
                    TPRPropertyWeigths[(TPRProps)rng.Next(0, 3)] += error * _learningRate;
                    SaveToLogFile(TPRPropertyWeigths, _sumOfAllErrors / count);
                    printCounter = 0;
                }

                _sumOfAllErrors += Math.Abs(error);
            };


            //Fetch the weights from last run, stored in the Weights.txt file
            TPRPropertyWeigths = UpdateWeightsFromFile();

            TimeKeeper.SimulationFinished += delegate
            {
                SaveToLogFile(TPRPropertyWeigths, _sumOfAllErrors / count);
                PrintWeightsToConsole(TPRPropertyWeigths, _sumOfAllErrors);
                SaveWeightsToFile(TPRPropertyWeigths);

                
                //Console.ReadKey();

                // create new process
                System.Diagnostics.Process NewInstance = new System.Diagnostics.Process();
                // link it to the application
                NewInstance.StartInfo.FileName = System.Threading.Thread.GetDomain().BaseDirectory + System.Threading.Thread.GetDomain().FriendlyName;
                // start new instance
                NewInstance.Start();
                // quit current instance
                System.Environment.Exit(0);

            };

        }


        void Learn(double error, Order order)
        {
            double sumOfAllWeights = TPRPropertyWeigths.Values.Sum();
            double sumOfAllResults = 0;

            sumOfAllResults += TPRPropertyWeigths[TPRProps.AreasToVisit] * order.Areas.Count;
            sumOfAllResults += TPRPropertyWeigths[TPRProps.DifferentLines] * order.Lines.Count;
            sumOfAllResults += TPRPropertyWeigths[TPRProps.QuantityPerLine] * order.Lines.Sum(l => l.Quantity);
            sumOfAllResults += TPRPropertyWeigths[TPRProps.Fill] * FillResult(order);
            sumOfAllResults += TPRPropertyWeigths[TPRProps.Bias] * Bias;



            TPRPropertyWeigths[TPRProps.AreasToVisit] += ((TPRPropertyWeigths[TPRProps.AreasToVisit] * order.Areas.Count) / sumOfAllResults) * error * _learningRate;
            TPRPropertyWeigths[TPRProps.DifferentLines] += ((TPRPropertyWeigths[TPRProps.DifferentLines] * order.Lines.Count) / sumOfAllResults) * error * _learningRate;
            TPRPropertyWeigths[TPRProps.QuantityPerLine]  += ((TPRPropertyWeigths[TPRProps.QuantityPerLine] * order.Lines.Sum(l => l.Quantity)) / sumOfAllResults) * error * _learningRate;
            TPRPropertyWeigths[TPRProps.Fill] += ((TPRPropertyWeigths[TPRProps.Fill] * FillResult(order)) / sumOfAllResults) * error * _learningRate;
            TPRPropertyWeigths[TPRProps.Bias] += ((TPRPropertyWeigths[TPRProps.Bias] * Bias) / sumOfAllResults) * error * _learningRate;



            //for (int i = 0; i < TPRPropertyWeigths.Count; i++)
            //{
            //    TPRPropertyWeigths[(TPRProps)i] +=
            //        ((TPRPropertyWeigths[(TPRProps)i] * 100) / sumOfAllWeights) * error * _learningRate;
            //}


        }


        void SaveToLogFile(Dictionary<TPRProps, double> weights, double ErrorValue)
        {

            using (StreamWriter sw = File.AppendText(Directory.GetParent(Directory
                                                         .GetParent(Directory
                                                             .GetParent(Environment.CurrentDirectory).ToString())
                                                         .ToString()) + "/SolarSystem.Backend/SolarData/Log.csv"))
            {
                sw.WriteLine(weights[TPRProps.AreasToVisit] + ";" +
                             weights[TPRProps.DifferentLines] + ";" +
                             weights[TPRProps.QuantityPerLine] + ";" +
                             weights[TPRProps.Fill] + ";" +
                             weights[TPRProps.Bias] + ";" +
                             ErrorValue + ";" +
                             timings[0] + ";" +
                             timings[1] + ";" +
                             timings[2]);
            }
        }


        void PrintWeightsToConsole(Dictionary<TPRProps, double> weights, double sumOfAllErrors)
        {
            Console.WriteLine($"AreasToVisit: {weights[TPRProps.AreasToVisit]}\r\n" +
                              $"DifferentLines: {weights[TPRProps.DifferentLines]}\r\n" +
                              $"Quantity: {weights[TPRProps.QuantityPerLine]}\r\n" +
                              $"Fill: {weights[TPRProps.Fill]}/r/n" +
                              $"Bias: {TPRProps.Bias} /r/n" +
                              $"Sum Of Total Errors: {sumOfAllErrors}\r\n" +
                              "---------------------------------------------------------------------------\r\n");

        }

        void SaveWeightsToFile(Dictionary<TPRProps, double> weights)
        {
            StreamWriter weightsFileWriter = new StreamWriter(Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString())
                                                                  .ToString()) + "/SolarSystem.Backend/SolarData/Weights.txt");


            weightsFileWriter.WriteLine($"{weights[TPRProps.AreasToVisit]};" +
                                        $"{weights[TPRProps.DifferentLines]};" +
                                        $"{weights[TPRProps.QuantityPerLine]};" +
                                        $"{weights[TPRProps.Fill]};" +
                                        $"{weights[TPRProps.Bias]}");


            weightsFileWriter.Close();


        }



        Dictionary<TPRProps, double> UpdateWeightsFromFile()
        {

            StreamReader weightsFileReader = new StreamReader(Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString())
                                                                  .ToString()) + "/SolarSystem.Backend/SolarData/Weights.txt");

            string weightLine = weightsFileReader.ReadLine();
            weightsFileReader.Close();
            TPRPropertyWeigths = new Dictionary<TPRProps, double>();
            TPRPropertyWeigths.Add(TPRProps.AreasToVisit, double.Parse(weightLine.Split(';')[0]));
            TPRPropertyWeigths.Add(TPRProps.DifferentLines, double.Parse(weightLine.Split(';')[1]));
            TPRPropertyWeigths.Add(TPRProps.QuantityPerLine, double.Parse(weightLine.Split(';')[2]));
            TPRPropertyWeigths.Add(TPRProps.Fill, double.Parse(weightLine.Split(';')[3]));
            TPRPropertyWeigths.Add(TPRProps.Bias, double.Parse(weightLine.Split(';')[4]));
            return TPRPropertyWeigths;
        }

        

        double GuessTimeForOrder(Order order)
        {
            /*return (float)order.Areas.Count / _handler.Areas.Count * TPRPropertyWeigths[TPRProps.AreasToVisit] +
                    (float)order.Lines.Distinct().Count() / _maxAmountOfLines * TPRPropertyWeigths[TPRProps.DifferentLines] +
                    (float)order.Lines.Sum(l => l.Quantity) / _MaxQuantityPerLine * TPRPropertyWeigths[TPRProps.QuantityPerLine];
                    */

           // return Bias * TPRPropertyWeigths[TPRProps.Bias] + FillResult(order) * TPRPropertyWeigths[TPRProps.Fill];

            return Bias * TPRPropertyWeigths[TPRProps.Bias] + order.Lines.Count * TPRPropertyWeigths[TPRProps.DifferentLines] +
                   order.Lines.Sum(o => o.Quantity) * TPRPropertyWeigths[TPRProps.QuantityPerLine] + FillResult(order) * TPRPropertyWeigths[TPRProps.Fill];


            //for hver area vi skal i, hvor meget plads er der ?

            /*
             * AreaFill [25,   20,   10,   17,   19]
             * Lines    [3 ,   10,   0 ,   3,    1 ]
             * Quantity [15,   10,   0 ,   20,   5 ]
             * product  [1125, 2000 ,0,    1020, 95] 
             * Weight   [2.5,  2.5,  2.5,  2.5, 2.5]
             * R        [2812, 5000, 0,   2550, 237]
             * b * w0 + Lines * w1 + Quantity * w2 + R * w3
            */




        }


        double FillResult(Order order)
        {
            double[] _fillresult = new double [5];
            for (int i = 0; i < _simInfo.GetState().Length-1; i++)
            {
                _fillresult[i] = _simInfo.GetState()[i] *
                    order.Lines.Where(l => l.Article.AreaCode == ((AreaCode) i)).Count()/* *
                    order.Lines.Where(l => l.Article.AreaCode == ((AreaCode) i)).Sum(Q => Q.Quantity)*/;
            }

            return _fillresult.Sum();
        }




        protected override Order ChooseNextOrder()
        {
            int chosenOrder = ActualOrderPool.OrderBy(o => o.OrderTime).First().OrderId;
           
            sentOrders.Add(chosenOrder, 
                new Tuple<double, double>(GuessTimeForOrder(ActualOrderPool.OrderBy(o => o.OrderTime).First()), 0));
            return ActualOrderPool.OrderBy(o => o.OrderTime).First();
        }
    }
}
