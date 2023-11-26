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
            CampingRepository campingRepository = new CampingRepository();
            Camping camping = new Camping(campingRepository);
            List<Place> listToCheck = new List<Place>();

            for (int i = 1; i <= 5; i++)
            {
                listToCheck.Add(new Place(i, true, i, i, i));
            }
            for (int i = 6; i <= 10; i++)
            {
                listToCheck.Add(new Place(i, false, i, i, i));
            }
            IEnumerable<Place> places = new List<Place>();
            camping.Places = listToCheck;
            places = listToCheck;
            places = PlacesOverviewPageSorting.SortColumnPrice(isSorted, camping.Places);
            Assert.That(places.First().PricePerNight, Is.EqualTo(number));
        }

        [TestCase(true, 10)]
        [TestCase(false, 1)]
        public void SortColumnPersonCount_boolAndInt_returnsList(bool isSorted, int number)
        {
            CampingRepository campingRepository = new CampingRepository();
            Camping camping = new Camping(campingRepository);
            List<Place> listToCheck = new List<Place>();

            for (int i = 1; i <= 5; i++)
            {
                listToCheck.Add(new Place(i, true, i, i, i));
            }
            for (int i = 6; i <= 10; i++)
            {
                listToCheck.Add(new Place(i, false, i, i, i));
            }
            IEnumerable<Place> places = new List<Place>();
            camping.Places = listToCheck;
            places = listToCheck;
            places = PlacesOverviewPageSorting.SortColumnPersonCount(isSorted, camping.Places);
            Assert.That(places.First().PricePerNight, Is.EqualTo(number));
        }

        [TestCase(true, 10)]
        [TestCase(false, 1)]
        public void SortColumnPlaceNumber_boolAndInt_returnsList(bool isSorted, int number)
        {
            CampingRepository campingRepository = new CampingRepository();
            Camping camping = new Camping(campingRepository);
            List<Place> listToCheck = new List<Place>();

            for (int i = 1; i <= 5; i++)
            {
                listToCheck.Add(new Place(i, true, i, i, i));
            }
            for (int i = 6; i <= 10; i++)
            {
                listToCheck.Add(new Place(i, false, i, i, i));
            }
            IEnumerable<Place> places = new List<Place>();
            camping.Places = listToCheck;
            places = listToCheck;
            places = PlacesOverviewPageSorting.SortColumnPlaceNumber(isSorted, camping.Places);
            Assert.That(places.First().PricePerNight, Is.EqualTo(number));
        }
    }
}
