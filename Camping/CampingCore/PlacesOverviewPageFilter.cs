﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore
{
    public class PlacesOverviewPageFilter
    {
        // Function to filter the list of places on the bool hasPower
        public static IEnumerable<Place> GetFilteredListOnPower(bool? hasPower, IEnumerable<Place> _placesSortedAndOrFiltered, Camping _camping)
        {
            if (hasPower != null)
            {
                _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.Intersect(_camping.Places.Where(i => i.HasPower == hasPower).Select(i => i));
            }
            return _placesSortedAndOrFiltered;
        }

        // Function to filter the list of places on the integer maxPriceRange 
        public static IEnumerable<Place> GetFilteredListOnPrice(double maxPriceRange, IEnumerable<Place> _placesSortedAndOrFiltered, Camping _camping)
        {

            if (maxPriceRange >= _camping.Places.Min(i => i.PricePerNight))
            {
                _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.Intersect(_camping.Places.Where(i => i.PricePerNight <= maxPriceRange).Select(i => i));
            }
            return _placesSortedAndOrFiltered;

        }

        // Function to filter the list of places on the int personCount
        public static IEnumerable<Place> GetFilteredListOnPersonCount(int personCount, IEnumerable<Place> _placesSortedAndOrFiltered, Camping _camping)
        {
            if (personCount >= 0)
            {
                _placesSortedAndOrFiltered = _placesSortedAndOrFiltered.Intersect(_camping.Places.Where(i => i.PersonCount >= personCount).Select(i => i));
            }
            return _placesSortedAndOrFiltered;
        }

        // Function to filter the list of places on the arrival and departure date
        public static IEnumerable<Place> GetFilteredListOnDate(DateTime arrivalDate, DateTime departureDate, IEnumerable<Place> _placesSortedAndOrFiltered, Camping _camping)
        {
            if (arrivalDate.Date < departureDate.Date && arrivalDate.Date >= DateTime.Now.Date)
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
            foreach (Place place in _camping.Places)
            {
                int counter = 0;
                //All reservations of place
                var reservationsOnPlace = _camping.Reservations.Where(i => i.place.PlaceNumber == place.PlaceNumber).Select(i => i);
                if (reservationsOnPlace.Count() > 0)
                {
                    foreach (Reservation reservation in reservationsOnPlace)
                    {
                        if ((arrivalDate <= reservation.StartDatum.Date && reservation.StartDatum.Date <= departureDate) //StartDate of a reservation is between the arrival and departure date
                        || (arrivalDate <= reservation.EindDatum.Date && reservation.EindDatum.Date <= departureDate) //EndDate of a reservation is between the arrival and departure date
                        || (reservation.StartDatum.Date <= arrivalDate && reservation.EindDatum.Date >= departureDate)) // Arrival and departure is between the reservation dates
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
