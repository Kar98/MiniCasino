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

        public static void Submit_Tsql_NonQuery(string tsql)
        {
            using (var conn = new SqlConnection(GetDbString()))
            {
                using (var command = new SqlCommand(tsql, conn))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                    }
                }
            }
        }

        public static void SubmitTSqlQuery(string tsql)
        {
            Console.WriteLine("Start submitsql method");
            using (var conn = new SqlConnection(GetDbString()))
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

        public static int RunSp(string spName, Dictionary<string,object> dict)
        {
            try
            {
                using (var conn = new SqlConnection(GetDbString()))
                {
                    using (var command = new SqlCommand(spName, conn))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        AddParamsToSP(command, dict);

                        conn.Open();
                        command.ExecuteNonQuery();

                        return Convert.ToInt32(command.Parameters["@returnid"].Value);
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);

                return -1;
            }
        }

        private static void AddParamsToSP(SqlCommand cmd, Dictionary<string,object> dict)
        {
            if (dict == null)
                return;
            foreach(var d in dict)
            {
                cmd.Parameters.AddWithValue(d.Key, d.Value);
            }
        }

        private static string GetAddress()
        {

            return "";
        }

        public static Patron GetPatronFromDB(int index)
        {
            var ds = new DataSet();
            
            var patroncmd = $"SELECT * FROM Patrons Where ID = {index}";

            try
            { 
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
                        var sex = (string)ds.Tables[0].Rows[0]["Sex"];
                        var verified = (bool)ds.Tables[0].Rows[0]["Verified"];
                        var birthday = (DateTime)ds.Tables[0].Rows[0]["Birthday"];

                        return new Patron(birthday, sex[0], verified, fname, lname);
                    }
                }

            }catch(SqlException ex)
            {
                Console.WriteLine(ex.Message);
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

