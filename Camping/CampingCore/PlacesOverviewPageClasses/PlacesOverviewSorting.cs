using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore.PlacesOverviewPageClasses
{
    public class PlacesOverviewSorting
    {
        // Function to sort the list on placenumbers 
        // Returns a bool to know which way the list is sorted now
        public static IEnumerable<Place> SortColumnPlaceID(bool isSorted, IEnumerable<Place> _placesSortedAndOrFiltered)
        {
            if (isSorted) _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderByDescending(i => i.PlaceID).Select(i => i);
            else _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderBy(i => i.PlaceID).Select(i => i);
            return _placesSortedAndOrFiltered;
        }

        // Function to sort the list on price 
        // Returns a bool to know which way the list is sorted now
        public static IEnumerable<Place> SortColumnPrice(bool isSorted, IEnumerable<Place> _placesSortedAndOrFiltered)
        {
            if (isSorted) _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderByDescending(i => i.PricePerNightPerPerson).Select(i => i);
            else _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderBy(i => i.PricePerNightPerPerson).Select(i => i);
            return _placesSortedAndOrFiltered;
        }

        // Function to sort the list on amount of possible people on a place 
        // Returns a bool to know which way the list is sorted now
        public static IEnumerable<Place> SortColumnAmountOfPeople(bool isSorted, IEnumerable<Place> _placesSortedAndOrFiltered)
        {
            if (isSorted) _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderByDescending(i => i.AmountOfPeople).Select(i => i);
            else _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderBy(i => i.AmountOfPeople).Select(i => i);
            return _placesSortedAndOrFiltered;
        }

        public static IEnumerable<Place> SetSortDuringFiltering(bool _isSortedAscending, string _headerTag, IEnumerable<Place> _placesSortedAndOrFiltered)
        {
            if (_isSortedAscending)
            {
                if (_headerTag.Equals("PlaceID"))
                    _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderBy(i => i.PlaceID).Select(i => i);
                else if (_headerTag.Equals("Price"))
                    _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderBy(i => i.PricePerNightPerPerson).Select(i => i);
                else
                    _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderBy(i => i.AmountOfPeople).Select(i => i);
            }
            else
            {
                if (_headerTag.Equals("PlaceID"))
                    _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderByDescending(i => i.PlaceID).Select(i => i);
                else if (_headerTag.Equals("Price"))
                    _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderByDescending(i => i.PricePerNightPerPerson).Select(i => i);
                else
                    _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.OrderByDescending(i => i.AmountOfPeople).Select(i => i);
            }
            return _placesSortedAndOrFiltered;
        }
    }
}
