using System.Collections.Generic;
using System.Linq;
using Ploeh.AutoFixture;
using SolarSystem.Backend.Classes.Simulation;
using Xunit;

namespace SolarSystem.Test.AreaStation
{
    public class StationTests
    {
        [Fact]
        public void MaybeEvictShelfBoxes_HasUnNeededShelfBox_EvictsIt()
        {
            // Arrange
            var fixture = new Fixture();
            
            var article1 = fixture.Create<Article>();
            var article2 = fixture.Create<Article>();
            var article3 = fixture.Create<Article>();

            var shelfBoxes = new List<ShelfBox>
            {
                new ShelfBox(new Line(article1, 100)),
                new ShelfBox(new Line(article2, 100)),
                new ShelfBox(new Line(article3, 100))
            };
            
            var orderBoxes = new List<OrderBox>
            {
                fixture.Build<OrderBox>().With(o => o.LineIsPickedStatuses, new Dictionary<Line, bool>
                {
                    {fixture.Build<Line>().With(l => l.Article, article1 ).Create(), false},

                }).Create(),
                
                fixture.Build<OrderBox>().With(o => o.LineIsPickedStatuses, new Dictionary<Line, bool>
                {
                    {fixture.Build<Line>().With(l => l.Article, article2).Create(), false},

                }).Create()
            };
            
            var sut = fixture.Create<Station>();
            sut.ShelfBoxes = shelfBoxes;
            sut.OrderBoxes = orderBoxes;
            
            // Act
            sut.MaybeEvictShelfBoxes();
            
            // Assert
            Assert.Equal(2, sut.ShelfBoxes.Count);
            Assert.True(sut.ShelfBoxes.Any(s => s.Line.Article == article1));
            Assert.True(sut.ShelfBoxes.Any(s => s.Line.Article == article2));
            Assert.False(sut.ShelfBoxes.Any(s => s.Line.Article == article3));
            
        }
        
        [Fact]
        public void MaybeEvictShelfBoxes_OnlyHasNeededShelfBox_EvictsNone()
        {
            // Arrange
            var fixture = new Fixture();
            
            var article1 = fixture.Create<Article>();
            var article2 = fixture.Create<Article>();
            var article3 = fixture.Create<Article>();

            var shelfBoxes = new List<ShelfBox>
            {
                new ShelfBox(new Line(article1, 100)),
                new ShelfBox(new Line(article2, 100)),
                new ShelfBox(new Line(article3, 100))
            };
            
            var orderBoxes = new List<OrderBox>
            {
                fixture.Build<OrderBox>().With(o => o.LineIsPickedStatuses, new Dictionary<Line, bool>
                {
                    {fixture.Build<Line>().With(l => l.Article, article1 ).Create(), false},

                }).Create(),
                
                fixture.Build<OrderBox>().With(o => o.LineIsPickedStatuses, new Dictionary<Line, bool>
                {
                    {fixture.Build<Line>().With(l => l.Article, article2).Create(), false},

                }).Create(),
                
                fixture.Build<OrderBox>().With(o => o.LineIsPickedStatuses, new Dictionary<Line, bool>
                {
                    {fixture.Build<Line>().With(l => l.Article, article3).Create(), false},

                }).Create()
            };
            
            var sut = fixture.Create<Station>();
            sut.ShelfBoxes = shelfBoxes;
            sut.OrderBoxes = orderBoxes;
            
            // Act
            sut.MaybeEvictShelfBoxes();
            
            // Assert
            Assert.Equal(3, sut.ShelfBoxes.Count);
            Assert.True(sut.ShelfBoxes.Any(s => s.Line.Article == article1));
            Assert.True(sut.ShelfBoxes.Any(s => s.Line.Article == article2));
            Assert.True(sut.ShelfBoxes.Any(s => s.Line.Article == article3));
            
        }
    }
}
