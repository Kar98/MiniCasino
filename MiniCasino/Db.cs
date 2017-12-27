using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using MiniCasino.Patrons;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace MiniCasino
{
    public static class Db
    {
        private const string connString = "Server=tcp:minicasino.database.windows.net,1433;Initial Catalog=minicasino;Persist Security Info=False;User ID={your_username};Password={your_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public static SqlConnectionStringBuilder GetDbString()
        {
            try
            {
                var cb = new SqlConnectionStringBuilder();
                cb.DataSource = "minicasino.database.windows.net";
                cb.UserID = "kar98";
                cb.Password = "CASINOsatellite11!";
                cb.InitialCatalog = "minicasino";

                return cb;
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }
            return null;
        }

        public static void Submit_Tsql_NonQuery(SqlConnection connection, string tsql)
        {
            using (var command = new SqlCommand(tsql, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {

                }
            }
        }

        public static void SubmitTSqlQuery(string connection,string tsql)
        {
            Console.WriteLine("Start submitsql method");
            using (var conn = new SqlConnection(connection))
            {
                using (var command = new SqlCommand(tsql, conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("row found");
                        }
                    }
                }
            }
        }

        private static string GetAddress()
        {

            return "";
        }

        public static Patron GetPatronFromDB(int index = 2)
        {
            var ds = new DataSet();
            
            var patroncmd = $"SELECT * FROM Patron Where ID = {index}";

            using (var conn = new SqlConnection(GetDbString().ConnectionString))
            {
                conn.Open();
                using (var command = new SqlCommand(patroncmd, conn))
                {
                    
                    using (SqlDataAdapter da = new SqlDataAdapter(command))
                    {
                        da.Fill(ds);
                    }
                    
                }
                //var patronaddresscmd = $"SELECT * FROM Address Where ID = {PatronTable["Address"]}";
            }

            return null;
        }



    }


}

