using System;
using System.Collections.Generic;
using System.Linq;

namespace SolarSystem.Backend.Classes
{
    public class OrderGenerator
    {
        public event Action<Order> CostumerSendsOrderEvent;
        
        private List<Article> ArticleList { get; }
        private static Random _rand = new Random();
        public double OrderChance { get; }

        private int minAmountOfLines = 1;
        private int maxAmountOfLines = 100;

        private int minArticleQuantity = 1;
        private int maxArticleQuantity = 100;

        private const int minOrderNumberId = 10000000;
        private const int maxOrderNumberId = 999999999;
      
        public OrderGenerator(List<Article> articleList, double orderChance)
        {
            ArticleList = articleList ?? throw new ArgumentNullException(nameof(articleList));

            OrderChance = orderChance;
            
            TimeKeeper.Tick += MaybeSendOrder;

        }

        public void MaybeSendOrder()
        {
            double chance = _rand.NextDouble();

            if (chance <= OrderChance)
            {
                var order = GenerateOrder();
                CostumerSendsOrderEvent?.Invoke(order);
            }
            
        }
        
        private Order GenerateOrder()
        {
            // Randomly choose amount of lines
            int numberOfLines = _rand.Next(minAmountOfLines, maxAmountOfLines);
            // Choose amount of unique articles
            List<Article> chosenArticles = ArticleList.OrderBy(x => _rand.Next()).Take(numberOfLines).ToList();
            // Generate lines based on chosen articles
            IEnumerable<Line> generatedLines = chosenArticles.Select(GenerateLine);
            
            // Construct AreasVisited for areas.
            Order order = new Order(_rand.Next(minOrderNumberId, maxOrderNumberId), TimeKeeper.CurrentDateTime, generatedLines.ToList());
            order.Areas = ConstructAreasVisited(order);
            
            return order;
        }

        private Dictionary<AreaCode, bool> ConstructAreasVisited(Order order)
        {
            // Dictionary to be returned.
            var returnAreas = new Dictionary<AreaCode, bool>();

            // Iterate through all lines and add to dictionary
            foreach (var line in order.Lines)
            {
                if (!returnAreas.ContainsKey(line.Article.AreaCode))
                {
                    returnAreas.Add(line.Article.AreaCode, false);
                }
            }
            
            // Sort according to the real flow
            // TODO: Sorting of dictionary needs do!!!
            
            // Return Dictionary
            return returnAreas;

        }
        
        private Line GenerateLine(Article article)
        {
            // Randomly choose quantity
            int quantity = _rand.Next(minArticleQuantity, maxArticleQuantity);
            
            // Assemble a line and return
            Line line = new Line(article, quantity);

            return line;
        }

      
    }
}