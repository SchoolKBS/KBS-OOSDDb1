using CampingCore.CampingRepositories;
using CampingCore.PlacesOverviewPageClasses;
using CampingCore;
using CampingDataAccess;
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
        public void EditPlace_places_EditsPlace()
        {
            //Arrange
            var campingRepositoryMock = TestSupportClass.MockIcampingRepository();
            Camping camping = new Camping(campingRepositoryMock.Object);

            //Act
            camping.CampingRepository.CampingPlaceRepository.UpdatePlaceData(1, 11,11, false, 11, 11, 11, false);
            camping.UpdatePlace(1, 11,11, false, 11, 11, 11, false);

           //Assert
            campingRepositoryMock.Verify(p => p.CampingPlaceRepository.UpdatePlaceData(1, 11,11, false, 11, 11, 11, false));
        }
    }
}
