using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YT_DLP_EZDown
{
    internal class Functions
    {
        public static Boolean isURL(string input)
        {
            return Regex.IsMatch(input, @"^(http|https)://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?$");
        }
    }
}
