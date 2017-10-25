using System;
using System.Collections.Generic;
using System.Globalization;
using SolarSystem.Picking_and_ERP;

namespace SolarSystem
{
    public class Program
    {
        public static void Main()
        {
            ErpScrape scrape = new ErpScrape();
            
            List<Order> orders = scrape.LoadListFromFile();
        }
    }
}