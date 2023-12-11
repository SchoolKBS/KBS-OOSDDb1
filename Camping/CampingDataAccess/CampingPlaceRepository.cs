using CampingCore;
using CampingCore.CampingRepositories;
using Microsoft.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CampingDataAccess
{
    public class CampingPlaceRepository : ICampingPlaceRepository
    {
        public string ConnectionString = "Data Source=Camping.db;Mode=ReadWriteCreate;";
        public CampingPlaceRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }
        public List<Place> GetPlaces()
        {
            List<Place> result = new List<Place>();

            string sql = "SELECT * FROM Place";

            using (var connection = new SqliteConnection(ConnectionString))
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
        public void RemovePlace(Place place)
        {
            string sql = "DELETE FROM Place WHERE PlaceID = @PlaceID";

            using (var connection = new SqliteConnection(ConnectionString))
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
        public void AddPlace(Place place)
        {
            string sql = "INSERT INTO Place (PlaceID, StreetID, Power, SurfaceArea, PricePerNightPerPerson, AmountOfPeople, Dogs, Xcord, Ycord) VALUES (@PlaceID, @StreetID, @Power, @SurfaceArea, @PricePerNightPerPerson, @AmountOfPeople, @Dogs, @Xcord, @Ycord);";

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@PlaceID", place.PlaceID);
                    command.Parameters.AddWithValue("@StreetID", place.StreetID);
                    command.Parameters.AddWithValue("@Power", place.Power);
                    command.Parameters.AddWithValue("@SurfaceArea", place.SurfaceArea);
                    command.Parameters.AddWithValue("@PricePerNightPerPerson", place.PricePerNightPerPerson);
                    command.Parameters.AddWithValue("@AmountOfPeople", place.AmountOfPeople);
                    command.Parameters.AddWithValue("@Dogs", place.Dogs);
                    command.Parameters.AddWithValue("@Xcord", place.Xcord);
                    command.Parameters.AddWithValue("@Ycord", place.Ycord);
                    command.Prepare();
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        public Place GetPlaceFromPlaceID(int id)
        {
            Place place = null;
            string sql = "SELECT * FROM Place WHERE PlaceID = @PlaceID;";

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("PlaceID", id);
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
        public void UpdatePlaceData(int placeID, int streetID,  bool power, int surfaceArea, double pricePerNightPerPerson, int amountOfPeople, bool dogs)
        {
            string sql = "UPDATE place SET StreetID = @StreetID, Power = @Power, SurfaceArea = @SurfaceArea, PricePerNightPerPerson = @PricePerNightPerPerson, AmountOfPeople = @AmountOfPeople, Dogs = @Dogs WHERE PlaceID = @PlaceID;";

            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@PlaceID", placeID);
                    command.Parameters.AddWithValue("@StreetID", streetID);
                    command.Parameters.AddWithValue("@Power", power);
                    command.Parameters.AddWithValue("@SurfaceArea", surfaceArea);
                    command.Parameters.AddWithValue("@PricePerNightPerPerson", pricePerNightPerPerson);
                    command.Parameters.AddWithValue("@AmountOfPeople", amountOfPeople);
                    command.Parameters.AddWithValue("@Dogs", dogs);
                    command.Prepare();
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }

        }
    }
}
