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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Place place { get; set; }

        public Employee Employee { get; set; } //Who handled the Reservation

        // public Guest guest { get; set;}

        //Guest still has to get added 
        public Reservation(int ID, DateTime Start, DateTime Eind, Place place)

        {
            this.ReservationNumber = ID;
            this.StartDate = Start;
            this.EndDate = Eind;
            this.place = place;
          
        }
        public override string ToString()
        {
            return "Reservatie: " + ReservationNumber + " Plek: " + place.PlaceNumber + " Start: " + StartDate.ToShortDateString() + " - Eind " + EndDate.ToShortDateString();

        }

    }
}
