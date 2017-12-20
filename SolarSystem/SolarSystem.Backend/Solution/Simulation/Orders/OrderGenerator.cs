using System;
using System.Collections.Generic;
using System.Linq;
using SolarSystem.Backend.Extraction;

namespace SolarSystem.Backend.Solution.Simulation.Orders
{
    public class OrderGenerator
    {
        public event Action<Order> CostumerSendsOrderEvent;

        private readonly OrderGenerationConfiguration _currentOrderGenerationConfiguration;
        private readonly List<int> _usedOrderNumbers = new List<int>();

        private List<Article> ArticleList { get; }
        private static readonly Random Rand = SimulationConfiguration.SeedType == RandomSeedType.Random ? new Random() : new Random(SimulationConfiguration.GetRandomSeedValue());
        private double OrderChance { get; }

        private static readonly int MinAmountOfLines = SimulationConfiguration.GetMinLineCountOg();
        private static readonly int MaxAmountOfLines = SimulationConfiguration.GetMaxLineCountOg();

        private static readonly int MinArticleQuantity = SimulationConfiguration.GetMinArticleQuanitityOg();
        private static readonly int MaxArticleQuantity = SimulationConfiguration.GetMaxArticleQuantityOg();

        private const int MinOrderNumberId = 10000000;
        private const int MaxOrderNumberId = 999999999;

        //private int _sendOrderCount = 0;

        private int _fromFileOrderSentCount = 0;
        private readonly int _fromFileOrderCount;

        List<string> _sentlines = new List<string>();

        private readonly List<PickingOrder> _scrapedOrders;

        public OrderGenerator(List<Article> articleList, double orderChance, List<PickingOrder> scrapedOrders, OrderGenerationConfiguration conf)
        {

            ArticleList = articleList ?? throw new ArgumentNullException(nameof(articleList));

            OrderChance = orderChance;

            _scrapedOrders = scrapedOrders;

            _scrapedOrders.Sort((x, y) => x.OrderTime.CompareTo(y.OrderTime));

            _fromFileOrderCount = _scrapedOrders.Count;

            _currentOrderGenerationConfiguration = conf;

            TimeKeeper.SimulationFinished += ResetOrderSentCount;
        }

        public void Start()
        {
            TimeKeeper.Tick += MaybeSendOrder;

        }

        private void ResetOrderSentCount()
        {
            _fromFileOrderSentCount = 0;
        }

        public void MaybeSendOrder()
        {
            if (_scrapedOrders.Count == 0)
                return;
            Order order;
            switch (_currentOrderGenerationConfiguration)
            {
                case OrderGenerationConfiguration.FromFile:
                    
                    if (_fromFileOrderSentCount < _fromFileOrderCount && _scrapedOrders[_fromFileOrderSentCount].OrderTime.CompareTo(TimeKeeper.CurrentDateTime) < 0)
                    {
                        order = _scrapedOrders[_fromFileOrderSentCount].ToSimOrder();
                        order.Areas = ConstructAreasVisited(order);
                        CostumerSendsOrderEvent?.Invoke(order);
                        //_sentlines.Add(order.OrderId.ToString());
                        //_scrapedOrders.RemoveAt(0);
                        //_sendOrderCount = 0;
                        
                        _fromFileOrderSentCount++;
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
            int orderId;
            do
            {
                orderId = Rand.Next(MinOrderNumberId, MaxOrderNumberId);
            } while (_usedOrderNumbers.Contains(orderId));
            
            _usedOrderNumbers.Add(orderId);
            Order order = new Order(orderId, TimeKeeper.CurrentDateTime, generatedLines);
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