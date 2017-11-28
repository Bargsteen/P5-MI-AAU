using System;
using SolarSystem.Backend.Classes;

namespace SolarSystem.PickingAndErp
{
    public class Line
    {
        public Article Article { get;  }
        public int Quantity { get; private set; }
        public DateTime Timestamp { get; private set; }
        
        public Line(Article article, int quantity, DateTime timeStamp)
        {
            Article = article;
            Quantity = quantity;
            Timestamp = timeStamp;
        }

        

        public override string ToString()
        {
            return "\n\tTime: " + Timestamp + "\n\t" + Article + "\tQuantity:" + Quantity + "\n";
        }

        public Backend.Classes.Line ToSimLine()
        {
            return new Backend.Classes.Line(this.Article, this.Quantity);
           
        }
    }




}
