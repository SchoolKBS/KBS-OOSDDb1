using CampingCore;
using CampingCore.CampingRepositories;
using CampingCore.NewFolder;
using CampingCore.PlacesOverviewPageClasses;
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
    public class CampingRepository : ICampingRepository
    {

        ICampingRepository ICampingRepository.CampingRepository { get ; set; }
        public ICampingPlaceRepository CampingPlaceRepository { get ; set; }
        public ICampingReservationRepository CampingReservationRepository { get; set; }
        public ICampingGuestRepository CampingGuestRepository { get; set; }
        public ICampingMapRepository CampingMapRepository { get; set; }

        public string ConnectionString = "Data Source=Camping.db;Mode=ReadWriteCreate;";

        public CampingRepository()
        {
            CreateAreaTable();
            CreateStreetTable();
            CreatePlaceTable();
            CreateGuestTable();
            CreateReservationTable();
            CampingPlaceRepository = new CampingPlaceRepository(ConnectionString);
            CampingMapRepository = new CampingMapRepository(ConnectionString);
            CampingGuestRepository = new CampingGuestRepository(ConnectionString);
            CampingReservationRepository = new CampingReservationRepository(ConnectionString);

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
            using (var connection = new SqliteConnection(ConnectionString))
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
                "SurfaceArea INT NOT NULL," +
                "PricePerNightPerPerson DECIMAL(18,2) NOT NULL," +
                "AmountOfPeople INT NOT NULL," +
                "Dogs TINYINT NOT NULL," +
                "Xcord1 INT NOT NULL," +
                "Ycord1 INT NOT NULL," +
                "Xcord2 INT NOT NULL," +
                "Ycord2 INT NOT NULL" +
                ");";
            using (var connection = new SqliteConnection(ConnectionString))
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
                "SurfaceArea INT NOT NULL," +
                "PricePerNightPerPerson DECIMAL(18,2) NOT NULL," +
                "AmountOfPeople INT NOT NULL," +
                "Dogs TINYINT NOT NULL," +
                "Xcord1 INT NOT NULL," +
                "Ycord1 INT NOT NULL," +
                "Xcord2 INT NOT NULL," +
                "Ycord2 INT NOT NULL," +
                "FOREIGN KEY(AreaID) REFERENCES Area(AreaID)" +
                ");";
            using (var connection = new SqliteConnection(ConnectionString))
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
                "PricePerNightPerPerson DECIMAL(18,2) NOT NULL," +
                "AmountOfPeople INT NOT NULL," +
                "Dogs TINYINT NOT NULL," +
                "Xcord INT NOT NULL," +
                "Ycord INT NOT NULL," +
                "FOREIGN KEY(StreetID) REFERENCES Street(StreetID)" +
                ");";
            using (var connection = new SqliteConnection(ConnectionString))
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
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                using (var cmd = new SqliteCommand(sql, connection))
                {
                    cmd.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void AddDummyDataArea()
        {
            string sql = "INSERT INTO Area (Power, SurfaceArea, PricePerNightPerPerson, AmountOfPeople, Dogs, Xcord1, Ycord1, Xcord2, Ycord2) VALUES (@Power, @SurfaceArea, @PricePerNightPerPerson, @AmountOfPeople, @Dogs, @Xcord1, @Ycord1, @Xcord2, @Ycord2);";

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                for (int i = 1; i <= 4; i++)
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Prepare();
                        command.Parameters.AddWithValue("@Power", i % 2 == 0);
                        command.Parameters.AddWithValue("@SurfaceArea", i);
                        command.Parameters.AddWithValue("@PricePerNightPerPerson", i);
                        command.Parameters.AddWithValue("@AmountOfPeople", i);
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
            string sql = "INSERT INTO Street (AreaID, Power, SurfaceArea, PricePerNightPerPerson, AmountOfPeople, Dogs, Xcord1, Ycord1, Xcord2, Ycord2) VALUES (@AreaID, @Power, @SurfaceArea, @PricePerNightPerPerson, @AmountOfPeople, @Dogs, @Xcord1, @Ycord1, @Xcord2, @Ycord2);";

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                for (int i = 1; i <= 4; i++)
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Prepare();
                        command.Parameters.AddWithValue("@AreaID", 1);
                        command.Parameters.AddWithValue("@Power", i % 2 == 0);
                        command.Parameters.AddWithValue("@SurfaceArea", i);
                        command.Parameters.AddWithValue("@PricePerNightPerPerson", i);
                        command.Parameters.AddWithValue("@AmountOfPeople", i);
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

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                for (int i = 1; i <= 10; i++)
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("PlaceID", i);
                        command.Parameters.AddWithValue("StreetID", (i / 3) + 1);
                        command.Parameters.AddWithValue("Power", i % 2 == 0);
                        command.Parameters.AddWithValue("SurfaceArea", i);
                        command.Parameters.AddWithValue("PricePerNightPerPerson", i);
                        command.Parameters.AddWithValue("AmountOfPeople", i);
                        command.Parameters.AddWithValue("Dogs", i % 2 == 0);
                        command.Parameters.AddWithValue("Xcord", i * 10);
                        command.Parameters.AddWithValue("Ycord", i * 10);
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
        }
        public void AddDummyDataGuests(string firstName, string lastName, string infix, string email, string city, string address, int i)
        {
            string sql = "INSERT INTO Guest (FirstName, LastName, Infix, Email, PhoneNumber, City, Address, PostalCode) VALUES (@FirstName, @LastName, @Infix, @Email, @PhoneNumber, @City, @Address, @PostalCode);";

            using (var connection = new SqliteConnection(ConnectionString))
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

            using (var connection = new SqliteConnection(ConnectionString))
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
            List<Place> places = CampingPlaceRepository.GetPlaces();
            List<string> firstNames = MakeFirstNamesList();
            List<string> lastNames = MakeFirstNamesList();
            List<string> infixes = MakeFirstNamesList();
            for (int i = 0; i < firstNames.Count; i++)
            {
                AddDummyDataGuests(firstNames[i], lastNames[i], "", "ditisemailadresnummer" + i + "@gmail.com", "stad" + i, "adres" + i, i);
            }
            List<Guest> guests = CampingGuestRepository.GetGuests();
            for (int i = 1; i <= guests.Count; i++)
            {
                AddDummyDataReservations(places[i - 1].PlaceID, i);
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




    }


}
