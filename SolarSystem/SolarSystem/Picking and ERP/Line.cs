using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Picking_and_ERP
{
    [Serializable]
    public class Line
    {
        public Article _article { get; private set; }
        public int _quantity { get; private set; }
        public DateTime _timestamp { get; private set; }
        
        public Line(Article article, int Quantity, DateTime TimeStamp)
        {
            _article = article;
            _quantity = Quantity;
            _timestamp = TimeStamp;
        }

        

        public override string ToString()
        {
            return "\n\tTime: " + _timestamp + "\n\t" + _article + "\tQuantity:" + _quantity + "\n";
        }
    }




}
