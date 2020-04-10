using NUnit.Framework;
using System;

namespace Monorail.GossipScript.Test
{
    [TestFixture]
    public class Test1
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void IsPrime_InputIs1_ReturnFalse()
        {
            var result = false;
            Assert.IsFalse(result, "1 should not be prime");
        }
    }
}
