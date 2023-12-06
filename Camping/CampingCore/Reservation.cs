﻿
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

        //Guest still has to get added 
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
        }
        public Reservation(ArrayList properties)
        {
            ReservationID = (int)properties[0];
            ArrivalDate = (DateTime)properties[1];
            DepartureDate = (DateTime)properties[2];
            PlaceID = (int)properties[3];
            GuestID = (int)properties[4];
            AmountOfPeople = (int)properties[5];
            IsPaid = (bool)properties[6];
            Price = (double)properties[7];
        }
        public override string ToString()
        {
            return "Reservering: " + ReservationID + " Plaats: " + PlaceID + " Aankomstdatum: " + ArrivalDate.ToShortDateString() + " - Vertrekdatum " + DepartureDate.ToShortDateString();

        }

    }
}
