using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SolarSystem.Picking_and_ERP
{
	public class ErpScrape
	{
		public List<Order> orders { get; private set; }

		public ErpScrape(string _path)
		{
			// Does file exist?
			try
			{
				orders = _scraperERPToOrderList(new StreamReader(_path));
			}
			catch (DirectoryNotFoundException e)
			{
				Console.WriteLine("File doesn't exist!\nException: " + e.Message);
			}
		}

		List<Order> _scraperERPToOrderList(StreamReader stream)
		{
			List<Order> _orderList = new List<Order>();
			
			// Retrive orders.
			List<String> _buffer = new List<string>();
			string _line;
			int _bufferLimit = 6;
			int _counter = 0;

			DateTime _date;
			int _ordernumber;
			
			while ((_line = stream.ReadLine()) != null)
			{
				_buffer.Insert(_counter % _bufferLimit, _line);
				_counter++;

				if (_line.Contains("OrderNumber: 15"))
				{
					// Retrieve ordernumber
					_ordernumber = int.Parse(_line.Substring(14, 2));
					Console.WriteLine(_ordernumber);
					
					// Retrieve data in buffer
					foreach (string bufferString in _buffer)
					{
						if (Regex.IsMatch(bufferString, "\\d-\\d-\\d"))
						{
							_date = new DateTime(int.Parse(bufferString.Substring(0, 4)), int.Parse(bufferString.Substring(5, 2)), int.Parse(bufferString.Substring(8, 2)), int.Parse(bufferString.Substring(11,2)), int.Parse(bufferString.Substring(14, 2)), int.Parse(bufferString.Substring(17,2)));
							Console.WriteLine(_date);
							break;
						}
					}
					
					//_orderList.Add(new Order(_ordernumber, new Line(new Article(),0, _date)));
					Console.WriteLine(_line);
				}
			}
			return _orderList;
		}
	}
}
// DT: 11 - :18
