using CampingCore;
using CampingCore.PlacesOverviewPageClasses;
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
            var mock = new Mock<ICampingRepository>();
            Camping camping = new Camping(mock.Object);
            Place place = new Place(1, true, 1, 1, 1);
            PlacesOverviewPageDelete.DeletePlace(camping, place, DateTime.Now.Date);
            mock.Verify(p => p.RemovePlace(place), Times.Once());
            mock.Verify(p => p.RemoveAllPreviousReservationsByPlace(place, DateTime.Now.Date), Times.Once());
        }
    }
}
