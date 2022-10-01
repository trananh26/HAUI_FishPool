using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAUI_FishPoolManagement
{
    public class BLDatabase
    {
        internal static bool InsertDataMaster(FishpoolCommonParameter Parameter)
        {
            string stored = "Proc_InsertDatatoTotalTable";            
            bool Insert = DatabaseManagerment.UpdateData(stored, Parameter);
            return Insert;
        }

        internal static bool UpdateCurrentData(FishpoolCommonParameter Parameter)
        {
            try
            {
                string stored = "Proc_GetDataByID";
                bool UpdateCurrentData;
                if (DatabaseManagerment.CheckDataByID(stored, Parameter.DeviceID) == true)
                {
                    stored = "Proc_UpdateCurrentData";
                    UpdateCurrentData = DatabaseManagerment.UpdateCurrentData(stored, Parameter);
                }
                else
                {
                    stored = "Proc_InsertCurentData";
                    UpdateCurrentData = DatabaseManagerment.UpdateCurrentData(stored, Parameter);
                }
                return UpdateCurrentData;
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
                return false;
            }
            
        }
    }
}
