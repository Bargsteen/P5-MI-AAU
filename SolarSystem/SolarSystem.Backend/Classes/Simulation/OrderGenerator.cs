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


        private List<Article> ArticleList { get; }
        private static readonly Random Rand = new Random();
        private double OrderChance { get; }

        private const int MinAmountOfLines = 1;
        private const int MaxAmountOfLines = 8;

        private const int MinArticleQuantity = 1;
        private const int MaxArticleQuantity = 5;

        private const int MinOrderNumberId = 10000000;
        private const int MaxOrderNumberId = 999999999;

        private int _sendOrderCount = 0;

        List<string> _sentlines = new List<string>();

        private readonly List<PickingAndErp.PickingOrder> _scrapedOrders;

        public OrderGenerator(List<Article> articleList, double orderChance, List<PickingAndErp.PickingOrder> scrapedOrders, OrderGenerationConfiguration conf)
        {

            ArticleList = articleList ?? throw new ArgumentNullException(nameof(articleList));

            OrderChance = orderChance;

            _scrapedOrders = scrapedOrders;

            _scrapedOrders.Sort((x, y) => x.OrderTime.CompareTo(y.OrderTime));

            _currentOrderGenerationConfiguration = conf;
        }

        public void Start()
        {
            TimeKeeper.Tick += MaybeSendOrder;
            //TimeKeeper.SimulationFinished += WriteOrdersOut;

        }

        //public void WriteOrdersOut()
        //{
        //    StreamWriter file = new StreamWriter("C:/New folder/Odfawdwadutput2.csv");
        //    foreach(string s in sentlines)
        //    {
        //        file.WriteLine(s);
        //    }
        //}


        public void MaybeSendOrder()
        {
            if (_scrapedOrders.Count == 0)
                return;
            Order order;
            switch (_currentOrderGenerationConfiguration)
            {
                case OrderGenerationConfiguration.FromFile:


                    if (_scrapedOrders[0].OrderTime.CompareTo(TimeKeeper.CurrentDateTime) < 0)
                    {
                        order = _scrapedOrders[0].ToSimOrder();
                        order.Areas = ConstructAreasVisited(order);
                        CostumerSendsOrderEvent?.Invoke(order);
                        _sentlines.Add(order.OrderId.ToString());
                        _scrapedOrders.RemoveAt(0);
                        _sendOrderCount = 0;
                    }

                    break;


                case OrderGenerationConfiguration.Random:
                    
                    double chance = Rand.NextDouble();
                    if (chance <= OrderChance)
                    {
                        _sendOrderCount = 0;
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
            Order order = new Order(Rand.Next(MinOrderNumberId, MaxOrderNumberId), TimeKeeper.CurrentDateTime, generatedLines);
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