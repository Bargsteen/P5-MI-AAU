using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolarSystem.Backend.Classes.Data;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend.PickingAndErp
{
    public class PickingScrape
    {
        public PickingScrape(string path)
        {
            _path = path;
            OrderList = new List<PickingOrder>();
        }

        private readonly string _path;

        public readonly List<PickingOrder> OrderList;

        private AreaCode AreaIntToCode(int areaInt)
        {
            switch (areaInt)
            {
                    case 21:
                        return AreaCode.Area21;
                    case 25:
                        return AreaCode.Area25;
                    case 27:
                        return AreaCode.Area27;
                    case 28:
                        return AreaCode.Area28;
                    case 29:
                        return AreaCode.Area29;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(areaInt));
            }
        }


        public void GetOrdersFromPicking()
        {
            // Initialize reader
            var reader = new StreamReader(_path);
            
            // Skip the column-name line
            reader.ReadLine(); 
            
            var ordersGathered = new Dictionary<int, List<Line>>();

            while (!reader.EndOfStream)
            {
                var lineElements = reader.ReadLine()?.Split(';');

                try
                {
                    if (lineElements != null)
                    {
                        var orderNumber = int.Parse(lineElements[0]);
                        var articleNumber = long.Parse(lineElements[1]);
                        var areaCode = AreaIntToCode(int.Parse(lineElements[10]));
                        var stationNumber = int.Parse(lineElements[11]);
                        var timeStampForPicking = new DateTime(2017, 10, 2, 0, 0, 0) + TimeSpan.Parse(lineElements[8]);
                        var lineQuantity = int.Parse(lineElements[15].Replace(".", ""));
                        var materialName = lineElements[4];

                        var article = new Classes.Simulation.Article(articleNumber, materialName, areaCode);
                        var line = new Line(article, lineQuantity, timeStampForPicking);

                        if (!ordersGathered.ContainsKey(orderNumber))
                        {
                            ordersGathered.Add(orderNumber, new List<Line> {line});
                            continue;
                        }
                        ordersGathered[orderNumber].Add(line);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"ERROR : {e}");
                }
               
            }

            foreach (var kvp in ordersGathered)
            {
                PickingOrder pickingOrder = new PickingOrder(kvp.Key, kvp.Value);
                pickingOrder.OrderTime = pickingOrder.LineList.OrderBy(l => l.OutTimeStamp).First().OutTimeStamp;
                OrderList.Add(pickingOrder);
            }
            
            
           
        }

    }
}
