using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/*
 * Make ErpScrape object.
 * Use ScrapeErp(filepath)
 * Use SaveToFile()
 * Load from file with LoadListFromFile() : List<Order>
 */

namespace SolarSystem.Picking_and_ERP
{
	public class ErpScrape
	{
		public List<Order> orders { get; private set; }

		
		
		public ErpScrape()
		{
			orders = new List<Order>();
		}
		
		public void SaveToFile()
		{
			string _path = AppDomain.CurrentDomain.BaseDirectory + "SaveFiles/";
			string _fileName = "Erp.dat";
			
			Directory.CreateDirectory(_path);

			if (orders != null)
			{
				using (Stream stream = File.Open(_path + _fileName, FileMode.Create))
				{
					var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
					formatter.Serialize(stream, orders);
				}
			}
			else
			{
				Console.WriteLine("Call ScrapeErp(string path) first");
			}
		}

		public List<Order> LoadListFromFile()
		{
			List<Order> _orders;
			
			string _path = AppDomain.CurrentDomain.BaseDirectory + "SaveFiles/";
			string _fileName = "Erp.dat";
			
			using (Stream stream = File.Open(_path + _fileName, FileMode.Open))
			{
				var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
				_orders = (List<Order>) formatter.Deserialize(stream);
			}

			return _orders;
		}
		
		public void ScrapeErp(string _path)
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
			List<Line> _lineList = new List<Line>();
			
			// Retrive orders.
			List<String> _buffer = new List<string>();
			string _line;
			int _bufferLimit = 6;
			int _counter = 0;

			DateTime _date = new DateTime();
			int _orderNumber = 0, _articleNumber = 0, _quantity = 0, _areaNumber = -1;
			
			while ((_line = stream.ReadLine()) != null)
			{
				
				_buffer.Add(_line);
				
				_counter++;
				
				// For ordernumber and date-time
				if (_line.Contains("OrderNumber: 15"))
				{
					// Clear the lines from the previous orders.
					_lineList.Clear();
					
					// Retrieve ordernumber
					_orderNumber = int.Parse(_line.Substring(15, 6));
					
					
					// Retrieve data in buffer (Get the date and time)
					string bufferString = _buffer[_counter - 6];
					_date = new DateTime(int.Parse(bufferString.Substring(0, 4)), int.Parse(bufferString.Substring(5, 2)), int.Parse(bufferString.Substring(8, 2)), int.Parse(bufferString.Substring(11,2)), int.Parse(bufferString.Substring(14, 2)), int.Parse(bufferString.Substring(17,2)));

					
					// Reset buffer and counter
					_buffer.Clear();
					_counter = 0;
					
					// skip 6 lines to get to articlenumber.
					_line = stream.ReadLine();
					_line = stream.ReadLine();
					_line = stream.ReadLine();
					_line = stream.ReadLine();
					_line = stream.ReadLine();
					_line = stream.ReadLine();

					while (_line.Contains("ArticleNumber: "))
					{
						// Read ArticleNumber
						_articleNumber = int.Parse(_line.Substring(15, 9));
				
				
						// Skip 3 lines
						_line = stream.ReadLine();
						_line = stream.ReadLine();
						_line = stream.ReadLine();
				
						// Read Quantity
						_quantity = int.Parse(_line.Substring(9));
				
						// Add line to lineList
						_lineList.Add(new Line(new Article(_articleNumber, _areaNumber), _quantity, _date));

				
						// Skip 4 times to see if there is any more lines for this order
						_line = stream.ReadLine();
						_line = stream.ReadLine();
						_line = stream.ReadLine();
						_line = stream.ReadLine();
						
					}
					
					// Order done. Add to list
					_orderList.Add(new Order(_orderNumber, _lineList));
				}
			
			}
			
			return _orderList;

		}
	}
}
// DT: 11 - :18
