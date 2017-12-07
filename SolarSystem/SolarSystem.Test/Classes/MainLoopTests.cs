using Ploeh.AutoFixture;
using SolarSystem.Backend.Classes;
using SolarSystem.Backend.Classes.Simulation;
using Xunit;

namespace SolarSystem.Test.Classes
{
    public class MainLoopTests
    {
        [Fact]
        public void _CheckAndSend_LoopHasABoxWithTimeZero_InvokesOrderBoxFinishedEvent()
        {
            // Arrange
            var fixture = new Fixture();
            var orderBoxProgressContainer = fixture.Create<OrderboxProgressContainer>();
            var orderBoxProgress = fixture.Build<OrderBoxProgress>().With(o => o.SecondsToSpend, 0).Create();         
            orderBoxProgressContainer.AddOrderBoxProgress(orderBoxProgress);

            fixture.Freeze(orderBoxProgressContainer);
            
            var sut = fixture.Create<MainLoop>();
            
            // Act
            sut._CheckAndSend();

            // Assert
        }
    }
}