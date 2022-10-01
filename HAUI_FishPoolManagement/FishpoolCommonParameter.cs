using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAUI_FishPoolManagement
{
    public class FishpoolCommonParameter
    {
        private int _DeviceID;
        private double _Temperature;
        private double _Ph;
        private double _Humidity;
        private string _Evaluation;
        private double _Dissolved_oxygen;
        private DateTime _Create_date;
        private string _ID;

        public int DeviceID
        {
            get
            {
                return _DeviceID;
            }

            set
            {
                _DeviceID = value;
            }
        }

        public double Temperature
        {
            get
            {
                return _Temperature;
            }

            set
            {
                _Temperature = value;
            }
        }

        public double Ph
        {
            get
            {
                return _Ph;
            }

            set
            {
                _Ph = value;
            }
        }

        public double Humidity
        {
            get
            {
                return _Humidity;
            }

            set
            {
                _Humidity = value;
            }
        }

        public string Evaluation
        {
            get
            {
                return _Evaluation;
            }

            set
            {
                _Evaluation = value;
            }
        }

        public double Dissolved_oxygen
        {
            get
            {
                return _Dissolved_oxygen;
            }

            set
            {
                _Dissolved_oxygen = value;
            }
        }

        public DateTime Create_date
        {
            get
            {
                return _Create_date;
            }

            set
            {
                _Create_date = value;
            }
        }

        public string ID
        {
            get
            {
                return _ID;
            }

            set
            {
                _ID = value;
            }
        }
    }
    
}
