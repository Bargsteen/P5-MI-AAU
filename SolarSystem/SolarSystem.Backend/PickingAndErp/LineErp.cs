using System;

namespace SolarSystem.Backend.PickingAndErp
{
    public class Line
    {
        public Classes.Simulation.Orders.Article Article { get;  }
        public int Quantity { get; }
        public DateTime OutTimeStamp { get; }
        
        public Line(Classes.Simulation.Orders.Article article, int quantity, DateTime timeStamp)
        {
            Article = article;
            Quantity = quantity;
            OutTimeStamp = timeStamp;
        }

        

        public override string ToString()
        {
            return "\n\tTime: " + OutTimeStamp + "\n\t" + Article + "\tQuantity:" + Quantity + "\n";
        }

        public Classes.Simulation.Orders.Line ToSimLine()
        {
            return new Classes.Simulation.Orders.Line(Article, Quantity);
           
        }
    }




}
