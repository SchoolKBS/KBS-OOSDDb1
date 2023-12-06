using CampingCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public class PlacesOverviewEditPlaceTests
    {
        [Test]
        public void test1()
        {
            var mock = new Mock<ICampingRepository>();
            Camping camping = new Camping(mock.Object);
            Assert.Pass();
        }
    }
}
