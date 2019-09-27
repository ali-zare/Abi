using System;
using System.Globalization;

namespace Abi.Test
{
    public static class PersianDate
    {
        static PersianDate()
        {
            fa_culture = CultureInfo.GetCultureInfo("fa-IR");
        }

        private static CultureInfo fa_culture;

        internal static DateTime ToDateTime(this string s,string foramt ="yyyy/MM/dd" )
        {
            return DateTime.ParseExact(s, foramt, fa_culture);
        }
    }
}
