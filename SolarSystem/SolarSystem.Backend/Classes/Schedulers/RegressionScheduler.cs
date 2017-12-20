using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend.Classes.Simulation;
using System.IO;
using Accord.Statistics.Kernels;

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
        private double _learningRate = 0.0001f;
        private Dictionary<int, Tuple<double, double>> sentOrders;
        private double Bias = 5;
        int[] timings = new int[3];

        public RegressionScheduler(OrderGenerator orderGenerator, Handler handler, double poolMoverTime, SimulationInformation simInfo) : base(orderGenerator, handler, poolMoverTime)
        {
            _handler = handler;
            _simInfo = simInfo;
            double _sumOfAllErrors = 0;
            sentOrders = new Dictionary<int, Tuple<double, double>>();

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
            //double sumOfAllResults = 0;

            //sumOfAllResults += TPRPropertyWeigths[TPRProps.AreasToVisit] * order.Areas.Count;
            //sumOfAllResults += TPRPropertyWeigths[TPRProps.DifferentLines] * order.Lines.Count;
            //sumOfAllResults += TPRPropertyWeigths[TPRProps.QuantityPerLine] * order.Lines.Sum(l => l.Quantity);
            //sumOfAllResults += TPRPropertyWeigths[TPRProps.Fill] * FillResult(order);
            //sumOfAllResults += TPRPropertyWeigths[TPRProps.Bias] * Bias;


            double[] input = new double[5];
            input[(int)TPRProps.AreasToVisit] = order.Areas.Count * TPRPropertyWeigths[TPRProps.AreasToVisit];
            input[(int)TPRProps.Bias] = Bias * TPRPropertyWeigths[TPRProps.Bias];
            input[(int)TPRProps.DifferentLines] = order.Lines.Count * TPRPropertyWeigths[TPRProps.DifferentLines];
            input[(int)TPRProps.QuantityPerLine] = order.Lines.Sum(o => o.Quantity) * TPRPropertyWeigths[TPRProps.QuantityPerLine];
            input[(int)TPRProps.Fill] = ReLU(FillResult(order) * TPRPropertyWeigths[TPRProps.Fill]);


            double[] output = new double[5];
            output = Softmax(input);


            TPRPropertyWeigths[TPRProps.AreasToVisit] += output[(int)TPRProps.AreasToVisit] * TPRPropertyWeigths[TPRProps.AreasToVisit] * error * _learningRate;
            TPRPropertyWeigths[TPRProps.DifferentLines] += output[(int)TPRProps.DifferentLines] * TPRPropertyWeigths[TPRProps.DifferentLines] * error * _learningRate;
            TPRPropertyWeigths[TPRProps.QuantityPerLine]  += output[(int)TPRProps.QuantityPerLine] * TPRPropertyWeigths[TPRProps.QuantityPerLine] * error * _learningRate;
            TPRPropertyWeigths[TPRProps.Fill] += output[(int)TPRProps.Fill] * TPRPropertyWeigths[TPRProps.Fill] * error * _learningRate;
            TPRPropertyWeigths[TPRProps.Bias] += output[(int)TPRProps.Bias] * TPRPropertyWeigths[TPRProps.Bias] * error * _learningRate;
   

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

            double[] input = new double[5];
            input[(int)TPRProps.Bias] = Bias * TPRPropertyWeigths[TPRProps.Bias];
            input[(int)TPRProps.DifferentLines] = order.Lines.Count * TPRPropertyWeigths[TPRProps.DifferentLines];
            input[(int)TPRProps.QuantityPerLine] = order.Lines.Sum(o => o.Quantity) * TPRPropertyWeigths[TPRProps.QuantityPerLine];
            input[(int)TPRProps.Fill] = ReLU(FillResult(order) * TPRPropertyWeigths[TPRProps.Fill]);


            double[] output = new double[5];
            output = Softmax(input);

            output[(int)TPRProps.Bias] *= TPRPropertyWeigths[TPRProps.Bias];
            output[(int)TPRProps.DifferentLines] *= TPRPropertyWeigths[TPRProps.DifferentLines];
            output[(int)TPRProps.QuantityPerLine] *= TPRPropertyWeigths[TPRProps.QuantityPerLine];
            output[(int)TPRProps.Fill] *= TPRPropertyWeigths[TPRProps.Fill];


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


        public double ReLU(double x)
        {
            return x< 0 ? 0:x;
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
