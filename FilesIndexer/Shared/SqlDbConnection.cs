using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AntiPShared
{
    public class SqlDbConnection
    {
        //string connectionString = @"Data Source=SQL5024.SmarterASP.NET;Initial Catalog=DB_9F9885_skdautorework;User Id=DB_9F9885_skdautorework_admin;Password=salo123456;";
        SqlConnection conn;
        string connectionString = @"Data Source = SQL5006.site4now.net; Initial Catalog = DB_9F9885_antip; User Id = DB_9F9885_antip_admin; Password=salo123456;";
        string commandString;
        SqlCommand cmd;
        SqlDataReader reader;
        string procedureName;
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        Dictionary<int, Dictionary<string, string>> resultData = new Dictionary<int, Dictionary<string, string>>();

        public int ResultRowAmount
        {
            get { return resultData.Count; }
        }
        public string Procedurename
        {
            set { procedureName = value; }
        }
        public SqlDbConnection()
        {
            conn = new SqlConnection();
            conn.ConnectionString = connectionString;
        }


        public void AddParameter(string key, string value)
        {
            parameters.Add(key, value);
        }
        public void AddDict(string key, DataTable value)
        {
            try
            {
                cmd = new SqlCommand(procedureName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                foreach (var x in parameters)
                {
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@" + x.Key, Value = x.Value });
                }
                var param = cmd.Parameters.AddWithValue("myTable", value);
                param.SqlDbType = SqlDbType.Structured;
                conn.Open();
                reader = cmd.ExecuteReader();
                int j = 0;
                while (reader.Read())
                {
                    Dictionary<string, string> Result = new Dictionary<string, string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Result.Add(reader.GetName(i), Convert.ToString(reader.GetValue(i)));
                    }
                    resultData.Add(j, Result);
                    j++;
                }
            }
            finally
            {
                // close reader
                if (reader != null)
                {
                    reader.Close();
                }

                // close connection
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }
        public void ExecuteObject()
        {
            try
            {
                cmd = new SqlCommand(procedureName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
            
                foreach (var x in parameters)
                {
                    cmd.Parameters.Add(new SqlParameter() { ParameterName = "@" + x.Key, Value = x.Value });
                }
                conn.Open();
                reader = cmd.ExecuteReader();
                int j = 0;
                while (reader.Read())
                {
                    Dictionary<string, string> Result = new Dictionary<string, string>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        Result.Add(reader.GetName(i), Convert.ToString(reader.GetValue(i)));
                    }
                    resultData.Add(j, Result);
                    j++;
                }
            }
            finally
            {
                // close reader
                if (reader != null)
                {
                    reader.Close();
                }

                // close connection
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }
        public string GetFieldByName(int row, string name)
        {
            return resultData[row][name];
        }
    }
}