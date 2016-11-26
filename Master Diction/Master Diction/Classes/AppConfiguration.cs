using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Master_Diction.Classes
{
    
    public enum Licence
    {
        Trial,
        Standard,
        Premium,
        MasterKey
    }

    public class AppConfiguration
    {
        private static string _masterKey = "MAST-ERDI-CTIO-NJOH-NATA-NONA";

        public bool FirstStart { get; set; }
        public Licence Licence { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
        public DateTime KeyExpireDate { get; set; }

        public static string MasterKEy()
        {
            return _masterKey;
        }
    }
}
