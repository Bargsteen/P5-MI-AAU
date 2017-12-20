using System;
using SolarSystem.Backend.Solution.Simulation.Orders;

namespace SolarSystem.Backend.Extraction
{
    public class Line
    {
        public Article Article { get;  }
        public int Quantity { get; }
        public DateTime OutTimeStamp { get; }
        
        public Line(Article article, int quantity, DateTime timeStamp)
        {
            Article = article;
            Quantity = quantity;
            OutTimeStamp = timeStamp;
        }

        public override string ToString()
        {
            return "\n\tTime: " + OutTimeStamp + "\n\t" + Article + "\tQuantity:" + Quantity + "\n";
        }

        public Solution.Simulation.Orders.Line ToSimLine()
        {
            return new Solution.Simulation.Orders.Line(this.Article, this.Quantity);
           
        }
    }




}
