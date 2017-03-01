using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Diction_Master___Library;

namespace Diction_Master___Server
{
    public class Keys
    {
        private HashSet<string> fullYear = new HashSet<string>()
        {
            "09sJyzHTd3nJcrymoyXE0SyXpx4UV5",
            "FX8hMdt5JixcCokAwdik2B4AlMWV4K",
            "rHb9DekHlO7guHysInCDBNBiYh6SW9",
            "iCVmbdYLokUP9K6XRWtk4h85F7Y0jP",
            "o7CIcg5h0146jIutGzZPrtuyTZdTCS",
            "cmPhq22KbBkD2ZWTOQHUPcdunHbcXM",
            "00WzmfpJ2fZhsJNgcZxmv0XTwsIjV6",
            "GgY5HqFNAwtBYEBSHOGoNKSt8oL5qi",
            "OJwsiJ4QjX4kdJLMeOkBTjPLR0mS1z",
            "wlZTGKUWqJ7ZTMZ40n0AGpGmIH95RX",
            "LYeDAcmgzx6qrQ38I2FHGrpbhC9C0e",
            "cTpvKK1thmGuO1SMa7sTcWmdFaoVEp",
            "wfZ2egBzpN7dqkl7QiXq380O6GPquu",
            "Alf5fD7kyUQCiMCl7JUQmg0vUjfGyx",
            "yAVaHLkR10J2yvxc6keL3sz27HOcgB",
            "7hKAJQ8juGOWczOuyi5yqKlXdgnvl5",
            "DM7zPjQ5RXw4BOYjZ0hD87ACo7GAxv",
            "MqIw3lWn108EkLJVytfBGnKWXYEnSr",
            "QIEKftumR5hlf4BrVKlV3A39j2rzVF",
            "b4Am8oG5zaH3R2eleFxmdXh122s28o",
            "5O6kosBbLr45f9WlTYFiELj8eMinGc",
            "koEItlgVGbpZUB0hET3YIaWHI9OMTi",
            "HEbyTrcsGXp5L9QqzwY2VbrqtG3EzU",
            "yI2WmuxXNc9nPwqZ6SczxY2NbEWAr0"
        };

        private HashSet<string> oneTerm = new HashSet<string>()
        {
            "5O7s4tuojHpgVv72kAp1OHTnH8ck9c",
            "qjpgRpYl6IuAwvOfFCZvUOxEmhKFtb",
            "Pt15dGCpSKVJ3Q7RlitWVHwjjd3I6a",
            "M4sStoH3n2HN6PYOnYU0EPfCINf3kv",
            "iOT2Tf2UNV0LKJgZPFFV6sK9AbDZQe",
            "LcdJG2jcG93IOO09jbPhEeabW4NeYn",
            "pfot7Q3Gy8gJan7allodp6GeHaju4T",
            "HJMOiqWKb2VMsjnhO7BB8t7Ry9ccd4",
            "oTHhvOYxL7MbGeiGFCpPBxGYq5xE6t",
            "qtArscPo5IMYtCUqt7mO1TtEKF4UKs",
            "i4NMmB8J6kGHRNRbaRPPPQ4kVhOMk9",
            "Wvy7IRCnnOY4rrSyznqWJWhNCtJcv8",
            "iz2pLfO6QiHfgdqphq40tLTM5qHl0K",
            "HDggVBqn9kXRar3puJA8KyyJRyol5H",
            "mfsBUMMflzsjkPripWhGAPApvtvTk6",
            "q24pa1yjhP7I4q7Iqe1txkPJdfb3da",
            "0EfhS08ERqKhUvPKiq4SR27PTx6XwD",
            "4khoX7kvPG3mIhwWBt0KHfP3FGmElR",
            "VPj73NOmol9oC9Lc5duTh3yTlyPVIe",
            "73OjEAU23sRSvyM6YDs4NfKWkpPEAn",
            "VpBQrWrjAxz2eswTxRS9kjhnjoXJuf",
            "8SYuCV53eerwgAfpdhB8E8dmKuE7gL",
            "r7WC4d2Ih3isKATPoMvDZUescjC7at",
            "K5IYvOqmQCM5fIn3ieLWTgihHFnVfn"
        };

        public KeyValidation CheckKey(string key)
        {
            if (oneTerm.Contains(key))
                return KeyValidation.ValidOneTerm;
            if (fullYear.Contains(key))
                return KeyValidation.ValidFullYear;
            return KeyValidation.Invalid;
        }
    }
}
