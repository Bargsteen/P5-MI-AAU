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
			List<Line> _lineList = new List<Line>();
			
			// Retrive orders.
			List<String> _buffer = new List<string>();
			string _line;
			int _bufferLimit = 6;
			int _counter = 0;

			DateTime _date = new DateTime();
			int _orderNumber, _articleNumber, _quantity, _areaNumber = -1;
			
			while ((_line = stream.ReadLine()) != null)
			{
				_buffer.Insert(_counter % _bufferLimit, _line);
				_counter++;
				
				// For ordernumber and date-time
				if (_line.Contains("OrderNumber: 15"))
				{
					_lineList.Clear();
					
					// Retrieve ordernumber
					_orderNumber = int.Parse(_line.Substring(15, 6));
					
					// Retrieve data in buffer
					foreach (string bufferString in _buffer)
					{
						if (Regex.IsMatch(bufferString, "\\d-\\d-\\d"))
						{
							_date = new DateTime(int.Parse(bufferString.Substring(0, 4)), int.Parse(bufferString.Substring(5, 2)), int.Parse(bufferString.Substring(8, 2)), int.Parse(bufferString.Substring(11,2)), int.Parse(bufferString.Substring(14, 2)), int.Parse(bufferString.Substring(17,2)));
							break;
						}
					}
					
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
					Console.WriteLine(_orderList[0]);
				}
				
			}
			
			
			return _orderList;
		}
	}
}
// DT: 11 - :18
