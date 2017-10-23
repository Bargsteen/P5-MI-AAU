using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Picking_and_ERP
{
    public class Line
    {
        public Line(Article article, int Quantity, DateTime TimeStamp)
        {
            _article = article;
            _quantity = Quantity;
            _timestamp = TimeStamp;
        }

        Article _article;
        int _quantity;
        DateTime _timestamp;

        public override string ToString()
        {
            return "Quantity: " + _quantity + "\nTimeStamp: " + _timestamp + _article;
        }
    }




}
