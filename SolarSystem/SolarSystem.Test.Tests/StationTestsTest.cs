// <copyright file="StationTestsTest.cs">Copyright ©  2017</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolarSystem.Test.AreaStation;

namespace SolarSystem.Test.AreaStation.Tests
{
    [TestClass]
    [PexClass(typeof(StationTests))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class StationTestsTest
    {

        [PexMethod(MaxBranches = 20000)]
        public void ReceiveOrder_OrderBoxWhenFull_ReturnsFullError([PexAssumeUnderTest]StationTests target)
        {
            target.ReceiveOrder_OrderBoxWhenFull_ReturnsFullError();
            // TODO: add assertions to method StationTestsTest.ReceiveOrder_OrderBoxWhenFull_ReturnsFullError(StationTests)
        }
    }
}
