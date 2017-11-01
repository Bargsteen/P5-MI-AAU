using System;

namespace SolarSystem.PickingAndErp
{
    public class Line
    {
        public Article Article { get; private set; }
        public int Quantity { get; private set; }
        public DateTime Timestamp { get; private set; }
        
        public Line(Article article, int quantity, DateTime timeStamp)
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
