using BetterPrivateObject;
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
    }
}