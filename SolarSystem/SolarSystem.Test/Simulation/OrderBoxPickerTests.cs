using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Ploeh.AutoFixture;
using SolarSystem.Classes;
using SolarSystem.Simulation;
using Xunit;

namespace SolarSystem.Test.Simulation
{
    public class OrderBoxPickerTests
    {
        private readonly IFixture _fixture;
        public OrderBoxPickerTests()
        {
            _fixture = new Fixture();
        }
        
        [Fact]
        public void GetNextOrder_UsingFIFO_ReturnsOrdersInFIFO()
        {
            // Arrange

            var firstOrderTime = DateTime.Today;
            
            Order firstOrder = _fixture.Build<Order>()
                .With(o => o.OrderTime, firstOrderTime)
                .Create();
            Order secondOrder = _fixture.Build<Order>()
                .With(o => o.OrderTime, firstOrderTime.AddDays(1))         
                .Create();
            
            var orderList = new List<Order> {firstOrder, secondOrder};

            // Act
            var res = OrderBoxPicker.GetNextOrder(OrderBoxPicker.PickingOrder.FirstInFirstOut, orderList);
            
            // Assert
            Assert.Equal(firstOrderTime, firstOrder.OrderTime);
        }

        [Fact]
        public void GetNextOrder_UsingLIFO_ReturnsOrdersInLIFO()
        {
            // Arrange
            
            var secondOrderTime = DateTime.Today;
            
            Order firstOrder = _fixture.Build<Order>()
                .With(o => o.OrderTime, secondOrderTime.AddDays(-1))
                .Create();
            Order secondOrder = _fixture.Build<Order>()
                .With(o => o.OrderTime, secondOrderTime)
                .Create();
            
            var orderList = new List<Order> {firstOrder, secondOrder};
            
            // Act
            var res = OrderBoxPicker.GetNextOrder(OrderBoxPicker.PickingOrder.LastInFirstOut, orderList);
            
            // Assert
            Assert.Equal(secondOrderTime, secondOrder.OrderTime);
            
        }
    }
}