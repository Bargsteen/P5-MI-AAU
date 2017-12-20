using System;

namespace SolarSystem.Backend.PickingAndErp
{
    public class Line
    {
        public Classes.Simulation.Article Article { get;  }
        public int Quantity { get; }
        public DateTime OutTimeStamp { get; }
        
        public Line(Classes.Simulation.Article article, int quantity, DateTime timeStamp)
        {
            Article = article;
            Quantity = quantity;
            OutTimeStamp = timeStamp;
        }

        

        public override string ToString()
        {
            return "\n\tTime: " + OutTimeStamp + "\n\t" + Article + "\tQuantity:" + Quantity + "\n";
        }

        public Classes.Simulation.Line ToSimLine()
        {
            return new Classes.Simulation.Line(this.Article, this.Quantity);
           
        }
    }




}
