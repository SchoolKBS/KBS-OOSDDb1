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


    }
}








