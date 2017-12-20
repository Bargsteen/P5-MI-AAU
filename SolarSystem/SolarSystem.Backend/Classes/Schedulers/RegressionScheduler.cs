using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend.Classes.Simulation;
using System.IO;
using Accord.Statistics.Kernels;
using SolarSystem.Backend.Classes.Simulation.Boxes;
using SolarSystem.Backend.Classes.Simulation.ConstantsAndEnums;
using SolarSystem.Backend.Classes.Simulation.Orders;
using SolarSystem.Backend.Classes.Simulation.WareHouse;

namespace SolarSystem.Backend.Classes.Schedulers
{
    class RegressionScheduler : Scheduler
    {
        enum TprProps
        {
            AreasToVisit,
            DifferentLines,
            QuantityPerLine,
            Bias,
            Fill
        }


        private readonly Handler _handler;
        private int _maxAmountOfLines = 60;
        private int _maxQuantityPerLine = 900;
        private readonly SimulationInformation _simInfo;
        private List<Tuple<double, double>> _tprProperties;
        private Dictionary<TprProps, double> _tprPropertyWeigths;
        private readonly double _learningRate = 0.0001f;
        private readonly Dictionary<int, Tuple<double, double>> _sentOrders;
        private readonly double _bias = 5;
        readonly int[] _timings = new int[3];

        public RegressionScheduler(OrderGenerator orderGenerator, Handler handler, double poolMoverTime, SimulationInformation simInfo) : base(orderGenerator, handler, poolMoverTime)
        {
            _handler = handler;
            _simInfo = simInfo;
            double sumOfAllErrors = 0;
            _sentOrders = new Dictionary<int, Tuple<double, double>>();

            double averageError = 0;
            int count = 0;
            int printCounter = 0;

            _handler.OnOrderBoxFinished += delegate (OrderBox box)
            {
                _sentOrders[box.Order.OrderId] = new Tuple<double, double>(_sentOrders[box.Order.OrderId].Item1, (double)(TimeKeeper.CurrentDateTime - box.Order.OrderTime).TotalSeconds);
                double error = _sentOrders[box.Order.OrderId].Item2 - _sentOrders[box.Order.OrderId].Item1;
                count++;
                averageError += error;

                if (Math.Abs(error) < 1)
                    _timings[0]++;
                else if (Math.Abs(error) < 3)
                    _timings[1]++;
                else
                    _timings[2]++;


                Learn(error, box.Order);

                if (printCounter++ > 100)
                {

                    Console.WriteLine($"Our guess: {_sentOrders[box.Order.OrderId].Item1:000.000}, Actual time: {_sentOrders[box.Order.OrderId].Item2:000.000}," +
                                      $" Delta: {error:000.000}");
                    SaveToLogFile(_tprPropertyWeigths, sumOfAllErrors / count);
                    printCounter = 0;
                }

                sumOfAllErrors += Math.Abs(error);
            };


            //Fetch the weights from last run, stored in the Weights.txt file
            _tprPropertyWeigths = UpdateWeightsFromFile();

            TimeKeeper.SimulationFinished += delegate
            {
                SaveToLogFile(_tprPropertyWeigths, sumOfAllErrors / count);
                PrintWeightsToConsole(_tprPropertyWeigths, sumOfAllErrors);
                SaveWeightsToFile(_tprPropertyWeigths);

                
                //Console.ReadKey();

                // create new process
                System.Diagnostics.Process newInstance = new System.Diagnostics.Process();
                // link it to the application
                newInstance.StartInfo.FileName = System.Threading.Thread.GetDomain().BaseDirectory + System.Threading.Thread.GetDomain().FriendlyName;
                // start new instance
                newInstance.Start();
                // quit current instance
                Environment.Exit(0);

            };

        }


