using System;
using Xunit;
using SolarSystem;

namespace SolarSystem.Test
{
    public class Tests
    {
        [Fact]
        public void Test1()
        {
            Assert.True(true);
        }

        [Fact]
        public void Add_TwoPlusTwo_ReturnsFour()
        {
            // Arrange
            int num1 = 2;
            int num2 = 2;
            
            // Act
            int result = Program.SolarAdd(num1, num2);
            
            // Assert
            Assert.Equal(result, 4);
        }
    }
}