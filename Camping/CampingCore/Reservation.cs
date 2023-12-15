
using System.Collections;

namespace CampingCore
{
    public class Reservation
    {
        public int ReservationID { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public int PlaceID { get; set; }
        public int GuestID { get; set;}
        public int AmountOfPeople { get; set; }
        public bool IsPaid { get; set; }
        public double Price { get; set; }

        public string GuestName {  get; set; }

        //Guest still has to get added 
        public Reservation(int reservationNumber, DateTime arrivalDate, DateTime departureDate, int placeID, int guestID, int personCount, bool isPaid, double price, string guestName)
        {
            this.ReservationID = reservationNumber;
            this.ArrivalDate = arrivalDate;
            this.DepartureDate = departureDate;
            this.PlaceID = placeID;
            this.GuestID = guestID;
            this.AmountOfPeople = personCount;
            this.IsPaid = isPaid;
            this.Price = price;
            this.GuestName = guestName;
        }
        public Reservation(int reservationNumber, DateTime arrivalDate, DateTime departureDate, int placeID, int guestID, int amountOfPeople, bool isPaid, double price)
        {
            this.ReservationID = reservationNumber;
            this.ArrivalDate = arrivalDate;
            this.DepartureDate = departureDate;
            this.PlaceID = placeID;
            this.GuestID = guestID;
            this.AmountOfPeople = amountOfPeople;
            this.IsPaid = isPaid;
            this.Price = price;
            this.GuestName = "Empty";
        }
        public Reservation(ArrayList properties)
        {
            ReservationID = (int)properties[0];
            PlaceID = (int)properties[1];
            GuestName = (string)properties[2];
            GuestID = (int)properties[3];
            ArrivalDate = (DateTime)properties[4];
            DepartureDate = (DateTime)properties[5];
            AmountOfPeople = (int)properties[6]; 
            IsPaid = (bool)properties[7];
            Price = (double)properties[8];
        }



        public override string ToString()
        {
            return "Reservering: " + ReservationID + " Plaats: " + PlaceID + " Aankomstdatum: " + ArrivalDate.ToShortDateString() + " - Vertrekdatum " + DepartureDate.ToShortDateString();

        }
        public override bool Equals(object? obj)
        {
            if(obj == null) return false;
            if(obj.GetType() != this.GetType()) return false;
            Reservation other = (Reservation)obj;
            return other.ReservationID == ReservationID;
        }

    }
}
