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
                //AddDummyData();
            }
        }
        public void CreateDB()
        {
            CreateAreaTable();
            CreateStreetTable();
            CreatePlaceTable();
            CreatePlaceExtendsTable();
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
                "Color INT NOT NULL," +
                "Power BOOLEAN NOT NULL," +
                "Dogs BOOLEAN NOT NULL," +
                "SurfaceArea INT NOT NULL," +
                "PricePerNightPerPerson FLOAT NOT NULL," +
                "AmountOfPeople INT NOT NULL," +
                "XCord1 INT NOT NULL," +
                "YCord1 INT NOT NULL," +
                "Width INT NOT NULL," +
                "Height INT NOT NULL" +
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
                "Name VARCHAR(255), " +
                "Power BOOLEAN NOT NULL," +
                "Dogs BOOLEAN NOT NULL," +
                "SurfaceArea INT NOT NULL," +
                "PricePerNightPerPerson FLOAT NOT NULL," +
                "AmountOfPeople INT NOT NULL," +
                "XCord1 INT NOT NULL," +
                "YCord1 INT NOT NULL," +
                "XCord2 INT NOT NULL," +
                "YCord2 INT NOT NULL" +
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
                "AreaID INTEGER NOT NULL, " +
                "Power BOOLEAN NOT NULL," +
                "Dogs BOOLEAN NOT NULL," +
                "SurfaceArea INT NOT NULL," +
                "PricePerNightPerPerson FLOAT NOT NULL," +
                "AmountOfPeople INT NOT NULL, " +
                "XCord INT NOT NULL, " +
                "YCord INT NOT NULL, " +
                "FOREIGN KEY(StreetID) REFERENCES Street(StreetID), " +
                "FOREIGN KEY(AreaID) REFERENCES Area(AreaID) " +
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
        public void CreatePlaceExtendsTable()
        {
            string sql = "CREATE TABLE IF NOT EXISTS Place_Extends (" +
                            "PlaceID INTEGER PRIMARY KEY, " +
                            "Power BOOLEAN, " +
                            "Dogs BOOLEAN, " +
                            "SurfaceArea BOOLEAN, " +
                            "PricePerNightPerPerson BOOLEAN, " +
                            "AmountOfPeople BOOLEAN, " +
                            "FOREIGN KEY(PlaceID) REFERENCES Place(PlaceID) " +
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
                "PlaceID INTEGER NOT NULL," +
                "GuestID INTEGER NOT NULL," +
                "ArrivalDate DATE NOT NULL," +
                "DepartureDate DATE NOT NULL," +
                "AmountOfPeople INT NOT NULL," +
                "IsPaid BOOLEAN NOT NULL," +
                "Price FLOAT NOT NULL," +
                "FOREIGN KEY(PlaceID) REFERENCES Place(PlaceID), " +
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
            string sql = "INSERT INTO Area (Name, Color, Power, Dogs, SurfaceArea, PricePerNightPerPerson, AmountOfPeople, XCord1, YCord1, Width, Height) " +
                "                   VALUES (@Name, @Color, @Power, @Dogs, @SurfaceArea, @PricePerNightPerPerson, @AmountOfPeople, @XCord1, @YCord1, @Width, @Height);";
            List<ArrayList> list = new List<ArrayList>(){
                new ArrayList(){"Zwolle", 0, 0,0,11,11,11,0,0,500,375},
                new ArrayList(){"Meppel", 2, 0,1,13,13,13,500,0,500,375},
                new ArrayList(){"Warnsveld", 4, 1, 1,14,14,14,0,375,500,375},
                new ArrayList(){"Nijkerk", 6, 1 ,0,12,12,12,500,375,500,375}
                };
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                foreach (var item in list)
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Prepare();
                        command.Parameters.AddWithValue("@Name", item[0]);
                        command.Parameters.AddWithValue("@Color", item[1]);
                        command.Parameters.AddWithValue("@Power", item[2]);
                        command.Parameters.AddWithValue("@Dogs", item[3]);
                        command.Parameters.AddWithValue("@SurfaceArea", item[4]);
                        command.Parameters.AddWithValue("@PricePerNightPerPerson", item[5]);
                        command.Parameters.AddWithValue("@AmountOfPeople", item[6]);
                        command.Parameters.AddWithValue("@XCord1", item[7]);
                        command.Parameters.AddWithValue("@YCord1", item[8]);
                        command.Parameters.AddWithValue("@Width", item[9]);
                        command.Parameters.AddWithValue("@Height", item[10]);
                        command.ExecuteNonQuery();
                    }
                }
                
                connection.Close();
            }
        }
        public void AddDummyDataStreet()
        {
            string sql = "INSERT INTO Street (Name, Power, Dogs, SurfaceArea, PricePerNightPerPerson, AmountOfPeople, XCord1, YCord1, XCord2, YCord2)" +
                                    " VALUES (@Name, @Power, @Dogs, @SurfaceArea, @PricePerNightPerPerson, @AmountOfPeople, @XCord1, @YCord1, @XCord2, @YCord2);";
            List<ArrayList> list = new List<ArrayList>() {
                new ArrayList(){"Kalverstraat", 1,1,21,21,21,0,50,100,10},
                new ArrayList(){"Leidsestraat", 0,0,22,22,22,100,50,10,200},
                new ArrayList(){"Coolsingel", 1,1,23,23,23,100,100,400,10},
                new ArrayList(){"A. Kerkhof", 0,0,24,24,24,400,100,10,275},
                new ArrayList(){"Tielemansstraat",  1,1,25,25,25,500,100,200,10},
                new ArrayList(){"Barteljorisstraat", 0,0,26,26,26,600,100,10,275},
                new ArrayList(){"Houtstraat", 1,1,27,27,27,500,425,200,10},
                new ArrayList(){"Jac P. Thijsselaan", 0,0,28,28,28,600,375,10,200},
                new ArrayList(){"Gorterstraat", 1,1,29,29,29,300,425,200,10},
                new ArrayList(){"Weeverstraat", 0,0,30,30,30,400,375,10,200},
            };
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                foreach (var item in list)
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Prepare();
                        command.Parameters.AddWithValue("@Name", item[0]);
                        command.Parameters.AddWithValue("@Power", item[1]);
                        command.Parameters.AddWithValue("@Dogs", item[2]);
                        command.Parameters.AddWithValue("@SurfaceArea", item[3]);
                        command.Parameters.AddWithValue("@PricePerNightPerPerson", item[4]);
                        command.Parameters.AddWithValue("@AmountOfPeople", item[5]);
                        command.Parameters.AddWithValue("@XCord1", item[6]);
                        command.Parameters.AddWithValue("@YCord1", item[7]);
                        command.Parameters.AddWithValue("@XCord2", item[8]);
                        command.Parameters.AddWithValue("@YCord2", item[9]);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        public void AddDummyDataPlaces()
        {

            string sql = "INSERT INTO Place (PlaceID, StreetID, AreaID, Power, Dogs, SurfaceArea, PricePerNightPerPerson, AmountOfPeople, XCord, YCord) " +
                                    "VALUES (@PlaceID, @StreetID, @AreaID, @Power, @Dogs, @SurfaceArea, @PricePerNightPerPerson, @AmountOfPeople, @XCord, @YCord);";

            List<ArrayList> list = new List<ArrayList>() {
                new ArrayList(){1,1,1,0,0,1,1,1,10,10},
                new ArrayList(){2,1,1,1,1,2,2,2,50,10},
                new ArrayList(){3,2,1,0,0,3,3,3,65,65},
                new ArrayList(){4,2,1,1,1,4,4,4,65,105},
                new ArrayList(){5,2,1,0,0,5,5,5,65,145},
                new ArrayList(){6,3,1,1,1,6,6,6,120,65},
                new ArrayList(){7,3,1,0,0,7,7,7,160,65},
                new ArrayList(){8,3,1,1,1,8,8,8,200,65},
                new ArrayList(){9,4,1,0,0,9,9,9,360,125},
                new ArrayList(){10,4,1,1,1,10,10,10,360,165},
            };
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                foreach(var item in list)
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@PlaceID", item[0]);
                        command.Parameters.AddWithValue("@StreetID", item[1]);
                        command.Parameters.AddWithValue("@AreaID", item[2]);
                        command.Parameters.AddWithValue("@Power", item[3]);
                        command.Parameters.AddWithValue("@Dogs", item[4]);
                        command.Parameters.AddWithValue("@SurfaceArea", item[5]);
                        command.Parameters.AddWithValue("@PricePerNightPerPerson", item[6]);
                        command.Parameters.AddWithValue("@AmountOfPeople", item[7]);
                        command.Parameters.AddWithValue("@XCord", item[8]);
                        command.Parameters.AddWithValue("@YCord", item[9]);
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
        }
        public void AddDummyDataGuests()
        {
            string sql = "INSERT INTO Guest (FirstName, LastName, Infix, Email, PhoneNumber, City, Address, PostalCode) VALUES (@FirstName, @LastName, @Infix, @Email, @PhoneNumber, @City, @Address, @PostalCode);";
            List<ArrayList> list = new List<ArrayList>() {
                new ArrayList(){"Jan", "Jansen", "", "janjansen@gmail.com", "0612345678", "Zwolle", "Zwollestraat 3", "1111 AA"},
                new ArrayList(){"Ties", "Tiessen", "", "tiestiessen@gmail.com", "0612345678", "Zwolle", "Zwollestraat 3", "1111 AB"},
                new ArrayList(){"Jens", "Jenssen", "", "jensjenssen@gmail.com", "0612345678", "Zwolle", "Zwollestraat 3", "1111 AC"},
                new ArrayList(){"Aidin", "Aidinsen", "", "aidinaidinsen@gmail.com", "0612345678", "Zwolle", "Zwollestraat 3", "1111 AD"},
                new ArrayList(){"Hannelore", "Hanneloresen", "", "hannelorehanneloresen@gmail.com", "0612345678", "Zwolle", "Zwollestraat 3", "1111 AE"},
                new ArrayList(){"Joren", "Jorensen", "", "jorenjorensen@gmail.com", "0612345678", "Zwolle", "Zwollestraat 3", "1111 AF" },
                new ArrayList(){"Sam", "Samsen", "", "samsamsen@gmail.com", "0612345678", "Zwolle", "Zwollestraat 3", "1111 AG"},
                new ArrayList(){"Bas", "Bassen", "", "basbassen@gmail.com", "0612345678", "Zwolle", "Zwollestraat 3", "1111 AH"},
                new ArrayList(){"Tim", "Timsen", "", "timtimsen@gmail.com", "0612345678", "Zwolle", "Zwollestraat 3", "1111 AI"},
                new ArrayList(){"Wietze", "Wietzesen", "", "wietzewitzsen@gmail.com", "0612345678", "Zwolle", "Zwollestraat 3", "1111 AJ"},
            };
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                foreach (var item in list)
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Prepare();
                        command.Parameters.AddWithValue("@FirstName", item[0]);
                        command.Parameters.AddWithValue("@LastName", item[1]);
                        command.Parameters.AddWithValue("@Infix", item[2]);
                        command.Parameters.AddWithValue("@Email", item[3]);
                        command.Parameters.AddWithValue("@PhoneNumber", item[4]);
                        command.Parameters.AddWithValue("@City", item[5]);
                        command.Parameters.AddWithValue("@Address", item[6]);
                        command.Parameters.AddWithValue("@PostalCode", item[7]);
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
        }
        public void AddDummyDataReservations(int placeID, int i)
        {
            string sql = "INSERT INTO Reservation (PlaceID, GuestID, ArrivalDate, DepartureDate, AmountOfPeople, IsPaid, Price)" +
                                         " VALUES (@PlaceID, @GuestID, @ArrivalDate, @DepartureDate, @AmountOfPeople, @IsPaid, @Price);";

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@PlaceID", placeID);
                    command.Parameters.AddWithValue("@GuestID", i);
                    command.Parameters.AddWithValue("@ArrivalDate", DateTime.Now.Date.AddDays(i));
                    command.Parameters.AddWithValue("@DepartureDate", DateTime.Now.Date.AddDays(i + 10));
                    command.Parameters.AddWithValue("@AmountOfPeople", i);
                    command.Parameters.AddWithValue("@IsPaid", i % 2 == 0);
                    command.Parameters.AddWithValue("@Price", i);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public void AddDummyDataPlaceExtends()
        {
            string sql = "INSERT INTO Place_Extends (PlaceID)" +
                                           " VALUES (@PlaceID);";
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                for (int i = 1; i<=10;i++)
                {
                    using (var command = new SqliteCommand(sql, connection))
                    {
                        command.Prepare();
                        command.Parameters.AddWithValue("@PlaceID", i);
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
        }
        public void AddDummyData()
        {
            AddDummyDataArea();
            //AddDummyDataStreet();
            //AddDummyDataPlaces();
            //AddDummyDataPlaceExtends();
            //List<Place> places = CampingPlaceRepository.GetPlaces();
            //AddDummyDataGuests();
            //List<Guest> guests = CampingGuestRepository.GetGuests();
            //for (int i = 1; i <= guests.Count; i++)
            //{
            //    AddDummyDataReservations(places[i - 1].PlaceID, i);
            //}

        }
    }
}
