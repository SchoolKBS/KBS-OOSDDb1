﻿using CampingCore;
using CampingCore.CampingRepositories;
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
    public class CampingMapRepository : ICampingMapRepository
    {
        public string ConnectionString = "Data Source=Camping.db;Mode=ReadWriteCreate;";
        public CampingMapRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public List<Street> GetStreets()
        {
            List<Street> result = new List<Street>();

            string sql = "SELECT * FROM Street";

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

                                PropertyInfo property = typeof(Street).GetProperty(columnName);
                                if (property != null)
                                {
                                    Properties.Add(Convert.ChangeType(colmnValue, property.PropertyType));
                                }
                            }
                            result.Add(new Street(Properties));
                        }
                    }
                }
                return result;
            }
        }

        public List<Area> GetAreas()
        {
            throw new NotImplementedException();
        }

        public Street GetStreetByStreetID(Place place)
        {
            string sql = "SELECT * FROM Street WHERE StreetID = @StreetID";
            Street street = null;
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                using (var cmd = new SqliteCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@StreetID", place.StreetID);
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

                                PropertyInfo property = typeof(Street).GetProperty(columnName);
                                if (property != null)
                                {
                                    Properties.Add(Convert.ChangeType(colmnValue, property.PropertyType));
                                }
                            }
                            street = new Street(Properties);
                        }
                    }
                }
                return street;
            }
        }
    }
}
