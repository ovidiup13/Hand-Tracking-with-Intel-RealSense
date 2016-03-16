using System;
using CameraModule.Interfaces.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CameraModuleTests.Interfaces.Module
{
    [TestClass()]
    public class TrackingTests
    {
        [TestMethod()]
        public void GetDistanceTest()
        {
            //arrange
            var point1 = new PXCMPoint3DF32(0, 0, 0);
            var point2 = new PXCMPoint3DF32(0, 0, 0);

            //act
            var distance = Tracking.GetDistance(point1, point2);

            //assert
            Assert.AreEqual(0 - Tracking.Offset, distance);
        }

        [TestMethod()]
        public void GetDistanceTest2()
        {
            //arrange
            var point1 = new PXCMPoint3DF32(0, 0, 0);
            var point2 = new PXCMPoint3DF32(1, 1, 1);

            //act
            var distance = Tracking.GetDistance(point1, point2);

            //assert
            Assert.AreEqual(Math.Sqrt(3) - Tracking.Offset, distance);
        }

        [TestMethod()]
        public void GetDistanceTest3()
        {
            //arrange
            var point1 = new PXCMPoint3DF32(5, 4, -2);
            var point2 = new PXCMPoint3DF32(1, 1, 1);

            //act
            var distance = Tracking.GetDistance(point1, point2);

            //assert
            Assert.AreEqual(Math.Sqrt(34) - Tracking.Offset, distance);
        }
    }
}