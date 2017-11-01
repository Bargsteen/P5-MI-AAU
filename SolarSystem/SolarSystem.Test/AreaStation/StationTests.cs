using Ploeh.AutoFixture;
using SolarSystem.Backend.Classes;
using Xunit;

namespace SolarSystem.Test.AreaStation
{
    public class StationTests
    {
        [Fact]
        public void ReceiveOrder_ShelfBoxWhenFull_ReturnsFullError()
        {
            // Arrange
            var fixture = new Fixture();
            var sut = fixture.Build<Station>().With(s => s.MaxShelfBoxes, 0).Create();
            var shelfBox = fixture.Create<ShelfBox>();
            // Act
            var result = sut.ReceiveBox(shelfBox);

            // Assert
            Assert.Equal(StationResult.FullError, result);
        }
    }
}
