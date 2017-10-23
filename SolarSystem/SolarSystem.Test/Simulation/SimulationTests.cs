using System;
using System.Collections.Generic;
using System.Reflection;
using BetterPrivateObject;
using Castle.DynamicProxy.Generators.Emitters;
using Moq;
using SolarSystem.Classes;
using SolarSystem.Simulation;
using Xunit;

namespace SolarSystem.Test.Simulation
{
    public class SimulationTests
    {
        [Fact]
        public void Run_GivenATimeUnit_ReturnsString()
        {
           Assert.False(true);
           
        }

        [Fact]
        public void GetNextOrder_OnRun_ReturnsOrderWithValidName()
        {
            // Arrange
            dynamic simPo = new PrivateType<Sim>();
            
            // Act
            dynamic res = simPo.GetNextOrder();
            
            // Assert
            Assert.True(res.OrderName[0] >= 'A' && res.OrderName[0] <= 'Z');
            
        }

        [Fact]
        public void Update_HasZeroBoxesAndOneMaxConcurrent_AddsBox()
        {
            // Arrange
            Sim sut = new Sim(1);
            
            // Act
            sut._Update(() => new Order("OrderName", 10));

            // Assert
            Assert.Equal(sut.BoxesInSystem.Count, 1);
        }
    }
}