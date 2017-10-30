using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSystem.Picking_and_ERP
{
	public class ErpScrape
	{
		public List<Order> orders;
		string path;

		public ErpScrape()
		{
			orders = new List<Order>();
			path = "/Users/Casper/Library/Projects/Uni/P5/wetransfer-f8286e/ErpTask_trace.log";

			// Does file exist?
			try
			{
				scraperERP(new StreamReader(path));
			}
			catch (DirectoryNotFoundException e)
			{
				Console.WriteLine("File doesn't exist!\nException: " + e.Message);
			}
		}

		void scraperERP(StreamReader stream)
		{
			// Path goes here
			string line;


			// Retrive orders.
			List<String> buffer = new List<string>();
			int bufferLimit = 6;
			int counter = 0;

			while ((line = stream.ReadLine()) != null)
			{
				buffer.Insert(counter % bufferLimit, line);
				counter++;

				if (line.Contains("OrderNumber: 15"))
				{
					foreach (string x in buffer)
					{
						if (x.Contains("2017"))
						{
							Console.WriteLine(x);
							break;
						}
					}
					Console.WriteLine(line);
				}
			}
		}
	}
}
