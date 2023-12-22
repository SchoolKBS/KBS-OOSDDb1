using CampingCore;
using CampingCore.NewFolder;
using Microsoft.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CampingDataAccess
{
    public class CampingGuestRepository : ICampingGuestRepository
    {
        public string ConnectionString = "Data Source=Camping.db;Mode=ReadWriteCreate;";
        public CampingGuestRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public Place GetGuestFromGuestID(int id)
        {
            throw new NotImplementedException();
        }


        public void AddGuest(Guest guest)
        {
            string sql = "INSERT INTO Guest (FirstName, LastName, Infix, Email, PhoneNumber, City, Address, PostalCode) VALUES (@FirstName, @LastName, @Infix, @Email, @PhoneNumber, @City, @Address, @PostalCode);";
            using (var connection = new SqliteConnection(ConnectionString))
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
            using (var connection = new SqliteConnection(ConnectionString))
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
        public int getLatestGuestID()
        {
            int result = 0;
            string sql =
                "SELECT MAX(GuestID) FROM Guest";
            using (var connection = new SqliteConnection(ConnectionString))
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

        public List<Guest> GetGuests()
        {
            List<Guest> result = new List<Guest>();

            string sql = "SELECT * FROM Guest";

            using (var connection = new SqliteConnection(ConnectionString))
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

        public void UpdateGuest(Guest guest)
        {
            string sql = "UPDATE Guest SET FirstName = @FirstName, LastName = @LastName, Infix = @Infix, Email = @Email, PhoneNumber = @PhoneNumber, City = @City, Address = @Address, PostalCode = @PostalCode WHERE GuestId = @GuestId;";
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Prepare();
                    command.Parameters.AddWithValue("@GuestId", guest.GuestID);
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
    }
}
