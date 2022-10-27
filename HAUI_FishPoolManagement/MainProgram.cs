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
        static int Temp, Oxy, Ph, good, midle, notgood;
        static DateTime startdate, finishDate;
        static void Main(string[] args)
        {
            //LoadConfig();
            Console.OutputEncoding = Encoding.UTF8;
            
            Console.WriteLine(ConfigurationSettings.AppSettings["TitleHello"]);
            Console.WriteLine(ConfigurationSettings.AppSettings["Help"]);
            //LoadConfig();
            //ClientTest();
            LoadInfor();
            Console.ReadLine();
        }
        static string data;
        private static void ClientTest()
        {
            startdate = DateTime.Now;
            for (int i=0;i<100;i++)
            {
                //0001(ID)+ 1234(oxygen) + 1234(humidity) + 1234(ph) + 1234(Temperature) + x
                Console.WriteLine("Client thứ: " + i.ToString());

                data = i.ToString("0000") + "0650355008503" + i.ToString("000");
                LoraDataAnalys(data);
            }
            // Thời gian kết thúc
            finishDate = DateTime.Now;

            //Tổng thời gian thực hiện 
            TimeSpan time = finishDate - startdate;
            Console.WriteLine("Start:= " + startdate.ToString("dd-MM-yyyy HH:mm:ss.fffffff"));
            Console.WriteLine("Finish:= " + finishDate.ToString("dd-MM-yyyy HH:mm:ss.fffffff"));
            Console.WriteLine("Tổng thời gian chạy:= " + time.TotalSeconds + "s");
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
                Console.WriteLine(" Comport connection is opened");
                Console.WriteLine(" Comport name: " + Lora_Input.PortName);
                Console.WriteLine(" Comport baudrate: " + Lora_Input.BaudRate.ToString());
                Lora_Input.DataReceived += Lora_Input_DataReceived;
            }
            catch (Exception)
            {
                Console.WriteLine("Check connection of your computer com port with Lora station");
                //Console.ReadKey();

            }
        }
        
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

            // Đưa ra giá trị ràng buộc về các chỉ số để qyết định tình trạng môi trường ao nuôi
            // Nhiệt độ: Tốt(20 - 30 độ C), Bình thường(18 - 19,99 độ C hoặc 30,01 - 33 độ C), Cảnh báo (ngoài các vùng trên)
            // Nồng độ oxy: Tốt(> 5 mg / l), Bình thường (3,5 - 4,99 mg / l), Cảnh báo (< 3,5 mg / l)	
            // Độ ph: Tốt(7,5 - 8,5),  Bình thường (7 - 7,49 hoặc 8,51 - 9), Cảnh báo (goài các vùng trên)
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

                Console.WriteLine(" Mã thiết bị: " + Parameter.DeviceID.ToString());
                Console.WriteLine(" Nồng độ Oxy: " + Parameter.Dissolved_oxygen.ToString());
                Console.WriteLine(" Độ ẩm: " + Parameter.Humidity.ToString());
                Console.WriteLine(" Độ Ph: " + Parameter.Ph.ToString());

                Temp = Convert.ToInt32(Parameter.Temperature);
                Oxy = Convert.ToInt32(Parameter.Dissolved_oxygen);
                Ph = Convert.ToInt32(Parameter.Ph);

                if (Temp >= 20 && Temp <= 30)
                    good++;
                else if (Temp < 18 || Temp > 33)
                    notgood++;
                else
                    midle++;

                if (Oxy > 5)
                    good++;
                else if (Oxy < 3.5)
                    notgood++;
                else
                    midle++;

                if (Ph >= 7.5 && Ph <= 8.5)
                    good++;
                else if (Ph < 7 || Ph > 9)
                    notgood++;
                else
                    midle++;

                if (good == 3 || (good == 2 && midle == 1)) Parameter.Evaluation = "Tốt";
                else if (notgood >= 1) Parameter.Evaluation = "Cảnh báo";
                else Parameter.Evaluation = "Bình thường";

                Console.WriteLine(" Mức độ môi trường: " + Parameter.Evaluation);

                //if (Parameter.DeviceID == 1)
                //{
                //    startdate = DateTime.Now;
                //}
                //else if (Parameter.DeviceID == 100)
                //{
                //    finishDate = DateTime.Now;
                //    //Tổng thời gian thực hiện 
                //    TimeSpan time = finishDate - startdate;
                //    Console.WriteLine("Start:= " + startdate.ToString("dd-MM-yyyy HH:mm:ss.fffffff"));
                //    Console.WriteLine("Finish:= " + finishDate.ToString("dd-MM-yyyy HH:mm:ss.fffffff"));
                //    Console.WriteLine("Tổng thời gian chạy:= " + time.TotalSeconds + "s");
                //}

                //if (BLDatabase.InserDataTest(Parameter))
                //{

                //}
                // Insert data vảo bảng lưu data tổng
                if (BLDatabase.InsertDataMaster(Parameter))
                    Console.WriteLine("Insert Master Data Complete");
                //Console.ReadKey();
                // Check xem ao này đã được khai báo trong bảng curent hay chưa và thực hiện update
                if (BLDatabase.UpdateCurrentData(Parameter))
                    Console.WriteLine("Update Current Data Complete");
                //Console.ReadKey();
                good = 0; midle = 0; notgood = 0;
            }
            catch (Exception)
            {

            }
        }
    }
}
