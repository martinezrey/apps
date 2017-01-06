using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions.StringExtensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmptyOrWhiteSpace(this string value)
        {
            if (String.IsNullOrEmpty(value) || String.IsNullOrWhiteSpace(value))
                return true;

            return false;
        }
    }
}
