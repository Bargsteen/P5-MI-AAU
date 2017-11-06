using System;
using SolarSystem.Backend.Classes;

namespace SolarSystem.PickingAndErp
{
    public class Line
    {
        public Backend.Classes.Article Article { get;  }
        public int Quantity { get; private set; }
        public DateTime Timestamp { get; private set; }
        
        public Line(Backend.Classes.Article article, int quantity, DateTime timeStamp)
        {
            Article = article;
            this.Quantity = quantity;
            Timestamp = timeStamp;
        }

        

        public override string ToString()
        {
            return "\n\tTime: " + Timestamp + "\n\t" + Article + "\tQuantity:" + Quantity + "\n";
        }
    }




}
