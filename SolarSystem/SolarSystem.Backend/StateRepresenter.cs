using System.Collections.Generic;
using System.Linq;
using Accord.Math;
using SolarSystem.Backend.Classes;
using SolarSystem.Backend.Classes.Simulation;

namespace SolarSystem.Backend
{
    public static class StateRepresenter
    {
        public static Sparse<double> MakeOrderRepresentation(Order order, List<Article> articles)
        {
            int articleCount = articles.Count;
            // Create vector for return
            double[] orderVector = new double[articleCount];
            
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
                else
                {
                    // otherwise add 0 in the vector
                    orderVector[i] = 0;
                }
            }
            return Sparse.FromDense(orderVector, false);
        }

        public static Sparse<double> MakeFullRepresentation(Order order, List<Article> articles,
            Sparse<double> stateRepresentation)
        {
            int articleCount = articles.Count;
            // Create vector for return
            List<double> orderVector = new List<double>(articleCount + stateRepresentation.Length);
            
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
                else
                {
                    // otherwise add 0 in the vector
                    orderVector[i] = 0;
                }
            }
            orderVector.AddRange(stateRepresentation);
            return Sparse.FromDense(orderVector.ToArray(), false);
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