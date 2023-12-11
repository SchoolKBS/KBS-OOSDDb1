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
using System.Runtime.InteropServices;
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
            CampingPlaceRepository = new CampingPlaceRepository(ConnectionString);
            CampingMapRepository = new CampingMapRepository(ConnectionString);
            CampingGuestRepository = new CampingGuestRepository(ConnectionString);
            CampingReservationRepository = new CampingReservationRepository(ConnectionString);
            if (!File.Exists("Camping.db")) {
                CreateDB();
                AddDummyData();
            }
        }
        public void CreateDB()
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
                "Power BOOLEAN NOT NULL," +
                "SurfaceArea INT NOT NULL," +
                "PricePerNightPerPerson DECIMAL(18,2) NOT NULL," +
                "AmountOfPeople INT NOT NULL," +
                "Dogs BOOLEAN NOT NULL," +
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
                "Power BOOLEAN NOT NULL," +
                "SurfaceArea INT NOT NULL," +
                "PricePerNightPerPerson DECIMAL(18,2) NOT NULL," +
                "AmountOfPeople INT NOT NULL," +
                "Dogs BOOLEAN NOT NULL," +
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
                "PlaceID INTEGER PRIMARY KEY," +
                "StreetID INTEGER NOT NULL, " +
                "Power BOOLEAN NOT NULL," +
                "SurfaceArea INT NOT NULL," +
                "PricePerNightPerPerson DECIMAL(18,2) NOT NULL," +
                "AmountOfPeople INT NOT NULL," +
                "Dogs BOOLEAN NOT NULL," +
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
                "IsPaid BOOLEAN NOT NULL," +
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
            string sql = "INSERT INTO Area (Power, SurfaceArea, PricePerNightPerPerson, AmountOfPeople, Dogs, Xcord1, Ycord1, Xcord2, Ycord2) " +
                "                   VALUES (@Power, @SurfaceArea, @PricePerNightPerPerson, @AmountOfPeople, @Dogs, @Xcord1, @Ycord1, @Xcord2, @Ycord2);";
            List<ArrayList> list = new List<ArrayList>(){
                new ArrayList(){0,11,11,11,0,0,0,500,375},
                new ArrayList(){0,13,13,13,1,500,0,500,375},
                new ArrayList(){1,14,14,14,1,0,375,500,375},
                new ArrayList(){1,12,12,12,0,500,375,500,375}
                };
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                foreach (var item in list)
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Prepare();
                        command.Parameters.AddWithValue("@Power", item[0]);
                        command.Parameters.AddWithValue("@SurfaceArea", item[1]);
                        command.Parameters.AddWithValue("@PricePerNightPerPerson", item[2]);
                        command.Parameters.AddWithValue("@AmountOfPeople", item[3]);
                        command.Parameters.AddWithValue("@Dogs", item[4]);
                        command.Parameters.AddWithValue("@Xcord1", item[5]);
                        command.Parameters.AddWithValue("@Ycord1", item[6]);
                        command.Parameters.AddWithValue("@Xcord2", item[7]);
                        command.Parameters.AddWithValue("@Ycord2", item[8]);
                        command.ExecuteNonQuery();
                    }
                }
                
                connection.Close();
            }
        }
        public void AddDummyDataStreet()
        {
            string sql = "INSERT INTO Street (AreaID, Name, Power, SurfaceArea, PricePerNightPerPerson, AmountOfPeople, Dogs, Xcord1, Ycord1, Xcord2, Ycord2)" +
                                    " VALUES (@AreaID, @Name, @Power, @SurfaceArea, @PricePerNightPerPerson, @AmountOfPeople, @Dogs, @Xcord1, @Ycord1, @Xcord2, @Ycord2);";
            List<ArrayList> list = new List<ArrayList>() {
                new ArrayList(){1,"Kalverstraat", 1,21,21,21,1,0,50,100,10},
                new ArrayList(){1,"Leidsestraat", 0,22,22,22,0,100,50,10,200},
                new ArrayList(){1,"Coolsingel", 1,23,23,23,1,100,100,400,10},
                new ArrayList(){1,"A. Kerkhof", 0,24,24,24,0,400,100,10,275},
                new ArrayList(){2,"Tielemansstraat", 1,25,25,25,1,0,100,200,10},
                new ArrayList(){2,"Barteljorisstraat", 0,26,26,26,0,100,100,10,275},
                new ArrayList(){4,"Houtstraat", 1,27,27,27,1,0,50,200,10},
                new ArrayList(){4,"Jac P. Thijsselaan", 0,28,28,28,0,100,0,10,200},
                new ArrayList(){3,"Gorterstraat", 1,29,29,29,1,300,50,200,10},
                new ArrayList(){3,"Weeverstraat", 0,30,30,30,0,400,0,10,200},
            };
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                foreach (var item in list)
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Prepare();
                        command.Parameters.AddWithValue("@AreaID", item[0]);
                        command.Parameters.AddWithValue("@Name", item[1]);
                        command.Parameters.AddWithValue("@Power", item[2]);
                        command.Parameters.AddWithValue("@SurfaceArea", item[3]);
                        command.Parameters.AddWithValue("@PricePerNightPerPerson", item[4]);
                        command.Parameters.AddWithValue("@AmountOfPeople", item[5]);
                        command.Parameters.AddWithValue("@Dogs", item[6]);
                        command.Parameters.AddWithValue("@Xcord1", item[7]);
                        command.Parameters.AddWithValue("@Ycord1", item[8]);
                        command.Parameters.AddWithValue("@Xcord2", item[9]);
                        command.Parameters.AddWithValue("@Ycord2", item[10]);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public void AddDummyDataPlaces()
        {

            string sql = "INSERT INTO Place (PlaceID, StreetID, Power, SurfaceArea, PricePerNightPerPerson, AmountOfPeople, Dogs, Xcord, Ycord) VALUES (@PlaceID, @StreetID, @Power, @SurfaceArea, @PricePerNightPerPerson, @AmountOfPeople, @Dogs, @Xcord, @Ycord);";

            List<ArrayList> list = new List<ArrayList>() {
                new ArrayList(){1,1,0,1,1,1,0,10,10},
                new ArrayList(){2,1,1,2,2,2,1,50,10},
                new ArrayList(){3,2,0,3,3,3,0,65,65},
                new ArrayList(){4,2,1,4,4,4,1,65,105},
                new ArrayList(){5,2,0,5,5,5,0,65,145},
                new ArrayList(){6,3,1,6,6,6,1,120,65},
                new ArrayList(){7,3,0,7,7,7,0,160,65},
                new ArrayList(){8,3,1,8,8,8,1,200,65},
                new ArrayList(){9,4,0,9,9,9,0,360,125},
                new ArrayList(){10,4,1,10,10,10,1,360,165},
            };
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                foreach(var item in list)
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("PlaceID", item[0]);
                        command.Parameters.AddWithValue("StreetID", item[1]);
                        command.Parameters.AddWithValue("Power", item[2]);
                        command.Parameters.AddWithValue("SurfaceArea", item[3]);
                        command.Parameters.AddWithValue("PricePerNightPerPerson", item[4]);
                        command.Parameters.AddWithValue("AmountOfPeople", item[5]);
                        command.Parameters.AddWithValue("Dogs", item[6]);
                        command.Parameters.AddWithValue("Xcord", item[7]);
                        command.Parameters.AddWithValue("Ycord", item[8]);
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
