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
        private string connectionString = "Server=localhost;Database=camping;User ID=root;";
        public void AddGuestToDatabase(Guest guest)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand("INSERT INTO guest (FirstName, LastName, Infix, Email, Telefoon, Woonplaats, Adress, Postcode) VALUES (@FirstName, @LastName, @Infix, @Email, @Telefoon, @Woonplaats, @Adress, @Postcode)", connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", guest.FirstName);
                        command.Parameters.AddWithValue("@LastName", guest.LastName);
                        command.Parameters.AddWithValue("@Infix", guest.PrepositionName);
                        command.Parameters.AddWithValue("@Email", guest.Email);
                        command.Parameters.AddWithValue("@Telefoon", guest.PhoneNumber);
                        command.Parameters.AddWithValue("@Woonplaats", guest.City);
                        command.Parameters.AddWithValue("@Adress", guest.Address);
                        command.Parameters.AddWithValue("@Postcode", guest.PostalCode);
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
