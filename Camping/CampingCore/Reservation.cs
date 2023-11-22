using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore
{
    public class Reservation
    {
        public int ReservationNumber { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public Place Place { get; set; }

        public Employee Employee { get; set; } //Who handled the Reservation

        // public Guest guest { get; set;}

        //Guest still has to get added 
        public Reservation(int reservationNumber, DateTime arrivalDate, DateTime departureDate, Place place)

        {
            this.ReservationNumber = reservationNumber;
            this.ArrivalDate = arrivalDate;
            this.DepartureDate = departureDate;
            this.Place = place;
          
        }
        public override string ToString()
        {
            return "Reservatie: " + ReservationNumber + " Plaats: " + Place.PlaceNumber + " Aankomstdatum: " + ArrivalDate.ToShortDateString() + " - Vertrekdatum " + DepartureDate.ToShortDateString();

        }

    }
}
