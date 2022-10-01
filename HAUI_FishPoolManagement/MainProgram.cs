using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MySql.Data.MySqlClient;
using System.IO.Ports;
using System.Configuration;

namespace HAUI_FishPoolManagement
{
    class MainProgram
    {
       delegate void MyDelegate();    
        static SerialPort Lora_Input = new SerialPort();
        static void Main(string[] args)
        {
            //LoadConfig();
            Console.WriteLine(ConfigurationSettings.AppSettings["TitleHello"]);
            Console.WriteLine(ConfigurationSettings.AppSettings["Help"]);
            //LoadConfig();
            LoadInfor();
            Console.ReadLine();
        }

        private static void LoadInfor()
        {
            FishPoolDataManager_Load();
        }
        //khoi tao ETX
        static byte[] etx = { 0x03 };
        static byte[] stx = { 0x02 };
        static string ETXx;
        
        private static void FishPoolDataManager_Load()
        {
            ETXx = Encoding.ASCII.GetString(etx);
            try
            {
                Lora_Input.PortName = XINIFILE.ReadValue("COM_LORA");
                Lora_Input.BaudRate = int.Parse(XINIFILE.ReadValue("BAURATE"));
                Lora_Input.Open();
                Console.WriteLine("");
                Console.WriteLine("Comport connection is opened");
                Console.WriteLine("Comport name: " + Lora_Input.PortName);
                Console.WriteLine("Comport baudrate: " + Lora_Input.BaudRate.ToString());
                Lora_Input.DataReceived += Lora_Input_DataReceived;
            }
            catch (Exception)
            {
                Console.WriteLine("Check connection of yuor computer com port with Lora station");
                //Console.ReadKey();
            }
        }
        static string data;
        private static void Lora_Input_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                if (Lora_Input.BytesToRead > 500)
                {
                    Lora_Input.DiscardInBuffer();
                    return;
                }
                data = Lora_Input.ReadTo("x");
                Console.WriteLine("R " + DateTime.Now.ToString("HH:mm:ss") + ": " + data);

                LoraDataAnalys(data);
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }
        }

        private static void LoraDataAnalys(string data)
        {
            // data mẫu: 0001(ID)+ 1234(oxygen) + 1234(humidity) + 1234(ph) + 1234(Temperature) + x
            // ký tự x dùng để đánh dấu kết thúc chuỗi
            // các thông số của sensor lấy tới 2 số thập phân sau dấu phẩy ==> Temperature 1234 = 12.34 *C
            // Nếu nhiệt độ là 2.34*C thì data là 0234

            try
            {
                FishpoolCommonParameter Parameter = new FishpoolCommonParameter();

                Parameter.Create_date = DateTime.Now;
                Parameter.DeviceID = int.Parse(data.Substring(0, 4));
                Guid guid = Guid.NewGuid();
                Parameter.ID = guid.ToString();
                Parameter.Dissolved_oxygen = double.Parse(data.Substring(4, 2) + "." + data.Substring(6, 2));
                Parameter.Humidity = double.Parse(data.Substring(8, 2) + "." + data.Substring(10, 2));
                Parameter.Ph = double.Parse(data.Substring(12, 2) + "." + data.Substring(14, 2));
                Parameter.Temperature = double.Parse(data.Substring(16, 2) + "." + data.Substring(18, 2));
                int temp = Convert.ToInt32(Parameter.Temperature);

                if (temp >= 20 && temp <= 30)
                {
                    Parameter.Evaluation = "Tốt";
                }
                else if (temp < 15 || temp > 35)
                {
                    Parameter.Evaluation = "Cảnh báo";
                }
                else
                {
                    Parameter.Evaluation = "Bình thường";
                }

                // Insert data vảo bảng lưu data tổng
                if (BLDatabase.InsertDataMaster(Parameter))
                    Console.WriteLine("Insert Master Data Complete");
                //Console.ReadKey();
                // Check xem ao này đã được khai báo trong bảng curent hay chưa và thực hiện update
                if (BLDatabase.UpdateCurrentData(Parameter))
                    Console.WriteLine("Update Current Data Complete");
                //Console.ReadKey();
            }
            catch (Exception)
            {

            }
        }
    }
}
