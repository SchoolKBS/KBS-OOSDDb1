using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CampingCore;

namespace CampingDataAccess
{
    public class Database
    {
        private string connectionString = "Server=localhost;Database=camping;User ID=root; Password: lestaenbenthe";
        public void AddPlaceToDatabase(Place place)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand("INSERT INTO place (PlaceID, Power, SurfaceArea, PricePerNightPerPerson, AmountOfPeople, description) VALUES (@PlaceID, @Power, @SurfaceArea, @PricePerNightPerPerson, @AmountOfPeople, @description)", connection))
                    {
                        command.Parameters.AddWithValue("@PlaceID", place.PlaceNumber);
                        command.Parameters.AddWithValue("@Power", place.HasElectricity);
                        command.Parameters.AddWithValue("@SurfaceArea", place.SurfaceArea);
                        command.Parameters.AddWithValue("@PricePerNightPerPerson", place.PricePerNight);
                        command.Parameters.AddWithValue("@AmountOfPeople", place.NumberOfPeople);
                        command.Parameters.AddWithValue("@description", place.Description);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); 
            }
        }
    }
}
