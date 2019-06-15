using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kasug
{
    public enum Language
    {
        Chinese = 0,
        English = 1
    }
    public class Localization
    {
        public static Dictionary<Language, string> languageDictionary = new Dictionary<Language, string>()
        {
            { Language.Chinese,"Localization/Chinese.xml" },
            { Language.English,"Localization/English.xml" },
        };
    }
}
