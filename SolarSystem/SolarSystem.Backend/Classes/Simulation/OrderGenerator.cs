using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SolarSystem.Backend.Classes.Simulation
{
    public class OrderGenerator
    {
        public event Action<Order> CostumerSendsOrderEvent;

        private readonly OrderGenerationConfiguration _currentOrderGenerationConfiguration;
        private List<int> _usedOrderNumbers = new List<int>();

        private List<Article> ArticleList { get; }
        private static readonly Random Rand = SimulationConfiguration.SeedType == RandomSeedType.Random ? new Random() : new Random(SimulationConfiguration.GetRandomSeedValue());
        private double OrderChance { get; }

        private static readonly int MinAmountOfLines = SimulationConfiguration.GetMinLineCountOG();
        private static readonly int MaxAmountOfLines = SimulationConfiguration.GetMaxLineCountOG();

        private static readonly int MinArticleQuantity = SimulationConfiguration.GetMinArticleQuanitityOG();
        private static readonly int MaxArticleQuantity = SimulationConfiguration.GetMaxArticleQuantityOG();

        private const int MinOrderNumberId = 10000000;
        private const int MaxOrderNumberId = 999999999;

        //private int _sendOrderCount = 0;

        private int fromFileOrderSentCount = 0;
        private int fromFileOrderCount;

        List<string> _sentlines = new List<string>();

        private readonly List<PickingAndErp.PickingOrder> _scrapedOrders;

        public OrderGenerator(List<Article> articleList, double orderChance, List<PickingAndErp.PickingOrder> scrapedOrders, OrderGenerationConfiguration conf)
        {

            ArticleList = articleList ?? throw new ArgumentNullException(nameof(articleList));

            OrderChance = orderChance;

            _scrapedOrders = scrapedOrders;

            _scrapedOrders.Sort((x, y) => x.OrderTime.CompareTo(y.OrderTime));

            fromFileOrderCount = _scrapedOrders.Count;

            _currentOrderGenerationConfiguration = conf;
        }

        public void Start()
        {
            TimeKeeper.Tick += MaybeSendOrder;

        }


        public void MaybeSendOrder()
        {
            if (_scrapedOrders.Count == 0)
                return;
            Order order;
            switch (_currentOrderGenerationConfiguration)
            {
                case OrderGenerationConfiguration.FromFile:

                    int nextOrderIndex = fromFileOrderSentCount % fromFileOrderCount;
                    
                    if (_scrapedOrders[nextOrderIndex].OrderTime.CompareTo(TimeKeeper.CurrentDateTime) < 0)
                    {
                        order = _scrapedOrders[nextOrderIndex].ToSimOrder();
                        order.Areas = ConstructAreasVisited(order);
                        CostumerSendsOrderEvent?.Invoke(order);
                        //_sentlines.Add(order.OrderId.ToString());
                        //_scrapedOrders.RemoveAt(0);
                        //_sendOrderCount = 0;
                        
                        fromFileOrderSentCount++;
                    }
                    
                    break;


                case OrderGenerationConfiguration.Random:
                    
                    double chance = Rand.NextDouble();
                    if (chance <= OrderChance)
                    {
                        //_sendOrderCount = 0;
                        order = GenerateOrder();
                        order.Areas = ConstructAreasVisited(order);
                        CostumerSendsOrderEvent?.Invoke(order);
                    }
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Order GenerateOrder()
        {
            // Randomly choose amount of lines
            int numberOfLines = Rand.Next(MinAmountOfLines, MaxAmountOfLines);
            // Choose amount of unique articles
            List<Article> chosenArticles = ArticleList.OrderBy(x => Rand.Next()).Take(numberOfLines).ToList();
            //List<Article> chosenArticles = ArticleList.Where(a => a.AreaCode == AreaCode.Area21 || a.AreaCode == AreaCode.Area25).OrderBy(x => Rand.Next()).Take(numberOfLines).ToList();
            // Generate lines based on chosen articles
            var generatedLines = chosenArticles.Select(GenerateLine).ToList();

            // Construct AreasVisited for areas.
            int orderID;
            do
            {
                orderID = Rand.Next(MinOrderNumberId, MaxOrderNumberId);
            } while (_usedOrderNumbers.Contains(orderID));
            _usedOrderNumbers.Add(orderID);
            Order order = new Order(orderID, TimeKeeper.CurrentDateTime, generatedLines);
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
            int quantity = Rand.Next(MinArticleQuantity, MaxArticleQuantity);

            // Assemble a line and return
            Line line = new Line(article, quantity);

            return line;
        }


    }
}