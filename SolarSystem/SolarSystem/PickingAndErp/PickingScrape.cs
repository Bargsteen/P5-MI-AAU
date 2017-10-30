using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SolarSystem.Picking_and_ERP
{
    class PickingScrape
    {
        public PickingScrape(string Path)
        {
            _path = Path;
        }

        static string _path;

        public List<Order> _pickingOrderList;

        

        void OrdersFromPicking(string path)
        {
            try
            {

            }
            catch (ArgumentNullException)
            {

                throw;
            }
        }


        public void GetOrdersFromPicking(StreamReader reader)
        {
            reader.ReadLine();

            List<string[]> list = new List<string[]>();

            while (!reader.EndOfStream)
            {
                list.Add(reader.ReadLine().Split(';'));
            }

            int index = 0;

            foreach (string[] line in list)
            {

                if (index > 0 && Int32.Parse(line[0]) == _pickingOrderList.Last().orderNumber)
                {
                    _pickingOrderList.Last().lineList.Add(
                        new Line(
                            new Article(
                                int.Parse(line[1]),
                                int.Parse(line[10])), int.Parse(line[15]), 
                                DateTime.Parse(line[8].ToString())));
                }
                else
                {
                    _pickingOrderList.Add(
                        new Order(
                            int.Parse(line[0]), 
                                new Line(
                                new Article(
                                    int.Parse(line[1]),
                                    int.Parse(line[10])),
                                    int.Parse(line[15]), 
                                    DateTime.Parse(line[8].ToString()))));
                index++;
            }

            foreach (var item in list)
            {
                Console.WriteLine(item);
            }



        }
    }
}
