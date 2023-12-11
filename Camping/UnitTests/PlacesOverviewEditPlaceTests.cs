using CampingCore;
using CampingCore.CampingRepositories;
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
            List<Place> places = PlacesOverviewPageFilterTests.CreatePlaces();
            Place place = places.First();
            camping.CampingRepository.CampingPlaceRepository.UpdatePlaceData(place, false, 11, 11, 11, false);
            mock.Verify(p => p.CampingPlaceRepository.UpdatePlaceData(place, false, 11, 11, 11, false), Times.Once());
        }

    }
}
