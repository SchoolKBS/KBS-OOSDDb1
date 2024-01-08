using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore.PlacesOverviewPageClasses
{
    public class PlacesOverviewFilter
    {

        public static IEnumerable<Place> GetFilteredListOnDogs(bool? dogsAllowed, IEnumerable<Place> _placesSortedAndOrFiltered, Camping _camping)
        {
            if (dogsAllowed != null)
            {
                _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.Intersect(_camping.GetPlaces().Where(i => i.Dogs == dogsAllowed).Select(i => i));
            }
            return _placesSortedAndOrFiltered;
        }
        // Function to filter the list of places on the bool hasPower
        public static IEnumerable<Place> GetFilteredListOnPower(bool? hasPower, IEnumerable<Place> _placesSortedAndOrFiltered, Camping _camping)
        {
            if (hasPower != null)
            {
                _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.Intersect(_camping.GetPlaces().Where(i => i.Power == hasPower).Select(i => i));
            }
            return _placesSortedAndOrFiltered;
        }

        // Function to filter the list of places on the integer maxPriceRange 
        public static IEnumerable<Place> GetFilteredListOnPrice(double maxPriceRange, IEnumerable<Place> _placesSortedAndOrFiltered, Camping _camping)
        {

            if (maxPriceRange >= _placesSortedAndOrFiltered.Min(i => i.PricePerNightPerPerson))
            {
                _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.Intersect(_camping.GetPlaces().Where(i => i.PricePerNightPerPerson <= maxPriceRange).Select(i => i));
            }
            return _placesSortedAndOrFiltered;

        }

        // Function to filter the list of places on the int AmountOfPeople
        public static IEnumerable<Place> GetFilteredListOnAmountOfPeople(int amountOfPeople, IEnumerable<Place> _placesSortedAndOrFiltered, Camping _camping)
        {
            if (amountOfPeople >= 0)
            {
                _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.Intersect(_camping.GetPlaces().Where(i => i.AmountOfPeople >= amountOfPeople).Select(i => i));
            }
            return _placesSortedAndOrFiltered;
        }

        // Function to filter the list of places on the arrival and departure date
        public static IEnumerable<Place> GetFilteredListOnDate(bool emptyDates, DateTime arrivalDate, DateTime departureDate, IEnumerable<Place> _placesSortedAndOrFiltered, Camping _camping)
        {
            if (arrivalDate.Date < departureDate.Date && arrivalDate.Date >= DateTime.Now.Date && !emptyDates)
            {
                _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.Intersect(GetAvailablePlacesBetweenDates(arrivalDate.Date, departureDate.Date, _camping));
            }
            return _placesSortedAndOrFiltered;
        }

        // Function to get the places available between the arrival and departure date
        // Returns an IEnumerable<Place>
        public static IEnumerable<Place> GetAvailablePlacesBetweenDates(DateTime arrivalDate, DateTime departureDate, Camping _camping)
        {
            List<Place> availablePlacesBetweenDates = new List<Place>();
            foreach (Place place in _camping.GetPlaces())
            {
                int counter = 0;
                //All reservations of place
                var reservationsOnPlace = _camping.GetReservations().Where(i => i.PlaceID == place.PlaceID).Select(i => i);
                if (reservationsOnPlace.Count() > 0)
                {
                    foreach (Reservation reservation in reservationsOnPlace)
                    {
                        if (arrivalDate <= reservation.ArrivalDate.Date && reservation.ArrivalDate.Date <= departureDate //StartDate of a reservation is between the arrival and departure date
                        || arrivalDate <= reservation.DepartureDate.Date && reservation.DepartureDate.Date <= departureDate //EndDate of a reservation is between the arrival and departure date
                        || reservation.ArrivalDate.Date <= arrivalDate && reservation.DepartureDate.Date >= departureDate) // Arrival and departure is between the reservation dates
                        {
                            counter++;
                        }
                    }
                }
                //counter will be 0 if no reservations interfere with the arrival and departure date
                if (counter == 0)
                {
                    availablePlacesBetweenDates.Add(place);
                }
            }
            return availablePlacesBetweenDates;
        }
    }
}
