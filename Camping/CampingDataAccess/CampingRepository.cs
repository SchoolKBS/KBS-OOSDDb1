
using CampingCore;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;

namespace CampingDataAccess
{
    public class CampingRepository : ICampingRepository
    {
        private string connectionString = "Data Source=127.0.0.1;Initial Catalog=campingapplicatiedb;User ID=sa;Password=OOSDDb1!HAJTT";

        public List<Place> GetPlaces()
        {
            List<Place> result = new List<Place>();

            string sql = "SELECT * FROM Place";

            using(var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using(var command = new SqlCommand(sql, connection))
                {
                    using(var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new Place(reader.GetInt32(0), reader.GetBoolean(1), reader.GetInt32(2), reader.GetInt32(4), (int) reader.GetFloat(3)));
                        }
                    }
                }
            }
            return result;
        }

        public List<Reservation> GetReservations()
        {
            throw new NotImplementedException();
        }
    }
}