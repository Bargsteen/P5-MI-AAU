using System;
using System.Collections.Generic;
using SolarSystem.Classes;
using Xunit;

namespace SolarSystem.Test.Simulation
{
    public class OrderboxProgressContainerTest
    {
        [Theory]
        public void AddFunctionTest()
        {
            OrderboxProgressContainer container = new OrderboxProgressContainer();
            
            List<Line> lineList = new List<Line>();
            lineList.Add(new Line(new ItemType("Item1"), 10));
            
            OrderBox orderbox = new OrderBox(new Order("Order1", 10, new DateTime(), lineList));
            
            container.AddOrderBoxProgress(new OrderBoxProgress(orderbox, new DateTime(), 10));
            
            Assert.True(container.GetNext() is OrderBoxProgress);
        }
        
    }
}