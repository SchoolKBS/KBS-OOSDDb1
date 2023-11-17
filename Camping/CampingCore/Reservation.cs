using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampingCore
{
    public class Reservation
    {
        public int ReservatieNummer { get; set; }
        public DateTime StartDatum { get; set; }
        public DateTime EindDatum { get; set; }
        public Place place { get; set; }

        public Employee Employee { get; set; } //Who handled the Reservation

        // public Guest guest { get; set;}

        //Guest still has to get added 
        public Reservation(int ID, DateTime Start, DateTime Eind, Place place)

        {
            this.ReservatieNummer = ID;
            this.StartDatum = Start;
            this.EindDatum = Eind;
            this.place = place;

        }
        public override string ToString()
        {
            return "Reservatie: " + ReservatieNummer + " Plek: " + place.PlaceNumber + " Start: " + StartDatum.ToShortDateString() + " - Eind " + EindDatum.ToShortDateString();

        }

    }
}
