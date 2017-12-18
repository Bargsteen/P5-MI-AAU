using System;
using System.Collections.Generic;
using System.IO;
using SolarSystem.Backend.Classes.Simulation;
using System.Linq;

namespace SolarSystem
{
    public class BargsteenData
    {
        private List<string> orderInfo;
            
        public BargsteenData(Handler handler)
        {
            orderInfo = new List<string>();
            orderInfo.Add("StartPackingTime, EndPackingTime, TotalQuantity");
            handler.OnOrderBoxFinished += orderBox => AddOrderInfoToList(orderBox.Order);
            TimeKeeper.SimulationFinished += SaveAllInfoToFile;
        }

        private void AddOrderInfoToList(Order order)
        {
            orderInfo.Add($"{order.StartPackingTime}, {TimeKeeper.CurrentDateTime}, {order.Lines.Sum(l => l.Quantity)}");
        }

        private void SaveAllInfoToFile()
        {
            var filePath =
                Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString())
                    .ToString()) + "/SolarSystem.Backend/SolarData/BargsteenData.csv";
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                foreach (var order in orderInfo)
                {
                    writer.WriteLine(order);
                }
                writer.Close();
            }
        }
        
    }
}