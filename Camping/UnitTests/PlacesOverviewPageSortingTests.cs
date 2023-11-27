using CampingCore;
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
        /*[TestCase(true, 1)]
        [TestCase(false, 10)]
        public void SortColumnPrice_boolAndInt_returnsList(bool isSorted, int number)
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
            places = PlacesOverviewPageSorting.SortColumnPrice(isSorted, camping.Places);
            Assert.That(places.First().PricePerNight, Is.EqualTo(number));
        }

        [TestCase(true, 1)]
        [TestCase(false, 10)]
        public void SortColumnPersonCount_boolAndInt_returnsList(bool isSorted, int number)
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
            places = PlacesOverviewPageSorting.SortColumnPersonCount(isSorted, camping.Places);
            Assert.That(places.First().PricePerNight, Is.EqualTo(number));
        }

        [TestCase(true, 1)]
        [TestCase(false, 10)]
        public void SortColumnPlaceNumber_boolAndInt_returnsList(bool isSorted, int number)
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
            places = PlacesOverviewPageSorting.SortColumnPlaceNumber(isSorted, camping.Places);
            Assert.That(places.First().PricePerNight, Is.EqualTo(number));
        }*/
    }
}
