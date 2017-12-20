using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolarSystem.Backend.Classes.Simulation;
using SolarSystem.Backend.Classes.Simulation.Orders;
using SolarSystem.Backend.Classes.Simulation.WareHouse;

namespace SolarSystem.SaveAndPrint
{
    public class SaveOrderData
    {
        private readonly List<string> orderInfo;
            
        public SaveOrderData(Handler handler)
        {
            orderInfo = new List<string>();
            orderInfo.Add("EndPackingTime, StartPackingTime, DeltaTime, TotalQuantity");
            handler.OnOrderBoxFinished += orderBox => AddOrderInfoToList(orderBox.Order);
            TimeKeeper.SimulationFinished += SaveAllInfoToFile;
        }

        private void AddOrderInfoToList(Order order)
        {
            orderInfo.Add($"{To24HourFormat(order.StartPackingTime)}, {To24HourFormat(TimeKeeper.CurrentDateTime)}, {order.Lines.Sum(l => l.Quantity)}");
        }

        private void SaveAllInfoToFile()
        {
            var filePath =
                Directory.GetParent(Directory.GetParent(Directory.GetParent(Environment.CurrentDirectory).ToString())
                    .ToString()) + "/SolarSystem.Backend/SolarData/SaveOrderData.csv";
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                foreach (var order in orderInfo)
                {
                    writer.WriteLine(order);
                }
                writer.Close();
            }
        }

        private static string To24HourFormat(DateTime dateTime)
        {
            return dateTime.ToString("HH:mm:ss");
        }
        
    }
}