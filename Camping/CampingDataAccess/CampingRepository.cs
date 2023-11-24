using CampingCore;
using Microsoft.Data.SqlClient;
using System;
using System.Data.SqlTypes;
using System.Net;
using System.Numerics;

namespace CampingDataAccess
{
    public class CampingRepository : ICampingRepository
    {
        private  string connectionString => BuildConnectionString();

        private string BuildConnectionString()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = "127.0.0.1";
            builder.UserID = "sa";
            builder.Password = "OOSDDb1!HAJTT";
            builder.InitialCatalog = "campingapplicatiedb";
            builder.Encrypt = true;
            builder.TrustServerCertificate = true;
            return builder.ConnectionString;
        }
        public List<Place> GetPlaces()
        {
            List<Place> result = new List<Place>();

            string sql = "SELECT * FROM Place";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Place(reader.GetInt32(0),
                                Convert.ToBoolean(reader.GetByte(1)),
                                reader.GetInt32(2),
                                reader.GetInt32(4),
                                Convert.ToDouble(reader.GetDecimal(3))));
                        }
                    }
                }
                return result;
        }
        public Place GetPlaceFromPlaceID(int id)
        {
            Place place = null;
            string sql = "SELECT * FROM Place WHERE PlaceID = @PlaceID";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        command.Parameters.AddWithValue("PlaceID", id);
                        while (reader.Read())
                        {
                            place = new Place(reader.GetInt32(0),
                                Convert.ToBoolean(reader.GetByte(1)),
                                reader.GetInt32(2),
                                reader.GetInt32(4),
                                Convert.ToDouble(reader.GetDecimal(3)));
                        }
                    }
                }
            }
            return place;
        }
        public List<Reservation> GetReservations()
        {
            List<Reservation> result = new List<Reservation>();

            string sql = "SELECT * FROM Reservation";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Reservation(reader.GetInt32(0),
                                reader.GetDateTime(1),
                                reader.GetDateTime(2),
                                reader.GetInt32(3),
                                reader.GetInt32(4),
                                reader.GetInt32(5),
                                reader.GetInt32(6),
                                Convert.ToBoolean(reader.GetByte(7)),
                                reader.GetDouble(8)
                                ));
                        }
                    }
                }
            }
            return result;
        }
        public List<Employee> GetEmployees()
        {
            List<Employee> result = new List<Employee>();

            string sql = "SELECT * FROM Employee";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Employee(reader.GetInt32(0),
                                reader.GetString(1),
                                reader.GetString(2),
                                reader.GetString(3)));
                        }
                    }
                }
            }
            return result;
        }
        public List<Guest> GetGuests()
        {
            List<Guest> result = new List<Guest>();

            string sql = "SELECT * FROM Guest";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var string1 = reader.GetString(1);
                            var string2 = reader.GetString(2);
                            var string3 = reader.GetString(3);
                            var string4 = reader.GetString(4);
                            var string5 = reader.GetString(5);
                            var string6 = reader.GetString(6);
                            var string7 = reader.GetString(7);
                            var string8 = reader.GetString(8);
                            result.Add(new Guest(reader.GetInt32(0), string1, string2, string3, string4, string5, string6, string7, string8));
                        }
                    }
                }
            }
            return result;
        }
        public void RemoveAllReservationsByPlace(Place place)
        {
            string sql = "DELETE FROM Reservation WHERE PlaceID = @PlaceID";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@PlaceID", place.PlaceNumber);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void RemovePlace(Place place)
        {
            string sql = "DELETE FROM Place WHERE PlaceID = @PlaceID";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("PlaceID", place.PlaceNumber);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void AddDummyDataPlaces()
        {
            string sql = "INSERT INTO Place (PlaceID, Power, SurfaceArea, PricePerNightPerPerson, AmountOfPeople, description) VALUES (@PlaceID, @Power, @SurfaceArea, @PricePerNightPerPerson, @AmountOfPeople, @description);";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                for (int i = 1; i <= 10; i++)
                {
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("PlaceID", i);
                        command.Parameters.AddWithValue("Power", i%2==0);
                        command.Parameters.AddWithValue("SurfaceArea", i);
                        command.Parameters.AddWithValue("PricePerNightPerPerson", i);
                        command.Parameters.AddWithValue("AmountOfPeople", i);
                        command.Parameters.AddWithValue("description", "");
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
        }
        public void AddDummyDataEmployee(string firstName, string lastName, string infix)
        {
            string sql = "INSERT INTO Employee (FirstName, LastName, Infix) VALUES (@FirstName, @LastName, @Infix);";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", firstName);
                        command.Parameters.AddWithValue("@LastName", lastName);
                        command.Parameters.AddWithValue("@Infix", infix);
                        command.ExecuteNonQuery();
                    }
                connection.Close();
            }
        }
        public void AddDummyDataGuests(string firstName, string lastName, string infix, string email, string city, string address, int i)
        {
            string sql = "INSERT INTO Guest (FirstName, LastName, Infix, Email, Phonenumber, City, Address, PostalCode) VALUES (FirstName, LastName, Infix, Email, Phonenumber, City, Address, PostalCode);";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("FirstName", firstName);
                    command.Parameters.AddWithValue("LastName", lastName);
                    command.Parameters.AddWithValue("Infix", infix);
                    command.Parameters.AddWithValue("Email", email);
                    command.Parameters.AddWithValue("Phonenumber", i);
                    command.Parameters.AddWithValue("City", city);
                    command.Parameters.AddWithValue("Address", address);
                    command.Parameters.AddWithValue("PostalCode", i);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void AddDummyDataReservations(int placeID, int employeeID, int guestID, int i)
        {
            string sql = "INSERT INTO Reservation (ArrivalDate, DepartureDate, PlaceID, EmployeeID, GuestID, PersonCount, IsPaid, Price) VALUES ( @ArrivalDate, @DepartureDate, @PlaceID, @EmployeeID, @GuestID, @PersonCount, @IsPaid, @Price);";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ArrivalDate", DateTime.Now.Date.AddDays(i));
                    command.Parameters.AddWithValue("@DepartureDate", DateTime.Now.Date.AddDays(i + 10));
                    command.Parameters.AddWithValue("@PlaceID", placeID);
                    command.Parameters.AddWithValue("@EmployeeID", employeeID);
                    command.Parameters.AddWithValue("@GuestID", guestID);
                    command.Parameters.AddWithValue("@PersonCount", i);
                    command.Parameters.AddWithValue("@IsPaid", i % 2 == 0);
                    command.Parameters.AddWithValue("@Price", i);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void AddDummyData()
        {

            AddDummyDataPlaces();
            var places = GetPlaces();
            AddDummyDataEmployee("Jan", "Jansen", "");
            var employees = GetEmployees();
            List<string> firstNames = MakeFirstNamesList();
            List<string> lastNames = MakeFirstNamesList();
            List<string> infixes = MakeFirstNamesList();
            for(int i = 0; i < firstNames.Count; i++)
            {
                AddDummyDataGuests(firstNames[i], lastNames[i], null, "ditisemailadresnummer"+i+"@gmail.com", "stad"+i, "adres"+i, i);
            }
            var guests = GetGuests();
            for(int i = 1; i <= guests.Count; i++)
            {
                AddDummyDataReservations(places[i-1].PlaceNumber, employees[0].EmployeeID, guests[i-1].ID, i);
            }
            
        }

        private List<string> MakeFirstNamesList()
        {
            List<string> firstNames = new List<string>();
            firstNames.Add("Jan");
            firstNames.Add("Piet");
            firstNames.Add("Hannelore");
            firstNames.Add("Jens");
            firstNames.Add("Ties");
            firstNames.Add("Aidin");
            firstNames.Add("Teun");
            firstNames.Add("Bas");
            firstNames.Add("Joren");
            firstNames.Add("Sam");
            return firstNames;
        }
        private List<string> MakeLastNameList()
        {
            List<string> firstNames = new List<string>();
            firstNames.Add("Jansen");
            firstNames.Add("Pietson");
            firstNames.Add("Baarssen");
            firstNames.Add("Bouma");
            firstNames.Add("Greve");
            firstNames.Add("Bos");
            firstNames.Add("Kleij");
            firstNames.Add("Luwental");
            firstNames.Add("Boerma");
            firstNames.Add("Harke");
            return firstNames;
        }

        public Place GetEmployeeFromEmployeeID(int id)
        {
            List<Reservation> result = new List<Reservation>();

            string sql = "SELECT * FROM Reservation";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int reservationNumber = reader.GetInt32(0);
                            DateTime arrivalDate = reader.GetDateTime(1);
                            DateTime departureDate = reader.GetDateTime(2);
                            int placeNumber = reader.GetInt32(3);
                            //GuestID and EmployeeID still have to get added and placeID too


                            Reservation reservation = new Reservation(reservationNumber, arrivalDate, departureDate, place);

                            result.Add(reservation);
                        }
                    }
                }
            }
            return result;
        }

        public Place GetGuestFromGuestID(int id)
        {
            throw new NotImplementedException();
        }

        /*Place ICampingRepository.GetPlaceFromPlaceID(int id)
        {
            throw new NotImplementedException();
        }*/
    }
}