        void Learn(double error, Order order)
        {
            double sumOfAllWeights = _tprPropertyWeigths.Values.Sum();
            //double sumOfAllResults = 0;

            //sumOfAllResults += TPRPropertyWeigths[TPRProps.AreasToVisit] * order.Areas.Count;
            //sumOfAllResults += TPRPropertyWeigths[TPRProps.DifferentLines] * order.Lines.Count;
            //sumOfAllResults += TPRPropertyWeigths[TPRProps.QuantityPerLine] * order.Lines.Sum(l => l.Quantity);
            //sumOfAllResults += TPRPropertyWeigths[TPRProps.Fill] * FillResult(order);
            //sumOfAllResults += TPRPropertyWeigths[TPRProps.Bias] * Bias;


            double[] input = new double[5];
            input[(int)TprProps.AreasToVisit] = order.Areas.Count * _tprPropertyWeigths[TprProps.AreasToVisit];
            input[(int)TprProps.Bias] = _bias * _tprPropertyWeigths[TprProps.Bias];
            input[(int)TprProps.DifferentLines] = order.Lines.Count * _tprPropertyWeigths[TprProps.DifferentLines];
            input[(int)TprProps.QuantityPerLine] = order.Lines.Sum(o => o.Quantity) * _tprPropertyWeigths[TprProps.QuantityPerLine];
            input[(int)TprProps.Fill] = ReLu(FillResult(order) * _tprPropertyWeigths[TprProps.Fill]);


            double[] output = new double[5];
            output = Softmax(input);


            _tprPropertyWeigths[TprProps.AreasToVisit] += output[(int)TprProps.AreasToVisit] * _tprPropertyWeigths[TprProps.AreasToVisit] * error * _learningRate;
            _tprPropertyWeigths[TprProps.DifferentLines] += output[(int)TprProps.DifferentLines] * _tprPropertyWeigths[TprProps.DifferentLines] * error * _learningRate;
            _tprPropertyWeigths[TprProps.QuantityPerLine]  += output[(int)TprProps.QuantityPerLine] * _tprPropertyWeigths[TprProps.QuantityPerLine] * error * _learningRate;
            _tprPropertyWeigths[TprProps.Fill] += output[(int)TprProps.Fill] * _tprPropertyWeigths[TprProps.Fill] * error * _learningRate;
            _tprPropertyWeigths[TprProps.Bias] += output[(int)TprProps.Bias] * _tprPropertyWeigths[TprProps.Bias] * error * _learningRate;
   

        }


        void SaveToLogFile(Dictionary<TprProps, double> weights, double errorValue)
        {

            using (StreamWriter sw = File.AppendText(Directory.GetParent(Directory
                                                         .GetParent(Directory
                                                             .GetParent(Environment.CurrentDirectory).ToString())
                                                         .ToString()) + "/SolarSystem.Backend/SolarData/Log.csv"))
            {
                sw.WriteLine(weights[TprProps.AreasToVisit] + ";" +
                             weights[TprProps.DifferentLines] + ";" +
                             weights[TprProps.QuantityPerLine] + ";" +
                             weights[TprProps.Fill] + ";" +
                             weights[TprProps.Bias] + ";" +
                             errorValue + ";" +
                             _timings[0] + ";" +
                             _timings[1] + ";" +
                             _timings[2]);
            }
        }


        void PrintWeightsToConsole(Dictionary<TprProps, double> weights, double sumOfAllErrors)
        {
            Console.WriteLine($"AreasToVisit: {weights[TprProps.AreasToVisit]}\r\n" +
                              $"DifferentLines: {weights[TprProps.DifferentLines]}\r\n" +
                              $"Quantity: {weights[TprProps.QuantityPerLine]}\r\n" +
                              $"Fill: {weights[TprProps.Fill]}/r/n" +
                              $"Bias: {TprProps.Bias} /r/n" +
                              $"Sum Of Total Errors: {sumOfAllErrors}\r\n" +
                              "---------------------------------------------------------------------------\r\n");

        }

        void SaveWeightsToFile(Dictionary<TprProps, double> weights)
        {
            StreamWriter weightsFileWriter = new StreamWriter(Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString())
                                                                  .ToString()) + "/SolarSystem.Backend/SolarData/Weights.txt");


            weightsFileWriter.WriteLine($"{weights[TprProps.AreasToVisit]};" +
                                        $"{weights[TprProps.DifferentLines]};" +
                                        $"{weights[TprProps.QuantityPerLine]};" +
                                        $"{weights[TprProps.Fill]};" +
                                        $"{weights[TprProps.Bias]}");


