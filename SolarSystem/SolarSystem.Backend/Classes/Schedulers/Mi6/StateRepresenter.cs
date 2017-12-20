using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using SolarSystem.Backend.Classes.Simulation.Orders;

namespace SolarSystem.Backend.Classes.Schedulers.Mi6
{
    public static class StateRepresenter
    {
        public static double[] MakeOrderRepresentation(Order order, List<Article> articles)
        {
            int articleCount = articles.Count;
            // Create vector for return
            double[] orderVector = Enumerable.Repeat(0d, articleCount).ToArray();
            
            // Get unique order lines with the quantities of duplicates summed
            List<Line> uniqueLines = SumLinesInOrder(order);
            // Loop through all articles.
            for (int i = 0; i < articleCount; i++)
            {
                //If they are needed in the order, add quantity in vector
                if (uniqueLines.Any(l => l.Article.Equals(articles[i])))
                {
                    orderVector[i] = uniqueLines.First(l => Equals(l.Article, articles[i])).Quantity;
                }
                // Otherwise leave the 0 in place
            }
            return orderVector;
        }

        public static Sparse<double> MakeFullRepresentation(Order order, List<Article> articles,
            double[] stateRepresentation)
        {
            int articleCount = articles.Count;
            // Create vector for return
            double[] orderVector = Enumerable.Repeat(0d, articleCount).ToArray();
            
            // Get unique order lines with the quantities of duplicates summed
            List<Line> uniqueLines = SumLinesInOrder(order);
            // Loop through all articles.
            for (int i = 0; i < articleCount; i++)
            {
                //If they are needed in the order, add quantity in vector
                if (uniqueLines.Any(l => l.Article.Equals(articles[i])))
                {
                    orderVector[i] = uniqueLines.First(l => Equals(l.Article, articles[i])).Quantity;
                }
                // otherwise just keep the 0 in the position
            }
            
            //orderVector.AddRange(stateRepresentation);
            var fullVector = new List<double>();
            fullVector.AddRange(orderVector);
            fullVector.AddRange(stateRepresentation);
            
            return Sparse.FromDense(fullVector.ToArray(), false);
        }

        public static double[] MakeSimulationRepresentation(Dictionary<string, double> simStats)
        {
            return simStats.Select(kvp => kvp.Value).ToArray();
        }

        public static List<Line> SumLinesInOrder(Order order)
        {
            return order.Lines
                .GroupBy(l => l.Article, l => l.Quantity)
                .Select(g => new Line(g.Key, g.Sum(x => x)))
                .ToList();
        }
    }
}