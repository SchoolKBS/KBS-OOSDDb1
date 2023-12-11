using CampingCore;
using CampingCore.CampingRepositories;
using CampingCore.PlacesOverviewPageClasses;
using CampingDataAccess;
using Moq;
using System.Numerics;

namespace UnitTests
{
    public class PlacesOverviewPageFilterTests
    {
        [Test]
        public void GetFilteredListOnPower_places_returnsList()
        {
            Camping camping = new Camping(TestSupportClass.MockIcampingRepository().Object);
            IEnumerable<Place> places = new List<Place>();
            places = PlacesOverviewFilter.GetFilteredListOnPower(true, camping.Places, camping);
            Assert.That(places.Count(), Is.EqualTo(5));
        }

        [Test]
        public void GetFilteredListOnAmountOfPeople_places_returnsList()
        {
            Camping camping = new Camping(TestSupportClass.MockIcampingRepository().Object);
            IEnumerable<Place> places = new List<Place>();
            places = PlacesOverviewFilter.GetFilteredListOnAmountOfPeople(6, camping.Places, camping);
            Assert.That(places.Count(), Is.EqualTo(5));
        }
        [Test]
        public void GetFilteredListOnPrice_places_returnsList()
        {
            Camping camping = new Camping(TestSupportClass.MockIcampingRepository().Object);
            IEnumerable<Place> places = new List<Place>();
            places = PlacesOverviewFilter.GetFilteredListOnPrice(5.5, camping.Places, camping);
            Assert.That(places.Count(), Is.EqualTo(5));
        }

        [Test]
        public void GetFilteredListOnDates_places_returnsList()
        {
            Camping camping = new Camping(TestSupportClass.MockIcampingRepository().Object);
            IEnumerable<Place> places = new List<Place>();
            places = PlacesOverviewFilter.GetFilteredListOnDate(false, DateTime.Now.Date.AddDays(1), DateTime.Now.Date.AddDays(5), camping.Places, camping);
            Assert.That(places.Count(), Is.EqualTo(5));
        }

        [Test]
        public void GetAvailablePlacesBetweenDates_places_returnsList()
        {
            Camping camping = new Camping(TestSupportClass.MockIcampingRepository().Object);

            IEnumerable<Place> places = new List<Place>();
            places = PlacesOverviewFilter.GetAvailablePlacesBetweenDates(DateTime.Now.Date.AddDays(1), DateTime.Now.Date.AddDays(5), camping);
            Assert.That(places.Count(), Is.EqualTo(5));
        }
    }
}