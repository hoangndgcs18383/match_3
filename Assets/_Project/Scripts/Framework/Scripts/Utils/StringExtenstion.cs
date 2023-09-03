using System;
using System.Collections.Generic;
using System.Linq;

namespace Zeff.Extensions
{
    public static class StringExtenstion
    {
        public static string ToSlash(this string str)
        {
            return str.Replace("\\", "/");
        }

        /// <summary>
        /// Example: "/StringExtenstion.cs" => "StringExtenstion.cs"
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        
        public static string ToBackSlash(this string str)
        {
            return str.Replace("/", "\\");
        }
        
        /// <summary>
        /// Example: "ID:0,NAME:Zeff,AGE:30" => [KEY] = ID, [VALUE] = 0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Dictionary<string, int> ToDictionaryStringInt(this string str)
        {
            return str.Split(",").Select(x => x.Split(":")).ToDictionary(x => x[0], x => int.Parse(x[1]));
        }
        
        public static Dictionary<string, int> TryToParserDictionary(this string str)
        {
            try
            {
                var keyValuePairs = str.Split(',').Select(x => x.Trim().Split(':'));

                Dictionary<string, int> result = new Dictionary<string, int>();

                foreach (var pair in keyValuePairs)
                {
                    if (pair.Length == 2)
                    {
                        string key = pair[0].Trim('"'); // Remove quotes if present
                        int value;
                        if (int.TryParse(pair[1], out value))
                        {
                            result[key] = value;
                        }
                        else
                        {
                            // Handle parsing error if necessary
                        }
                    }
                    else
                    {
                        // Handle invalid format if necessary
                    }
                }

                return result;

            }
            catch (Exception e)
            {
                return null;
            }
        }
        
        public static int TryToParserInt(this string str)
        {
            try
            {
                var keyValuePairs = str.Split(',').Select(x => x.Trim().Split(':'));
                return int.Parse(keyValuePairs.First()[1]);
            }
            catch (Exception e)
            {
                return 0;
            }
        }
    }
}