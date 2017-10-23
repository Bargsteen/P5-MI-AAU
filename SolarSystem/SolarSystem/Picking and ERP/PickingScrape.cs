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

        List<Order> _pickingOrderList;

        public void GetOrdersFromPicking()
        {
            StreamReader reader = new StreamReader(@_path);

            reader.ReadLine();

            var stringLine = reader.ReadLine();
            string[] list = stringLine.Split(';');

            int index = 0;

            foreach (string line in list)
            {
                if (index > 0 && line[0] == _pickingOrderList.Last().orderNumber)
                {
                    _pickingOrderList.Last().lineList.Add(new Line(new Article(line[1], line[10]), line[15], DateTime.Parse(line[8].ToString())));
                }
                else
                {
                    //_pickingOrderList.Add(new Order(line[0], new Line(new Article(line[1], line[10]), line[15], DateTime.Parse(line[8].ToString()))));
                }
            }
        }
    }
}
