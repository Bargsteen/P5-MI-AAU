using SolarSystem.Simulation;
using Xunit;
using Moq;

namespace SolarSystem.Test.Simulation
{
    public class SimulationTests
    {
        [Fact]
        public void Run_GivenATimeUnit_ReturnsString()
        {
            // Arrange
            ISim sim = new Sim();
            
            // Act
            var result = sim.Run(10u);
            
            // Assert
            Assert.IsType<string>(result);
        }
    }
}