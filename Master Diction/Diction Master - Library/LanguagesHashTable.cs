using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diction_Master___Library
{
    public class LanguagesDictionary
    {
        private Dictionary<string, Tuple<string,string>> languagesDictionary;

        public LanguagesDictionary()
        {
            languagesDictionary = new Dictionary<string, Tuple<string, string>>();
            Initialize();
        }

        public string GetLanguageName(string country)
        {
            if (languagesDictionary.ContainsKey(country))
            {
                return languagesDictionary[country].Item1;
            }
            return null;
        }

        public string GetCountryAbreviation(string country)
        {
            if (languagesDictionary.ContainsKey(country))
            {
                return languagesDictionary[country].Item2;
            }
            return null;
        }

        private void Initialize()
        {
            languagesDictionary["Algeria"] = new Tuple<string, string>("Algerian","Alg");
            languagesDictionary["Argentina"] = new Tuple<string, string>("Spanish (Argentinian)", "Arg");
            languagesDictionary["Armenia"] = new Tuple<string, string>("Armenian", "Arm");
            languagesDictionary["Australia"] = new Tuple<string, string>("English (Australian)", "Aut");
            languagesDictionary["Austria"] = new Tuple<string, string>("German (Austrian)", "Ast");


            languagesDictionary["Italy"] = new Tuple<string, string>("Italian", "Ita");
            languagesDictionary["Russia"] = new Tuple<string, string>("Russian", "Rus");
            languagesDictionary["Serbia"] = new Tuple<string, string>("Serbian", "Srb");
            languagesDictionary["United Kingdom"] = new Tuple<string, string>("English", "UK");
            languagesDictionary["United States"] = new Tuple<string, string>("English (American)", "US");
            languagesDictionary["Nigeria"] = new Tuple<string, string>("Nigerian", "Nig");
            languagesDictionary["France"] = new Tuple<string, string>("French", "Fra");
            languagesDictionary["Germany"] = new Tuple<string, string>("German", "Ger");
            languagesDictionary["Spain"] = new Tuple<string, string>("Spanish", "Spa");
            languagesDictionary["Portugal"] = new Tuple<string, string>("Portuguese", "Por");
        }
    }
}
