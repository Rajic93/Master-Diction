using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diction_Master___Library
{
    public class LanguagesHashTable
    {
        private Dictionary<string, string> languagesDictionary;

        public LanguagesHashTable()
        {
            languagesDictionary = new Dictionary<string, string>();
            Initialize();
        }

        public string GetLanguageName(string country)
        {
            if (languagesDictionary.ContainsKey(country))
            {
                return languagesDictionary[country];
            }
            return null;
        }

        private void Initialize()
        {
            languagesDictionary["Algeria"] = "Algerian";
            languagesDictionary["Argentina"] = "Spanish (Argentinian)";
            languagesDictionary["Armenia"] = "Armenian";
            languagesDictionary["Australia"] = "English (Australian)";
            languagesDictionary["Austria"] = "German (Austrian)";


            languagesDictionary["Italy"] = "Italian";
            languagesDictionary["Russia"] = "Russian";
            languagesDictionary["Serbia"] = "Serbian";
            languagesDictionary["United Kingdom"] = "English";
            languagesDictionary["United States"] = "English (American)";
            languagesDictionary["Nigeria"] = "Nigerian";
            languagesDictionary["France"] = "French";
            languagesDictionary["Germany"] = "German";
            languagesDictionary["Spain"] = "Spanish";
            languagesDictionary["Portugal"] = "Portuguese";
        }
    }
}
