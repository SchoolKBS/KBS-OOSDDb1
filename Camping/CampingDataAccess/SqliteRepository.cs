using CampingCore;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using MySqlX.XDevAPI.Common;
using SQLitePCL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CampingDataAccess
{
    public class SqliteRepository : ICampingRepository
    {
        public string connectionString = "Data Source=Camping.db;Mode=ReadWriteCreate;";
        public SqliteRepository()
        {
            CreateAreaTable();
            CreateStreetTable();
            CreatePlaceTable();
            CreateGuestTable();
            CreateReservationTable();

        }
        public void CreateGuestTable()
        {
            string sql = "CREATE TABLE IF NOT EXISTS Guest (" +
                "GuestID INTEGER PRIMARY KEY AUTOINCREMENT," +
                "FirstName VARCHAR(255) NOT NULL," +
                "LastName VARCHAR(255) NOT NULL," +
                "Infix VARCHAR(255)," +
                "Email VARCHAR(255)," +
                "PhoneNumber VARCHAR(255) NOT NULL," +
                "City VARCHAR(255)," +
                "Address VARCHAR(255)," +
                "PostalCode VARCHAR(255)" +
                ");";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new SqliteCommand(sql, connection)) 
                {
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void CreateAreaTable()
        {
            string sql = "CREATE TABLE IF NOT EXISTS Area (" +
                "AreaID INTEGER PRIMARY KEY AUTOINCREMENT," +
                "Name VARCHAR(255)," +
                "Power TINYINT NOT NULL," +
                "Dogs TINYINT NOT NULL," +
                "Xcord1 INT NOT NULL," +
                "Ycord1 INT NOT NULL," +
                "Xcord2 INT NOT NULL," +
                "Ycord2 INT NOT NULL" +
                ");";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new SqliteCommand(sql, connection))
                {
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }        
        public void CreateStreetTable()
        {
            string sql = "CREATE TABLE IF NOT EXISTS Street (" +
                "StreetID INTEGER PRIMARY KEY AUTOINCREMENT," +
                "AreaID INTEGER NOT NULL," +
                "Name VARCHAR(255), " +
                "Power TINYINT NOT NULL," +
                "Dogs TINYINT NOT NULL," +
                "Xcord1 INT NOT NULL," +
                "Ycord1 INT NOT NULL," +
                "Xcord2 INT NOT NULL," +
                "Ycord2 INT NOT NULL," +
                "FOREIGN KEY(AreaID) REFERENCES Area(AreaID)" +
                ");";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new SqliteCommand(sql, connection))
                {
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }        
        public void CreatePlaceTable()
        {
            string sql = "CREATE TABLE IF NOT EXISTS Place (" +
                "PlaceID INTEGER PRIMARY KEY AUTOINCREMENT," +
                "StreetID INTEGER NOT NULL, " +
                "Power TINYINT NOT NULL," +
                "SurfaceArea INT NOT NULL," +
                "PricePerNightPerPerson DECIMAL(18,2)," +
                "AmountOfPeople INT NOT NULL," +
                "Dogs TINYINT NOT NULL," +
                "Xcord INT NOT NULL," +
                "Ycord INT NOT NULL," +
                "FOREIGN KEY(StreetID) REFERENCES Street(StreetID)" +
                ");";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new SqliteCommand(sql, connection))
                {
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void CreateReservationTable()
        {
            string sql = "CREATE TABLE IF NOT EXISTS Reservation (" +
                "ReservationID INTEGER PRIMARY KEY AUTOINCREMENT," +
                "ArrivalDate DATE NOT NULL," +
                "DepartureDate DATE NOT NULL," +
                "PlaceID INTEGER NOT NULL," +
                "GuestID INTEGER NOT NULL," +
                "AmountOfPeople INT NOT NULL," +
                "IsPaid TINYINT NOT NULL," +
                "Price FLOAT NOT NULL," +
                "FOREIGN KEY(PlaceID) REFERENCES Place(PlaceID)," +
                "FOREIGN KEY(GuestID) REFERENCES Guest(GuestID)" +
                ");";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new SqliteCommand(sql, connection))
                {
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public List<Place> GetPlaces()
        {
            List<Place> result = new List<Place>();

            string sql = "SELECT * FROM Place";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new SqliteCommand(sql, connection))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ArrayList Properties = new ArrayList();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                Type columnType = reader.GetFieldType(i);
                                object colmnValue = reader.GetValue(i);

                                PropertyInfo property = typeof(Place).GetProperty(columnName);
                                if (property != null)
                                {
                                    Properties.Add(Convert.ChangeType(colmnValue, property.PropertyType));
                                }
                            }
                            result.Add(new Place(Properties));
                        }
                    }
                }
                return result;
            }
        }
        public Place GetPlaceFromPlaceID(int id)
        {
            Place place = null;
            string sql = "SELECT * FROM Place WHERE PlaceID = @PlaceID";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        command.Parameters.AddWithValue("PlaceID", id);
                        while (reader.Read())
                        {
                            ArrayList Properties = new ArrayList();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string columnName = reader.GetName(i);
                                Type columnType = reader.GetFieldType(i);
                                object colmnValue = reader.GetValue(i);

                                PropertyInfo property = typeof(Place).GetProperty(columnName);
                                if (property != null)
                                {
                                    Properties.Add(Convert.ChangeType(colmnValue, property.PropertyType));
                                }
                            }
                            place = new Place(Properties);
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

            using (var connection = new SqliteConnection(connectionString))
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
        
        public List<Guest> GetGuests()
        {
            List<Guest> result = new List<Guest>();

            string sql = "SELECT * FROM Guest";

            using (var connection = new SqliteConnection(connectionString))
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

                                PropertyInfo property = typeof(Guest).GetProperty(columnName);
                                if (property != null)
                                {
                                    Properties.Add(Convert.ChangeType(colmnValue, property.PropertyType));
                                }
                            }
                            result.Add(new Guest(Properties));
                        }
                    }
                }
            }
            return result;
        }
        public void RemoveAllPreviousReservationsByPlace(Place place, DateTime departureDate)
        {
            string sql = "DELETE FROM Reservation WHERE PlaceID = @PlaceID AND DepartureDate < @DepartureDate";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@PlaceID", place.PlaceID);
                    command.Parameters.AddWithValue("@DepartureDate", departureDate);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void RemovePlace(Place place)
        {
            string sql = "DELETE FROM Place WHERE PlaceID = @PlaceID";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new SqliteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("PlaceID", place.PlaceID);
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void AddDummyDataArea()
        {
            string sql = "INSERT INTO Area (Power, Dogs, Xcord1, Ycord1, Xcord2, Ycord2) VALUES (@Power, @Dogs, @Xcord1, @Ycord1, @Xcord2, @Ycord2);";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                for(int i = 1; i <= 4; i++)
                {
                    using(var command = new SqliteCommand(sql, connection))
                    {
                        command.Prepare();
                        command.Parameters.AddWithValue("@Power", i % 2 == 0);
                        command.Parameters.AddWithValue("@Dogs", i > 2);
                        command.Parameters.AddWithValue("@Xcord1", (i % 2) * 100);
                        command.Parameters.AddWithValue("@Ycord1", (i / 2) * 100);
                        command.Parameters.AddWithValue("@Xcord2", ((i % 2) * 100) + 100);
                        command.Parameters.AddWithValue("@Ycord2", ((i / 2) * 100) + 100);
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
        }
        public void AddDummyDataStreet()
        {
            string sql = "INSERT INTO Street (AreaID, Power, Dogs, Xcord1, Ycord1, Xcord2, Ycord2) VALUES (@AreaID, @Power, @Dogs, @Xcord1, @Ycord1, @Xcord2, @Ycord2);";
            
            using(var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                for (int i = 1; i <= 4; i++)
                {
                    using(var command = new SqliteCommand(sql, connection))
                    {
                        command.Prepare();
                        command.Parameters.AddWithValue("@AreaID", 1);
                        command.Parameters.AddWithValue("@Power", i % 2 == 0);
                        command.Parameters.AddWithValue("@Dogs", i > 2);
                        command.Parameters.AddWithValue("@Xcord1", 0);
                        command.Parameters.AddWithValue("@Ycord1", (i) * 20);
                        command.Parameters.AddWithValue("@Xcord2", 60);
                        command.Parameters.AddWithValue("@Ycord2", (i) * 20);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public void AddDummyDataPlaces()
        {
            string sql = "INSERT INTO Place (PlaceID, StreetID, Power, SurfaceArea, PricePerNightPerPerson, AmountOfPeople, Dogs, Xcord, Ycord) VALUES (@PlaceID, @StreetID, @Power, @SurfaceArea, @PricePerNightPerPerson, @AmountOfPeople, @Dogs, @Xcord, @Ycord);";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                for (int i = 1; i <= 10; i++) {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("PlaceID", i);
                        command.Parameters.AddWithValue("StreetID", (i /3) +1);
                        command.Parameters.AddWithValue("Power", i % 2 == 0);
                        command.Parameters.AddWithValue("SurfaceArea", i);
                        command.Parameters.AddWithValue("PricePerNightPerPerson", i);
                        command.Parameters.AddWithValue("AmountOfPeople", i);
                        command.Parameters.AddWithValue("Dogs", i % 2 == 0);
                        command.Parameters.AddWithValue("Xcord", i *10);
                        command.Parameters.AddWithValue("Ycord", i *10);
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
        }

        public void AddPlace(Place place)
        {
            string sql = "INSERT INTO Place (PlaceID, StreetID, Power, SurfaceArea, PricePerNightPerPerson, AmountOfPeople, Dogs, Xcord, Ycord) VALUES (@PlaceID, @StreetID @Power, @SurfaceArea, @PricePerNightPerPerson, @AmountOfPeople, @Dogs, @Xcord, @Ycord);";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("PlaceID", place.PlaceID);
                    command.Parameters.AddWithValue("Power", place.Power);
                    command.Parameters.AddWithValue("SurfaceArea", place.SurfaceArea);
                    command.Parameters.AddWithValue("PricePerNightPerPerson", place.PricePerNightPerPerson);
                    command.Parameters.AddWithValue("AmountOfPeople", place.AmountOfPeople);

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void AddDummyDataGuests(string firstName, string lastName, string infix, string email, string city, string address, int i)
        {
            string sql = "INSERT INTO Guest (FirstName, LastName, Infix, Email, PhoneNumber, City, Address, PostalCode) VALUES (@FirstName, @LastName, @Infix, @Email, @PhoneNumber, @City, @Address, @PostalCode);";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Prepare();
                    command.Parameters.AddWithValue("@FirstName", firstName);
                    command.Parameters.AddWithValue("@LastName", lastName);
                    command.Parameters.AddWithValue("@Infix", infix);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@PhoneNumber", i);
                    command.Parameters.AddWithValue("@City", city);
                    command.Parameters.AddWithValue("@Address", address);
                    command.Parameters.AddWithValue("@PostalCode", i);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void AddDummyDataReservations(int placeID, int i)
        {
            string sql = "INSERT INTO Reservation (ArrivalDate, DepartureDate, PlaceID, GuestID, AmountOfPeople, IsPaid, Price) VALUES ( @ArrivalDate, @DepartureDate, @PlaceID, @GuestID, @AmountOfPeople, @IsPaid, @Price);";

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ArrivalDate", DateTime.Now.Date.AddDays(i));
                    command.Parameters.AddWithValue("@DepartureDate", DateTime.Now.Date.AddDays(i + 10));
                    command.Parameters.AddWithValue("@PlaceID", placeID);
                    command.Parameters.AddWithValue("@GuestID", i);
                    command.Parameters.AddWithValue("@AmountOfPeople", i);
                    command.Parameters.AddWithValue("@IsPaid", i % 2 == 0);
                    command.Parameters.AddWithValue("@Price", i);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void AddDummyData()
        {
            AddDummyDataArea();
            AddDummyDataStreet();
            AddDummyDataPlaces();
            var places = GetPlaces();
            List<string> firstNames = MakeFirstNamesList();
            List<string> lastNames = MakeFirstNamesList();
            List<string> infixes = MakeFirstNamesList();
            for (int i = 0; i < firstNames.Count; i++)
            {
                AddDummyDataGuests(firstNames[i], lastNames[i], "", "ditisemailadresnummer" + i + "@gmail.com", "stad" + i, "adres" + i, i);
            }
            var guests = GetGuests();
            for (int i = 1; i <= guests.Count; i++)
            {
                AddDummyDataReservations(places[i - 1].PlaceID, i);
            }

        }

        public void AddReservation(Reservation reservation)
        {
            string sql =
                "INSERT INTO Reservation (ArrivalDate, DepartureDate, PlaceID, EmployeeID, GuestID, AmountOfPeople, IsPaid, Price)" +
                "VALUES (@stDate, @enDate, @plID, @emID, @guID, @AmountOfPeople, @IsPaid, @Price)";
            using (var connection = new SqliteConnection(connectionString))
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
        public int getLatestGuestID()
        {
            int result = 0;
            string sql =
                "SELECT MAX(GuestID) FROM Guest";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = reader.GetInt32(0);
                        }
                    }
                }
                connection.Close();
            }
            return result;
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
            throw new NotImplementedException();
        }

        public Place GetGuestFromGuestID(int id)
        {
            throw new NotImplementedException();
        }

        public void RemoveReservation(Reservation reservation)
        {
            string sql = "DELETE FROM Reservation WHERE ReservationID = @ReservationID";
            using (var connection = new SqliteConnection(connectionString))
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

        public void AddGuest(Guest guest)
        {
            string sql = "INSERT INTO Guest (FirstName, LastName, Infix, Email, PhoneNumber, City, Address, PostalCode) VALUES (@FirstName, @LastName, @Infix, @Email, @PhoneNumber, @City, @Address, @PostalCode);";
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Prepare();
                    command.Parameters.AddWithValue("@FirstName", guest.FirstName);
                    command.Parameters.AddWithValue("@LastName", guest.LastName);
                    command.Parameters.AddWithValue("@Infix", guest.Infix);
                    command.Parameters.AddWithValue("@Email", guest.Email);
                    command.Parameters.AddWithValue("@PhoneNumber", guest.PhoneNumber);
                    command.Parameters.AddWithValue("@City", guest.City);
                    command.Parameters.AddWithValue("@Address", guest.Address);
                    command.Parameters.AddWithValue("@PostalCode", guest.PostalCode);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public int GetLastGuestID()
        {
            string sql = "SELECT MAX(GuestID) FROM Guest";
            int result = 0;
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(sql, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = reader.GetInt32(0);
                        }
                    }
                }
                connection.Close();
            }
            return result;
        }

        public List<Street> GetStreets()
        {
            throw new NotImplementedException();
        }

        public List<Area> GetAreas()
        {
            throw new NotImplementedException();
        }
    }
}