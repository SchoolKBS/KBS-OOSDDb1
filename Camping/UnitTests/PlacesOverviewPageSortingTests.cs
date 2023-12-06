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
                listToCheck.Add(new Place(i, true, i, true, i, i, i, i, i));
            }
            for (int i = 6; i <= 10; i++)
            {
                listToCheck.Add(new Place(i, false, i, false, i, i, i, i, i));
            }
            IEnumerable<Place> places = new List<Place>();
            camping.Places = listToCheck;
            places = listToCheck;
            places = PlacesOverviewSorting.SortColumnPrice(isSorted, camping.Places);
            Assert.That(places.First().PricePerNightPerPerson, Is.EqualTo(number));
        }

        [TestCase(true, 10)]
        [TestCase(false, 1)]
        public void SortColumnAmountOfPeople_boolAndInt_returnsList(bool isSorted, int number)
        {
            CampingRepository campingRepository = new CampingRepository();
            Camping camping = new Camping(campingRepository);
            List<Place> listToCheck = new List<Place>();

            for (int i = 1; i <= 5; i++)
            {
                listToCheck.Add(new Place(i, true, i, true, i, i, i, i ,i));
            }
            for (int i = 6; i <= 10; i++)
            {
                listToCheck.Add(new Place(i, false, i, false, i, i, i, i, i));
            }
            IEnumerable<Place> places = new List<Place>();
            camping.Places = listToCheck;
            places = listToCheck;
            places = PlacesOverviewSorting.SortColumnAmountOfPeople(isSorted, camping.Places);
            Assert.That(places.First().PricePerNightPerPerson, Is.EqualTo(number));
        }

        [TestCase(true, 10)]
        [TestCase(false, 1)]
        public void SortColumnPlaceID_boolAndInt_returnsList(bool isSorted, int number)
        {
            CampingRepository campingRepository = new CampingRepository();
            Camping camping = new Camping(campingRepository);
            List<Place> listToCheck = new List<Place>();

            for (int i = 1; i <= 5; i++)
            {
                listToCheck.Add(new Place(i, true, i, true, i, i, i, i, i));
            }
            for (int i = 6; i <= 10; i++)
            {
                listToCheck.Add(new Place(i, false, i, false, i, i, i, i, i));
            }
            IEnumerable<Place> places = new List<Place>();
            camping.Places = listToCheck;
            places = listToCheck;
            places = PlacesOverviewSorting.SortColumnPlaceID(isSorted, camping.Places);
            Assert.That(places.First().PricePerNightPerPerson, Is.EqualTo(number));
        }
    }
}
