﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using SolarSystem;

namespace SolarSystem.Test.Picking_and_ERP
{
    class PickingTest
    {
        [Data]

        [Theory]
        [InlineData(101010, 1, "01-01-1001", 10)]
        public void GetOrdersFromPickingTest_PremadeCSVFile_Returns(int ordernumber, int articlenumber, int datetime, int quantity)
        {
            
        }

    }
}
