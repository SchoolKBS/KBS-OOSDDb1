using CampingCore;
using CampingCore.CampingRepositories;
using Microsoft.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CampingDataAccess
{
    public class CampingReservationRepository : ICampingReservationRepository
    {
        public string ConnectionString;
        public CampingReservationRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public List<Reservation> GetReservations()
        {
            List<Reservation> result = new List<Reservation>();

            string sql = "SELECT * FROM Reservation";

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ArrayList Properties = new ArrayList();

                            for (int i = 0; i < reader.FieldCount; i++)
                            {

                                string columnName = reader.GetName(i);
                                Type columnType = reader.GetFieldType(i);
                                object colmnValue = reader.GetValue(i);

                                PropertyInfo property = typeof(Reservation).GetProperty(columnName);

                                if (property != null)
                                    if (columnName == "GuestID")
                                    {
                                        // Fetch guest name using GetGuestFromGuestID method
                                        Guest guest = GetGuestFromGuestID(Convert.ToInt32(colmnValue));
                                        Properties.Add(guest.FirstName);
                                        Properties.Add(Convert.ChangeType(colmnValue, property.PropertyType));

                                    }
                                    else
                                    {
                                        Properties.Add(Convert.ChangeType(colmnValue, property.PropertyType));
                                    }
                            }
                            result.Add(new Reservation(Properties));
                        }
                    }
                }
            }
            return result;
        }
        public Guest GetGuestFromGuestID(int id)
        {
            Guest result = new Guest();

            string sql = "SELECT * FROM Guest WHERE GuestID = @GuestID";

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Prepare();
                    command.Parameters.AddWithValue("@GuestID", id);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ArrayList Properties = new ArrayList();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                Type columnType = reader.GetFieldType(i);
                                object colmnValue = reader.GetValue(i);

                                PropertyInfo property = typeof(Guest).GetProperty(columnName);
                                if (property != null)
                                {
                                    Properties.Add(Convert.ChangeType(colmnValue, property.PropertyType));
                                }
                            }
                            result = new Guest(Properties);
                        }
                    }
                }
            }
            return result;
        }
        public void RemoveAllPreviousReservationsByPlace(Place place, DateTime departureDate)
        {
            string sql = "DELETE FROM Reservation WHERE PlaceID = @PlaceID AND DepartureDate < @DepartureDate";

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Prepare();
                    command.Parameters.AddWithValue("@PlaceID", place.PlaceID);
                    command.Parameters.AddWithValue("@DepartureDate", departureDate);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public void RemoveReservation(Reservation reservation)
        {
            string sql = "DELETE FROM Reservation WHERE ReservationID = @ReservationID";
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Prepare();
                    command.Parameters.AddWithValue("@ReservationID", reservation.ReservationID);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void AddReservation(Reservation reservation)
        {
            string sql =
                "INSERT INTO Reservation (ArrivalDate, DepartureDate, PlaceID, GuestID, AmountOfPeople, IsPaid, Price)" +
                "VALUES (@stDate, @enDate, @plID, @guID, @AmountOfPeople, @IsPaid, @Price)";
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Prepare();
                    command.Parameters.AddWithValue("@stDate", reservation.ArrivalDate.Date);
                    command.Parameters.AddWithValue("@enDate", reservation.DepartureDate.Date);
                    command.Parameters.AddWithValue("@plID", reservation.PlaceID);
                    command.Parameters.AddWithValue("@guID", reservation.GuestID);
                    command.Parameters.AddWithValue("@AmountOfPeople", reservation.AmountOfPeople);
                    command.Parameters.AddWithValue("@IsPaid", reservation.IsPaid);
                    command.Parameters.AddWithValue("@Price", reservation.Price);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public void UpdateReservation(Reservation reservation)
        {
            string sql = "UPDATE Reservation SET ArrivalDate = @ArrivalDate, DepartureDate = @DepartureDate, PlaceID = @PlaceID, AmountOfPeople = @AmountOfPeople, IsPaid = @IsPaid, Price = @Price WHERE ReservationID = @ReservationID;";
            using(var connection = new SqliteConnection(ConnectionString))
            { 
                connection.Open();
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Prepare();
                    command.Parameters.AddWithValue("@ArrivalDate", reservation.ArrivalDate);
                    command.Parameters.AddWithValue("@DepartureDate", reservation.DepartureDate);
                    command.Parameters.AddWithValue("@PlaceID", reservation.PlaceID);
                    command.Parameters.AddWithValue("@AmountOfPeople", reservation.AmountOfPeople);
                    command.Parameters.AddWithValue("@IsPaid", reservation.IsPaid);
                    command.Parameters.AddWithValue("@Price", reservation.Price);
                    command.Parameters.AddWithValue("ReservationID", reservation.ReservationID);
                    command.ExecuteNonQuery();
                }
                
            }
        }
    }
}
