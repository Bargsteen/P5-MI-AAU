using System;
using System.Collections.Generic;
using System.Linq;

namespace SolarSystem.Backend.Classes
{
    public class OrderGenerator
    {
        public event Action<Order> CostumerSendsOrderEvent;
        
        private List<Article> ArticleList { get; }
        private static readonly Random Rand = new Random();
        public double OrderChance { get; }

        private int minAmountOfLines = 1;
        private int maxAmountOfLines = 8;

        private int minArticleQuantity = 1;
        private int maxArticleQuantity = 5;

        private const int minOrderNumberId = 10000000;
        private const int maxOrderNumberId = 999999999;

        private int SendOrderCount = 0;

        private List<PickingAndErp.Order> ScrapedOrders;

        public OrderGenerator(List<Article> articleList, double orderChance, List<PickingAndErp.Order> scrapedOrders)
        {

            ArticleList = articleList ?? throw new ArgumentNullException(nameof(articleList));

            OrderChance = orderChance;

            ScrapedOrders = scrapedOrders;
            
            TimeKeeper.Tick += MaybeSendOrder;

        }

        public void MaybeSendOrder()
        {
            if (SendOrderCount++ >= 10)
            {
                SendOrderCount = 1337;
                var order = ScrapedOrders[0].ToSimOrder();
                order.Areas = ConstructAreasVisited(order);
                CostumerSendsOrderEvent?.Invoke(order);
            }
            
            
            /*double chance = Rand.NextDouble();

            if (chance <= OrderChance)
            {
                
                
            }*/
            
        }
        
        private Order GenerateOrder()
        {
            // Randomly choose amount of lines
            int numberOfLines = Rand.Next(minAmountOfLines, maxAmountOfLines);
            // Choose amount of unique articles
            List<Article> chosenArticles = ArticleList.OrderBy(x => Rand.Next()).Take(numberOfLines).ToList();
            //List<Article> chosenArticles = ArticleList.Where(a => a.AreaCode == AreaCode.Area21 || a.AreaCode == AreaCode.Area25).OrderBy(x => Rand.Next()).Take(numberOfLines).ToList();
            // Generate lines based on chosen articles
            var generatedLines = chosenArticles.Select(GenerateLine).ToList();
            
            // Construct AreasVisited for areas.
            Order order = new Order(Rand.Next(minOrderNumberId, maxOrderNumberId), TimeKeeper.CurrentDateTime, generatedLines);
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
            int quantity = Rand.Next(minArticleQuantity, maxArticleQuantity);
            
            // Assemble a line and return
            Line line = new Line(article, quantity);

            return line;
        }

      
    }
}