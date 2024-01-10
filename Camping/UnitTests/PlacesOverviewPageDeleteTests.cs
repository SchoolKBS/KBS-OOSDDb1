using CampingCore;
using CampingCore.CampingRepositories;
using CampingCore.PlacesOverviewPageClasses;
using CampingDataAccess;
using Microsoft.IdentityModel.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public class PlacesOverviewPageDeleteTests
    {
        [Test]
        public void DeletePlace_places_DeletesPlaceAndReservations()
        {
            //Arrange
            var campingRepositoryMock = TestSupportClass.MockIcampingRepository();
            Camping camping = new Camping(campingRepositoryMock.Object);
            Place place = new Place(1, true, 1, 1, true, 1, 1, 1, 1, 1);

            //Act
            PlacesOverviewDelete.DeletePlace(camping, place, DateTime.Now.Date);

            //Assert
            campingRepositoryMock.Verify(p => p.CampingPlaceRepository.RemovePlace(place), Times.Once());
            campingRepositoryMock.Verify(p => p.CampingReservationRepository.RemoveAllPreviousReservationsByPlace(place, DateTime.Now.Date), Times.Once());
        }
    }
}
