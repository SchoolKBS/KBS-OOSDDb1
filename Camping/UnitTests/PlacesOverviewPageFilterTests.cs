using CampingCore;
using CampingCore.PlacesOverviewPageClasses;
using CampingDataAccess;
using System.Numerics;

namespace UnitTests
{
    public class PlacesOverviewPageFilterTests
    {
        //[Test]
        /*public void GetFilteredListOnPower_places_returnsList()
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
        }*/

        /*[Test]
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
        }*/
        [Test]
        public void GetFilteredListOnPrice_places_returnsList()
        {
            
            CampingRepository campingRepository = new CampingRepository();
            Camping camping = new Camping(campingRepository);
            Console.WriteLine(camping.Places);
            IEnumerable<Place> places = new List<Place>();
            places = camping.Places;
            if (places.First().Equals(camping.Places.First()))
            {
                Console.WriteLine("ja");
            }
            places = PlacesOverviewPageFilter.GetFilteredListOnPrice(5.5, camping.Places, camping);
            if (places.Count() == 0)
            {
                Console.WriteLine("leeg");
            }
            else
            {
                foreach (Place place in places)
                {
                    Console.WriteLine(place);
                }
            }
            /*camping.Places = new List<Place>();
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
            Assert.That(places.Count(), Is.EqualTo(5));*/
        }

        /*[Test]
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
        }*/
    }
}