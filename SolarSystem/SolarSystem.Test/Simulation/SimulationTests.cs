using System;

namespace SolarSystem.Test.Simulation
{
    public class SimulationTests : IDisposable
    {
    /*    private readonly IFixture _fixture;
        
        public SimulationTests()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
        }
        
        
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
            var order = _fixture.Build<Order>().With(o => o.TimeToFinish, 10).Create();
            
            // Act
            sut._Update(() => order);

            // Assert
            Assert.Equal(sut.BoxesInSystem.Count, 1);
        }

        [Fact]
        public void Update_BoxTime_GetsDecremented()
        {
            // Arrange
            Sim sut = new Sim(1);
            const int orderTime = 10;
            const int expectedTime = orderTime - 1;
            var order = _fixture.Build<Order>().With(o => o.TimeToFinish, 10).Create();
            
            // Act
            sut._Update(() => order);
            OrderBox orderBoxFromSystem = sut.BoxesInSystem.First();
            
            // Assert
            Assert.Equal(expectedTime, orderBoxFromSystem.TimeRemaining);
        }

        [Fact]
        public void Update_BoxFinishes_AddedToFinishedOrdersAndRemovedFromBoxesInSystem()
        {
            // Arrange
            Sim sut = new Sim(1);
            var order = _fixture.Build<Order>().With(o => o.TimeToFinish, 10).Create();
            
            // Act
            sut._Update(() => order);
            
            // Assert
            Assert.Equal(0, sut.BoxesInSystem.Count);
            Assert.Equal(1, sut.FinishedOrders.Count);
            
        }*/

        public void Dispose()
        {
            // Nothing to do
        }
        
    }
}