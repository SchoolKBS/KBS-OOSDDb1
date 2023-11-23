using CampingCore;
namespace UnitTests
{
    public class PlacesOverviewPageFilterTests
    {
        [Test]
        public void GetFilteredListOnPower_places_returnsList()
        {
            Camping camping = new Camping();
            camping.Places = new List<Place>();
            for (int i = 1; i <= 5; i++)
            {
                camping.Places.Add(new Place(i, true, i, i, i));
            }
            for (int i = 6; i <= 10; i++)
            {
                camping.Places.Add(new Place(i, false, i, i, i));
            }
            IEnumerable<Place> places = new List<Place>();
            places = PlacesOverviewPageFilter.GetFilteredListOnPower(true, camping.Places, camping);
            Assert.That(places.Count(), Is.EqualTo(5));
        }

        [Test]
        public void GetFilteredListOnPersonCount_places_returnsList()
        {
            Camping camping = new Camping();
            camping.Places = new List<Place>();
            for (int i = 1; i <= 5; i++)
            {
                camping.Places.Add(new Place(i, true, i, i, i));
            }
            for (int i = 6; i <= 10; i++)
            {
                camping.Places.Add(new Place(i, false, i, i, i));
            }
            IEnumerable<Place> places = new List<Place>();
            places = PlacesOverviewPageFilter.GetFilteredListOnPersonCount(6, camping.Places, camping);
            Assert.That(places.Count(), Is.EqualTo(5));
        }
        [Test]
        public void GetFilteredListOnPrice_places_returnsList()
        {
            Camping camping = new Camping();
            camping.Places = new List<Place>();
            for (int i = 1; i <= 5; i++)
            {
                camping.Places.Add(new Place(i, true, i, i, i));
            }
            for (int i = 6; i <= 10; i++)
            {
                camping.Places.Add(new Place(i, false, i, i, i));
            }
            IEnumerable<Place> places = new List<Place>();
            places = PlacesOverviewPageFilter.GetFilteredListOnPrice(5.5, camping.Places, camping);
            Assert.That(places.Count(), Is.EqualTo(5));
        }

        [Test]
        public void GetFilteredListOnDates_places_returnsList()
        {
            Camping camping = new Camping();
            camping.Places = new List<Place>();
            for (int i = 1; i <= 5; i++)
            {
                camping.Places.Add(new Place(i, true, i, i, i));
            }
            for (int i = 6; i <= 10; i++)
            {
                camping.Places.Add(new Place(i, false, i, i, i));
            }
            IEnumerable<Place> places = new List<Place>();
            places = PlacesOverviewPageFilter.GetFilteredListOnDate(DateTime.Now.Date.AddDays(1), DateTime.Now.Date.AddDays(5), camping.Places, camping);
            Assert.That(places.Count(), Is.EqualTo(5));
        }

        [Test]
        public void GetAvailablePlacesBetweenDates_places_returnsList()
        {
            Camping camping = new Camping();
            camping.Places = new List<Place>();
            for (int i = 1; i <= 5; i++)
            {
                camping.Places.Add(new Place(i, true, i, i, i));
            }
            for (int i = 6; i <= 10; i++)
            {
                camping.Places.Add(new Place(i, false, i, i, i));
            }
            IEnumerable<Place> places = new List<Place>();
            places = PlacesOverviewPageFilter.GetAvailablePlacesBetweenDates(DateTime.Now.Date.AddDays(1), DateTime.Now.Date.AddDays(5), camping);
            Assert.That(places.Count(), Is.EqualTo(5));
        }
    }
}