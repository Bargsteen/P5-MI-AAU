using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SolarSystem.Backend.Classes.Simulation
{

    //Find ud af hvorfor gennemsnit er lavt, men per ordre er høj
    //Skriv den hurtigste og langsomste ordre ud, fordi vi vil se hvad der sænker den
    //Udksriv hvor mange der var 0-1 min, 1-5min osv
    //



    class OrdertimeEstimator
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


        public OrdertimeEstimator()
        {
            //Fetch the weights from last run, stored in the Weights.txt file
            TPRPropertyWeigths = UpdateWeightsFromFile();
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



        public double GuessTimeForOrder(Order order)
        {
            return Bias * TPRPropertyWeigths[TPRProps.Bias] + order.Lines.Count * TPRPropertyWeigths[TPRProps.DifferentLines] +
                   order.Lines.Sum(o => o.Quantity) * TPRPropertyWeigths[TPRProps.QuantityPerLine] + FillResult(order) * TPRPropertyWeigths[TPRProps.Fill];
        }


        double FillResult(Order order)
        {
            double[] _fillresult = new double[5];
            for (int i = 0; i < _simInfo.GetState().Length - 1; i++)
            {
                _fillresult[i] = _simInfo.GetState()[i] *
                                 order.Lines.Count(l => l.Article.AreaCode == ((AreaCode)i));
            }

            return _fillresult.Sum();
        }
    }
}
