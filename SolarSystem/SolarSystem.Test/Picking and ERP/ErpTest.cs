using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SolarSystem.Picking_and_ERP;
using Xunit;

namespace SolarSystem.Test.Picking_and_ERP
{
	public class ErpTest
	{
		[Fact]
		public void Test_Return_List_nullOrEmpty()
		{
			// Arrange
			ErpScrape erpScrape = new ErpScrape();
			List<Order> orderList;
			
			// Act
			orderList = erpScrape.orders;
			
			// Assert
			Assert.NotEmpty(orderList);
		}
	}
}
