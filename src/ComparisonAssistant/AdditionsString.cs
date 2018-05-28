using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComparisonAssistant
{
    public static class AdditionsString
    {
        public static string ReplaseStartText(this string text, KeyValuePair<string, string> keyValue, bool addPostfix = false)
        {
            string find = keyValue.Key;
            if (addPostfix)
                find = find + "s";

            if (text.StartsWith(find))
                return keyValue.Value + text.Substring(find.Length);
            else
                return text;
        }

        public static string RemoveStartText(this string text, string find)
        {
            if (text.StartsWith(find))
                return text.Substring(find.Length);
            else
                return text;
        }

        public static string RemoveEndText(this string text, string find)
        {
            if (text.EndsWith(find))
                return text.Remove(text.Length - find.Length);
            else
                return text;
        }

        public static string RemoveSpace(this string text)
        {
            return text.Replace(" ", "");
        }
    }
}
