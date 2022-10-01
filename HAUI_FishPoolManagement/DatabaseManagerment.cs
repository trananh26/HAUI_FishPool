using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
namespace HAUI_FishPoolManagement
{
    public class DatabaseManagerment
    {
        static MySqlConnection conn = new MySqlConnection(ConfigurationSettings.AppSettings["DatabaseConnection"]);
        static MySqlCommand cmd = new MySqlCommand();
        static MySqlDataReader dr;
        static MySqlDataAdapter da;

        internal static bool UpdateData(string stored, FishpoolCommonParameter parameter)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                                    
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = stored;
                cmd.Parameters.AddWithValue("pDeviceID", parameter.DeviceID);
                cmd.Parameters.AddWithValue("pDissolved_oxygen", parameter.Dissolved_oxygen);
                cmd.Parameters.AddWithValue("pEvaluation", parameter.Evaluation);
                cmd.Parameters.AddWithValue("pHumidity", parameter.Humidity);
                cmd.Parameters.AddWithValue("pPh", parameter.Ph);
                cmd.Parameters.AddWithValue("pTemperature", parameter.Temperature);
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch(Exception ee)
            {
                Console.Write(ee.ToString());
                return false;
            }
        }

        internal static bool UpdateCurrentData(string stored, FishpoolCommonParameter parameter)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = stored;
                cmd.Parameters.AddWithValue("pDeviceID", parameter.DeviceID);
                cmd.Parameters.AddWithValue("pDissolved_oxygen", parameter.Dissolved_oxygen);
                cmd.Parameters.AddWithValue("pEvaluation", parameter.Evaluation);
                cmd.Parameters.AddWithValue("pHumidity", parameter.Humidity);
                cmd.Parameters.AddWithValue("pPh", parameter.Ph);
                cmd.Parameters.AddWithValue("pTemperature", parameter.Temperature);
                cmd.Parameters.AddWithValue("pID", parameter.ID);
                cmd.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            catch(Exception ee)
            {
                Console.Write(ee.ToString());
                return false;
            }
        }

        internal static bool CheckDataByID(string stored, int deviceID)
        {
            try
            {
                DataTable dt = new DataTable();
                bool check = false;
                //conn = new MySqlConnection(dbconnect);
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                string dbcomman = "Call " + stored + "(" + deviceID +")";
                da = new MySqlDataAdapter(dbcomman, conn);
                da.Fill(dt);
                conn.Close();
                if (dt.Rows.Count > 0)
                    check = true;
                else
                    check = false;

                return check;
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
                return false;
            }
        }
    }
}
