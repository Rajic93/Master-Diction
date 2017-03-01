
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Diction_Master___Library;

namespace Diction_Master___Library
{
    public class Keys
    {
        private List<string> _oneTerm;
        private List<string> _fullYear;

        public Keys()
        {
            _oneTerm = new List<string>();
            _fullYear = new List<string>();
            FillList(true);
            FillList(false);
        }

        private void FillList(bool fullYear)
        {
            if (fullYear)
            {
                _fullYear.Add("5O7s4tuojHpgVv72kAp1OHTn");
                _fullYear.Add("qjpgRpYl6IuAwvOfFCZvUOxE");
                _fullYear.Add("Pt15dGCpSKVJ3Q7RlitWVHwj");
                _fullYear.Add("M4sStoH3n2HN6PYOnYU0EPfC");
            }
            else
            {
                _oneTerm.Add("09sJyzHTd3nJcrymoyXE0SyX");
                _oneTerm.Add("FX8hMdt5JixcCokAwdik2B4A");
                _oneTerm.Add("rHb9DekHlO7guHysInCDBNBi");
                _oneTerm.Add("iCVmbdYLokUP9K6XRWtk4h85");
            }
        }

        public KeyValidation CheckKey(string key)
        {
            bool ctr1 = false;
            bool ctr2 = false;
            bool found1 = false;
            bool found2 = false;
            KeyValidation type = 0;
            new Thread(() =>
            {
                if (_oneTerm.Contains(key))
                {
                    type = KeyValidation.ValidOneTerm;
                    found1 = true;
                }
                ctr1 = true;
            }).Start();
            new Thread(() =>
            {
                if (_fullYear.Contains(key))
                {
                    type = KeyValidation.ValidFullYear;
                    found2 = true;
                }
                ctr2 = true;
            }).Start();
            while (!ctr1 || !ctr2) { }
            if (found1 && found2)
            {
                return KeyValidation.Invalid;
            }
            if (found1 || found2)
            {
                return type;
            }
            return KeyValidation.Invalid;
        }
    }
}
