using CampingCore;
using CampingCore.PlacesOverviewPageClasses;
using CampingDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    public class PlacesOverviewPageSortingTests
    {
        [TestCase(true, 10)]
        [TestCase(false, 1)]
        public void SortColumnPrice_boolAndInt_returnsList(bool isSorted, int number)
        {
            //Arrange
            Camping camping = new Camping(TestSupportClass.MockIcampingRepository().Object);

            //Act
            IEnumerable<Place> places = PlacesOverviewSorting.SortColumnPrice(isSorted, camping.GetPlaces());

            //Assert
            Assert.That(places.First().PricePerNightPerPerson, Is.EqualTo(number));
        }

        [TestCase(true, 10)]
        [TestCase(false, 1)]
        public void SortColumnAmountOfPeople_boolAndInt_returnsList(bool isSorted, int number)
        {
            //Arrange
            Camping camping = new Camping(TestSupportClass.MockIcampingRepository().Object);

            //Act
            IEnumerable<Place>places = PlacesOverviewSorting.SortColumnAmountOfPeople(isSorted, camping.GetPlaces());

            //Assert
            Assert.That(places.First().PricePerNightPerPerson, Is.EqualTo(number));
        }

        [TestCase(true, 10)]
        [TestCase(false, 1)]
        public void SortColumnPlaceID_boolAndInt_returnsList(bool isSorted, int number)
        {
            //Arrange
            Camping camping = new Camping(TestSupportClass.MockIcampingRepository().Object);

            //Act
            IEnumerable<Place> places = PlacesOverviewSorting.SortColumnPlaceID(isSorted, camping.GetPlaces());

            //Assert
            Assert.That(places.First().PricePerNightPerPerson, Is.EqualTo(number));
        }
    }
}