            weightsFileWriter.Close();


        }



        Dictionary<TprProps, double> UpdateWeightsFromFile()
        {

            StreamReader weightsFileReader = new StreamReader(Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString())
                                                                  .ToString()) + "/SolarSystem.Backend/SolarData/Weights.txt");

            string weightLine = weightsFileReader.ReadLine();
            weightsFileReader.Close();
            _tprPropertyWeigths = new Dictionary<TprProps, double>();
            _tprPropertyWeigths.Add(TprProps.AreasToVisit, double.Parse(weightLine.Split(';')[0]));
            _tprPropertyWeigths.Add(TprProps.DifferentLines, double.Parse(weightLine.Split(';')[1]));
            _tprPropertyWeigths.Add(TprProps.QuantityPerLine, double.Parse(weightLine.Split(';')[2]));
            _tprPropertyWeigths.Add(TprProps.Fill, double.Parse(weightLine.Split(';')[3]));
            _tprPropertyWeigths.Add(TprProps.Bias, double.Parse(weightLine.Split(';')[4]));
            return _tprPropertyWeigths;
        }

        

        double GuessTimeForOrder(Order order)
        {

            double[] input = new double[5];
            input[(int)TprProps.Bias] = _bias * _tprPropertyWeigths[TprProps.Bias];
            input[(int)TprProps.DifferentLines] = order.Lines.Count * _tprPropertyWeigths[TprProps.DifferentLines];
            input[(int)TprProps.QuantityPerLine] = order.Lines.Sum(o => o.Quantity) * _tprPropertyWeigths[TprProps.QuantityPerLine];
            input[(int)TprProps.Fill] = ReLu(FillResult(order) * _tprPropertyWeigths[TprProps.Fill]);


            double[] output = new double[5];
            output = Softmax(input);

            output[(int)TprProps.Bias] *= _tprPropertyWeigths[TprProps.Bias];
            output[(int)TprProps.DifferentLines] *= _tprPropertyWeigths[TprProps.DifferentLines];
            output[(int)TprProps.QuantityPerLine] *= _tprPropertyWeigths[TprProps.QuantityPerLine];
            output[(int)TprProps.Fill] *= _tprPropertyWeigths[TprProps.Fill];


            return output.Sum();

        }

        private static double[] Softmax(double[] oSums)
        {
            // does all output nodes at once so scale doesn't have to be re-computed each time
            // 1. determine max output sum
            double max = oSums[0];
            for (int i = 0; i < oSums.Length; ++i)
                if (oSums[i] > max) max = oSums[i];

            // 2. determine scaling factor -- sum of exp(each val - max)
            double scale = 0.0;
            for (int i = 0; i < oSums.Length; ++i)
                scale += Math.Exp(oSums[i] - max);

            double[] result = new double[oSums.Length];
            for (int i = 0; i < oSums.Length; ++i)
                result[i] = Math.Exp(oSums[i] - max) / scale;

            return result; // now scaled so that xi sum to 1.0
        }


        public double ReLu(double x)
        {
            return x< 0 ? 0:x;
        }


    double FillResult(Order order)
        {
            double[] fillresult = new double [5];
            for (int i = 0; i < _simInfo.GetState().Length-1; i++)
            {
                fillresult[i] = _simInfo.GetState()[i] *
                    order.Lines.Where(l => l.Article.AreaCode == ((AreaCode) i)).Count()/* *
                    order.Lines.Where(l => l.Article.AreaCode == ((AreaCode) i)).Sum(Q => Q.Quantity)*/;
            }

            return fillresult.Sum();
        }




        protected override Order ChooseNextOrder()
        {
            int chosenOrder = ActualOrderPool.OrderBy(o => o.OrderTime).First().OrderId;
           
            _sentOrders.Add(chosenOrder, 
                new Tuple<double, double>(GuessTimeForOrder(ActualOrderPool.OrderBy(o => o.OrderTime).First()), 0));
            return ActualOrderPool.OrderBy(o => o.OrderTime).First();
        }
    }
}
