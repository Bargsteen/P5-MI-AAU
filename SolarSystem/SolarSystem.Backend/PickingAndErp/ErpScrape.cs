using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolarSystem;
using SolarSystem.Backend.Classes.Data;

/*
 * Make ErpScrape object.
 * Use ScrapeErp(filepath)
 * Use SaveToFile()
 * Load from file with LoadListFromFile() : List<Order>
 */

namespace SolarSystem.Backend.PickingAndErp
{
    public class ErpScrape
    {
        public List<Order> Orders { get; private set; }



        public ErpScrape()
        {
            Orders = new List<Order>();
        }

        public void SaveToFile()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "SaveFiles/";
            string fileName = "Erp.dat";

            Directory.CreateDirectory(path);

            if (Orders != null)
            {
                using (Stream stream = File.Open(path + fileName, FileMode.Create))
                {
                    var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    formatter.Serialize(stream, Orders);
                }
            }
            else
            {
                Console.WriteLine("Call ScrapeErp(string path) first");
            }
        }

        public List<Order> LoadListFromFile()
        {
            List<Order> orders;

            string path = AppDomain.CurrentDomain.BaseDirectory + "SaveFiles/";
            string fileName = "Erp.dat";

            using (Stream stream = File.Open(path + fileName, FileMode.Open))
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                orders = (List<Order>)formatter.Deserialize(stream);
            }

            return orders;
        }

        public void ScrapeErp(string path)
        {
            // Does file exist?
            try
            {
                Orders = _scraperERPToOrderList(new StreamReader(path));
            }
            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine("File doesn't exist!\nException: " + e.Message);
            }
        }

        private List<Order> _scraperERPToOrderList(StreamReader stream)
        {
            List<Order> orderList = new List<Order>();
            List<Line> lineList = new List<Line>();

            // Retrive orders.
            List<String> buffer = new List<string>();
            string line;
            int bufferLimit = 6;
            int counter = 0;

            DateTime date = new DateTime();
            int orderNumber = 0, articleNumber = 0, quantity = 0, areaNumber = -1;

            while ((line = stream.ReadLine()) != null)
            {

                buffer.Add(line);

                counter++;

                // For ordernumber and date-time
                if (line.Contains("OrderNumber: 15"))
                {
                    // Clear the lines from the previous orders.
                    lineList.Clear();

                    // Retrieve ordernumber
                    orderNumber = int.Parse(line.Substring(15, 6));


                    // Retrieve data in buffer (Get the date and time)
                    string bufferString = buffer[counter - 6];
                    date = new DateTime(int.Parse(bufferString.Substring(0, 4)), int.Parse(bufferString.Substring(5, 2)), int.Parse(bufferString.Substring(8, 2)), int.Parse(bufferString.Substring(11, 2)), int.Parse(bufferString.Substring(14, 2)), int.Parse(bufferString.Substring(17, 2)));


                    // Reset buffer and counter
                    buffer.Clear();
                    counter = 0;
                    
                    // skip 6 lines to get to articlenumber.
                    line = stream.ReadLine();
                    line = stream.ReadLine();
                    line = stream.ReadLine();
                    line = stream.ReadLine();
                    line = stream.ReadLine();
                    line = stream.ReadLine();

                    while (line.Contains("ArticleNumber: "))
                    {
                        // Read ArticleNumber
                        articleNumber = int.Parse(line.Substring(15, 9));


                        // Skip 3 lines
                        line = stream.ReadLine();
                        line = stream.ReadLine();
                        line = stream.ReadLine();

                        // Read Quantity
                        //_quantity = int.Parse(_line.Substring(9));

                        // Add line to lineList
                        //_lineList.Add(new Line(new Article(_articleNumber, _areaNumber), _quantity, _date));


                        // Skip 4 times to see if there is any more lines for this order
                        line = stream.ReadLine();
                        line = stream.ReadLine();
                        line = stream.ReadLine();
                        line = stream.ReadLine();

                    }

                    // Order done. Add to list
                    Order order = new Order(orderNumber, lineList);
                    order.OrderTime = date;
                    orderList.Add(order);
                }

            }

            //foreach (Order order in orderList)
            //{
            //    DataSaving.orders.Find(o => o.Order.OrderId == order.OrderNumber).Order.OrderTime = order.OrderTime;
            //}

            return orderList;

        }
    }
}
// DT: 11 - :18
