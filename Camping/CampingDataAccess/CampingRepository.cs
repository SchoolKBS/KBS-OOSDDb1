
using CampingCore;
using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;

namespace CampingDataAccess
{
    public class CampingRepository : ICampingRepository
    {
        private string connectionString => BuildConnectionString();

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
                }
                return result;
            

        }
        public void AddReservation(Reservation reservation)
        {
            try
            {
                string slq = 
                    "INSERT into Reservation (StartDate, EndData, PlaceID, EmployeeID, GuestID)" +
                    "VALUES (@stDate, @enDate, @plID, @emID, @guID)";
                using(var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using(var command = new SqlCommand(slq))
                    {
                        command.Prepare();
                        command.Parameters.AddWithValue("@stDate", reservation.StartDatum.Date);
                        command.Parameters.AddWithValue("@enDate", reservation.EindDatum.Date);
                        command.Parameters.AddWithValue("@plID", reservation.place.PlaceNumber);
                        command.Parameters.AddWithValue("@emID", 1);
                        command.Parameters.AddWithValue("@guID", getLatestGuestID());
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                //for custrom exceptions
            }
        }
        public int getLatestGuestID()
        {
            int result = 0;
            string sql =
                "SELECT MAX(GuestID) FROM Guest";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using(var command = new SqlCommand(sql))
                {
                    using(var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result = reader.GetInt32(0);
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