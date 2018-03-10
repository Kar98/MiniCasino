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
        private const string connString = "Data Source=(localdb)\\ProjectsV13;Initial Catalog=CasinoDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        public static string GetDbString()
        {
            try
            {
                /* Azure
                var cb = new SqlConnectionStringBuilder();
                cb.DataSource = "minicasino.database.windows.net";
                cb.UserID = "kar98";
                cb.Password = "CASINOsatellite11!";
                cb.InitialCatalog = "minicasino";
                */
                return connString;
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

        public static Patron GetPatronFromDB(int index)
        {
            var ds = new DataSet();
            
            var patroncmd = $"SELECT * FROM Patron Where ID = {index}";

            using (var conn = new SqlConnection(GetDbString()))
            {
                conn.Open();
                using (var command = new SqlCommand(patroncmd, conn))
                {
                    
                    //Fix the below, it cannot find the Table apparently.
                    using (SqlDataAdapter da = new SqlDataAdapter(command))
                    {
                        da.Fill(ds);
                    }
                    
                    //Firstname, Lastname, Sex, Verified, Birthday
                    var fname = (string)ds.Tables[0].Rows[0]["Firstname"];
                    var lname = (string)ds.Tables[0].Rows[0]["Lastname"];
                    var sex = (char)ds.Tables[0].Rows[0]["Sex"];
                    var verified = (bool)ds.Tables[0].Rows[0]["Verified"];
                    var birthday = DbDateToString((string)ds.Tables[0].Rows[0]["Birthday"]);

                    return new Patron(birthday, sex, verified, fname, lname);
                }
                //var patronaddresscmd = $"SELECT * FROM Address Where ID = {PatronTable["Address"]}";
            }

            return null;
        }

        private static DateTime DbDateToString(string dbDate)
        {
            var splits = dbDate.Split('-');
            if(splits.Length != 3)
            {
                throw new Exception("Date string is not in the correct format");
            }

            return new DateTime(int.Parse(splits[0]), int.Parse(splits[1]), int.Parse(splits[2]));
        }



    }


}

