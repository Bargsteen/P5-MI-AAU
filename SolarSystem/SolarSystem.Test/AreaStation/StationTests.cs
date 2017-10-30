using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ploeh.AutoFixture;
using Xunit;
using Ploeh.AutoFixture.AutoMoq;
using SolarSystem.Classes;

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
