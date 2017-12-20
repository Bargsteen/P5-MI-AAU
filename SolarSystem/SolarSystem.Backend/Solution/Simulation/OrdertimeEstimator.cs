using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolarSystem.Backend.Solution.Simulation.Orders;
using SolarSystem.Backend.Solution.Simulation.Warehouse;

namespace SolarSystem.Backend.Solution.Simulation
{

    class OrdertimeEstimator
    {
        private enum TprProps
        {
            AreasToVisit,
            DifferentLines,
            QuantityPerLine,
            Bias,
            Fill
        }


        private Handler _handler;
        private int _maxAmountOfLines = 60;
        private int _maxQuantityPerLine = 900;
        private readonly SimulationInformation _simInfo;
        private List<Tuple<double, double>> _tprProperties;
        private Dictionary<TprProps, double> _tprPropertyWeigths;
        private double _learningRate = 0.00001f;
        private Dictionary<int, Tuple<double, double>> _sentOrders;
        private const double Bias = 5;


        public OrdertimeEstimator(SimulationInformation simInfo)
        {
            //Fetch the weights from last run, stored in the Weights.txt file
            _tprPropertyWeigths = UpdateWeightsFromFile();
            _simInfo = simInfo;
            
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



        public double GuessTimeForOrder(Order order)
        {

            if (order.OrderId == 0)
                return 0;
            return Bias * _tprPropertyWeigths[TprProps.Bias] + order.Lines.Count * _tprPropertyWeigths[TprProps.DifferentLines] +
                   order.Lines.Sum(o => o.Quantity) * _tprPropertyWeigths[TprProps.QuantityPerLine] + FillResult(order) * _tprPropertyWeigths[TprProps.Fill];
        }


        double FillResult(Order order)
        {
            double[] fillresult = new double[5];
            for (int i = 0; i < _simInfo.GetState().Length - 1; i++)
            {
                fillresult[i] = _simInfo.GetState()[i] *
                                 order.Lines.Count(l => l.Article.AreaCode == ((AreaCode)i));
            }

            return fillresult.Sum();
        }
    }
}
