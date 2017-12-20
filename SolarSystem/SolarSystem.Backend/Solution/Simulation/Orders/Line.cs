using System;

namespace SolarSystem.Backend.Solution.Simulation.Orders
{
    public class Line
    {
        public Article Article { get; set; }
        public int Quantity { get; set; }


        public Line(Article article, int quantity)
        {
            Article = article ?? throw new ArgumentNullException(nameof(article));
            Quantity = quantity;
            
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Line lineObj))
            {
                return false;
            }
            return Equals(lineObj);
        }

        private bool Equals(Line other)
        {
            return Equals(Article, other.Article) && Quantity == other.Quantity;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Article != null ? Article.GetHashCode() : 0) * 397) ^ Quantity;
            }
        }

        public override string ToString()
        {
            return ($@"({Article.Name}, {Quantity}");
        }
    }
}
