
namespace CampingCore
{
    public class Reservation
    {
        public int ReservationNumber { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DepartureDate { get; set; }
        public Place Place { get; set; }
        public Guest Guest { get; set; }

        public Employee Employee { get; set; } //Who handled the Reservation

        public Reservation(int reservationNumber, DateTime arrivalDate, DateTime departureDate, Place place, Guest guest, Employee employee)

        {
            this.ReservationNumber = reservationNumber;
            this.ArrivalDate = arrivalDate;
            this.DepartureDate = departureDate;
            this.Place = place;
            this.Guest = guest;
            this.Employee = employee;

        }
        public override string ToString()
        {
            return "Reservatie: " + ReservationNumber + " Plaats: " + Place.PlaceNumber + " Aankomstdatum: " + ArrivalDate.ToShortDateString() + " - Vertrekdatum " + DepartureDate.ToShortDateString() + " Gast " + Guest.FirstName + " Employee " + Employee.EmployeeID;

        }

    }
}
