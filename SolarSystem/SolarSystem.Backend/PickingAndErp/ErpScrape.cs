using System;
using System.Collections.Generic;
using System.IO;

/*
 * Make ErpScrape object.
 * Use ScrapeErp(filepath)
 * Use SaveToFile()
 * Load from file with LoadListFromFile() : List<PickingOrder>
 */

namespace SolarSystem.Backend.PickingAndErp
{
    public class ErpScrape
    {
        public List<PickingOrder> Orders { get; private set; }



        public ErpScrape()
        {
            Orders = new List<PickingOrder>();
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

        public List<PickingOrder> LoadListFromFile()
        {
            List<PickingOrder> orders;

            string path = AppDomain.CurrentDomain.BaseDirectory + "SaveFiles/";
            string fileName = "Erp.dat";

            using (Stream stream = File.Open(path + fileName, FileMode.Open))
            {
                var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                orders = (List<PickingOrder>)formatter.Deserialize(stream);
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

        private List<PickingOrder> _scraperERPToOrderList(StreamReader stream)
        {
            List<PickingOrder> orderList = new List<PickingOrder>();
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


                        // Skip 4 times to see if there is any more lines for this pickingOrder
                        line = stream.ReadLine();
                        line = stream.ReadLine();
                        line = stream.ReadLine();
                        line = stream.ReadLine();

                    }

                    // PickingOrder done. Add to list
                    PickingOrder pickingOrder = new PickingOrder(orderNumber, lineList);
                    pickingOrder.OrderTime = date;
                    orderList.Add(pickingOrder);
                }

            }

            //foreach (PickingOrder order in orderList)
            //{
            //    DataSaver.orders.Find(o => o.PickingOrder.OrderId == order.OrderNumber).PickingOrder.OrderTime = order.OrderTime;
            //}

            return orderList;

        }
    }
}
// DT: 11 - :18
