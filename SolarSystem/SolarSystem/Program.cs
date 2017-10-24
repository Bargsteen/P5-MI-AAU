using System.Collections.Generic;
using SolarSystem.Picking_and_ERP;

namespace SolarSystem
{
    public class Program
    {
        public static void Main()
        {
         
            ErpScrape scrape = new ErpScrape("/Users/Casper/Library/Projects/Uni/P5/wetransfer-f8286e/ErpTask_trace.log");
            List<Order> orderList = scrape.orders;
        }
    }
}