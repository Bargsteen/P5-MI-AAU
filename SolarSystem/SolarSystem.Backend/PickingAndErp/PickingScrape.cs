using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolarSystem.Backend.Classes;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend.PickingAndErp
{
    public class PickingScrape
    {
        public PickingScrape(string path)
        {
            _path = path;
            OrderList = new List<Order>();
        }

        private readonly string _path;

        public readonly List<Order> OrderList;

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
                        var timeStampForPicking = DateTime.Parse(lineElements[8]);
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
                Order order = new Order(kvp.Key, kvp.Value);
                order.OrderTime = DateTime.Now;
                OrderList.Add(order);
            }

            foreach (Order order in OrderList)
            {
                for(int i = 0; i < order.LineList.Count; i++)
                {
                    Outputter.LinesFromScrape.Add(order.OrderNumber + ";" + order.LineList[i].Article.Id + ";" + order.LineList[i].Timestamp.Hour + ":" + order.LineList[i].Timestamp.Minute + ":" + order.LineList[i].Timestamp.Second);
                }
            }
        }

    }
}
