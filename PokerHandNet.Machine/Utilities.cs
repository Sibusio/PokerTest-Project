using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerHandNet.Machine
{
    class Utilities
    {
        /// <summary>
        /// Converts and enum to a presentable title.
        /// </summary>
        /// <param name="enumToConvert">The enum to be converted.</param>
        /// <returns>A presentable title.</returns>
        public static string EnumToTitle(Enum enumToConvert)
        {
            return System.Text.RegularExpressions.Regex
            .Replace(enumToConvert.ToString(), "[A-Z]", " $0").Trim();
        }
    }
}
