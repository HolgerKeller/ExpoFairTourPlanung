using System.Collections.Generic;


namespace ExpofairTourPlanung.Models
{

    public static class StatCodes
    {
        public static IDictionary<string, string> StatusCodeDic;
        static StatCodes()
        {
            StatusCodeDic = new Dictionary<string, string>()
        {

            {"LS_OK", "Alle Artikel gemäß Lieferschein ohne Mängel geliefert"},
            {"LS_ANM", "Alle Artikel gemäß Lieferschein ohne Mängel mit Anmerkungen geliefert."},
            {"LS_NOK", "Artikel gemäß Lieferschein mit Mängel geliefert"},
            {"RLS_OK", "Alle Artikel gemäß Rücklieferschein ohne Mängel zurückgenommen."},
            {"RLS_NOK", "Artikel gemäß Rücklieferschein mit Mängel geliefert"}

        };
        }
    }
}