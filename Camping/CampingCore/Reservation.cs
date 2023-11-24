namespace CampingCore
{
    public class Reservation
    {
        public int ReservationNumber { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public int PlaceID { get; set; }
        public int EmployeeID { get; set; } //Who handled the Reservation
        public int GuestID { get; set;}
        public int personCount { get; set; }
        public bool IsPaid { get; set; }
        public double Price { get; set; }

        //Guest still has to get added 
        public Reservation(int reservationNumber, DateTime arrivalDate, DateTime departureDate, int placeID, int employeeID, int guestID, int personCount, bool isPaid, double price)
        {
            this.ReservationNumber = reservationNumber;
            this.ArrivalDate = arrivalDate;
            this.DepartureDate = departureDate;
            this.PlaceID = placeID;
            this.EmployeeID = employeeID;
            this.GuestID = guestID;
            this.personCount = personCount;
            this.IsPaid = isPaid;
            this.Price = price;
        }
        public override string ToString()
        {
            return "Reservering: " + ReservationNumber + " Plaats: " + PlaceID + " Aankomstdatum: " + ArrivalDate.ToShortDateString() + " - Vertrekdatum " + DepartureDate.ToShortDateString();

        }

    }
}
