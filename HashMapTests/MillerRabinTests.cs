using Microsoft.VisualStudio.TestTools.UnitTesting;
using HashMap.Helper;

namespace HashMapTests
{
    [TestClass]

    public class MillerRabinTests
    {
        [TestMethod]
        public void TestForSmallPromalityNumbers()
        {
            Assert.IsTrue(MillerRabin.IsSimplyNumber(1));
            Assert.IsTrue(MillerRabin.IsSimplyNumber(3));
            Assert.IsTrue(MillerRabin.IsSimplyNumber(5));
        }

        [TestMethod]
        public void TestForMediumPromalityNumbers()
        {
            Assert.IsTrue(MillerRabin.IsSimplyNumber(45137));
            Assert.IsTrue(MillerRabin.IsSimplyNumber(46147));
            Assert.IsTrue(MillerRabin.IsSimplyNumber(60373));
        }

        [TestMethod]
        public void TestForLargePromalityNumbers()
        {
            Assert.IsTrue(MillerRabin.IsSimplyNumber(1046527));
            Assert.IsTrue(MillerRabin.IsSimplyNumber(1073676287));
        }

        [TestMethod]
        public void TestForSmallNotPromalityNumbers()
        {
            Assert.IsFalse(MillerRabin.IsSimplyNumber(4));
            Assert.IsFalse(MillerRabin.IsSimplyNumber(15));
            Assert.IsFalse(MillerRabin.IsSimplyNumber(21));
        }

        [TestMethod]
        public void TestForMediumNotPromalityNumbers()
        {
            Assert.IsFalse(MillerRabin.IsSimplyNumber(221));
            Assert.IsFalse(MillerRabin.IsSimplyNumber(421321));
            Assert.IsFalse(MillerRabin.IsSimplyNumber(781323));
        }

        [TestMethod]
        public void TestForLargeNotPromalityNumbers()
        {
            Assert.IsFalse(MillerRabin.IsSimplyNumber(2114782963));
            Assert.IsFalse(MillerRabin.IsSimplyNumber(1114781963));
        }
    }
}
