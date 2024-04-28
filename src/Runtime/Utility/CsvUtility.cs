using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CGame
{
    public static class CsvUtility
    {
        public static List<T> FromCsv<T>(string csv, bool ignoreHead = true, char separator = ',', char linefeed = '\n')
        {
            if (string.IsNullOrWhiteSpace(csv))
                return null;
            if (typeof(T) == null)
                throw new ArgumentNullException(typeof(T).Name);
            return !typeof(T).IsAbstract
                ? FromCsvInternal<T>(csv, ignoreHead, separator, linefeed)
                : throw new ArgumentException("Cannot deserialize Csv to new instances of type '" + typeof(T).Name + ".'");
        }
        
        private static List<T> FromCsvInternal<T>(string csv, bool ignoreHead = true, char separator = ',', char linefeed = '\n')
        {
            var result = new List<T>();
            var value = new List<string>();
            
            var stringBuilder = new StringBuilder();
            var start = false;
            for (var i = 0; i < csv.Length; i++)
            {
                var c = csv[i];
                
                if (i == csv.Length - 1 || (c == linefeed && !start))
                {
                    value.Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                    if (ignoreHead)
                        ignoreHead = false;
                    else
                        result.Add(ValueToStringUtility<T>.StringToValue(value));
                    value.Clear();
                }
                else if (c == separator && !start)
                {
                    value.Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                }
                else if (c == '"')
                {
                    if (!start)
                        start = true;
                    else if (i < csv.Length - 1 && csv[i + 1] == '"')
                    {
                        stringBuilder.Append('"');
                        i++;
                    }
                    else
                        start = false;
                }
                else
                    stringBuilder.Append(c);
            }

            return result;
        }
        
        public static bool FromCsvNonAlloc<T>(string csv, List<T> result, bool ignoreHead = true, char separator = ',', char linefeed = '\n')
        {
            if (string.IsNullOrWhiteSpace(csv))
                return false;
            if (typeof(T) == null)
                throw new ArgumentNullException(typeof(T).Name);
            return !typeof(T).IsAbstract
                ? FromCsvInternalNonAlloc<T>(csv, result, ignoreHead, separator, linefeed)
                : throw new ArgumentException("Cannot deserialize Csv to new instances of type '" + typeof(T).Name + ".'");
        }
        
        private static bool FromCsvInternalNonAlloc<T>(string csv, List<T> result, bool ignoreHead = true, char separator = ',', char linefeed = '\n')
        {
            result.Clear();
            var value = new List<string>();
            
            var stringBuilder = new StringBuilder();
            var start = false;
            for (var i = 0; i < csv.Length; i++)
            {
                var c = csv[i];
                
                if (i == csv.Length - 1 || (c == linefeed && !start))
                {
                    value.Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                    if (ignoreHead)
                        ignoreHead = false;
                    else
                        result.Add(ValueToStringUtility<T>.StringToValue(value));
                    value.Clear();
                }
                else if (c == separator && !start)
                {
                    value.Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                }
                else if (c == '"')
                {
                    if (!start)
                        start = true;
                    else if (i < csv.Length - 1 && csv[i + 1] == '"')
                    {
                        stringBuilder.Append('"');
                        i++;
                    }
                    else
                        start = false;
                }
                else
                    stringBuilder.Append(c);
            }

            return true;
        }
        
        public static int FromCsvNonAlloc<T>(string csv, T[] result, bool ignoreHead = true, char separator = ',', char linefeed = '\n')
        {
            if (string.IsNullOrWhiteSpace(csv))
                return -1;
            if (typeof(T) == null)
                throw new ArgumentNullException(typeof(T).Name);
            return !typeof(T).IsAbstract
                ? FromCsvInternalNonAlloc<T>(csv, result, ignoreHead, separator, linefeed)
                : throw new ArgumentException("Cannot deserialize Csv to new instances of type '" + typeof(T).Name + ".'");
        }
        
        private static int FromCsvInternalNonAlloc<T>(string csv, T[] result, bool ignoreHead = true, char separator = ',', char linefeed = '\n')
        {
            var length = 0;
            var value = new List<string>();
            
            var stringBuilder = new StringBuilder();
            var start = false;
            for (var i = 0; i < csv.Length; i++)
            {
                var c = csv[i];
                
                if (i == csv.Length - 1 || (c == linefeed && !start))
                {
                    value.Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                    if (ignoreHead)
                        ignoreHead = false;
                    else
                        result[length++] = ValueToStringUtility<T>.StringToValue(value);
                    value.Clear();
                }
                else if (c == separator && !start)
                {
                    value.Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                }
                else if (c == '"')
                {
                    if (!start)
                        start = true;
                    else if (i < csv.Length - 1 && csv[i + 1] == '"')
                    {
                        stringBuilder.Append('"');
                        i++;
                    }
                    else
                        start = false;
                }
                else
                    stringBuilder.Append(c);
            }

            return length;
        }

        public static string ToCsv<T>(T[] objs, bool ignoreHead = true, char separator = ',', char linefeed = '\n', params string[] customHeads)
        {
            if (!(objs?.Length > 0))
                return "";
            
            if (typeof(T) == null)
                throw new ArgumentNullException(typeof(T).Name);
            return !typeof(T).IsAbstract
                ? ToCsvInternal(objs, ignoreHead, separator, linefeed, customHeads)
                : throw new ArgumentException("CsvUtility.ToCsv does not support abstract types.");
        }
        
        public static string ToCsv<T>(List<T> objs, bool ignoreHead = true, char separator = ',', char linefeed = '\n', params string[] customHeads)
        {
            if (!(objs?.Count > 0))
                return "";
            
            if (typeof(T) == null)
                throw new ArgumentNullException(typeof(T).Name);
            return !typeof(T).IsAbstract
                ? ToCsvInternal(objs, ignoreHead, separator, linefeed, customHeads)
                : throw new ArgumentException("CsvUtility.ToCsv does not support abstract types.");
        }

        private static string ToCsvInternal<T>(IEnumerable<T> objs, bool ignoreHead = true, char separator = ',', char linefeed = '\n', params string[] customHeads)
        {
            var result = new StringBuilder();

            var allValues = new List<List<string>>();
            if (ignoreHead)
            {
                allValues.Add(customHeads.Length > 0 ? customHeads.ToList() : ValueToStringUtility<T>.GetAllMemberName());
            }
            allValues.AddRange(objs.Select(obj => ValueToStringUtility<T>.ValueToString(obj)));
            
            foreach (var values in allValues)
            {
                for (var i = 0; i < values.Count; i++)
                {
                    var value = values[i];
                    
                    if (value.Contains('"') || value.Contains(separator) || value.Contains(linefeed))
                        result.Append('"').Append(value.Replace(@"""", @"""""")).Append('"');
                    else
                        result.Append(value);
                    
                    if (i < values.Count - 1)
                        result.Append(separator);
                }
                result.Append(linefeed);
            }

            return result.ToString();
        }
        
        public static List<List<string>> CsvContentToData(string content, char separator = ',', char linefeed = '\n')
        {
            var result = new List<List<string>> { new() };
            var stringBuilder = new StringBuilder();
            var start = false;
            var index = 0;
            for (var i = 0; i < content.Length; i++)
            {
                var c = content[i];
                
                if (i == content.Length - 1 || (c == linefeed && !start))
                {
                    result[index].Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                    
                    index++;
                    if (i < content.Length - 1)
                        result.Add(new List<string>());
                }
                else if (c == separator && !start)
                {
                    result[index].Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                }
                else if (c == '"')
                {
                    if (!start)
                        start = true;
                    else if (i < content.Length - 1 && content[i + 1] == '"')
                    {
                        stringBuilder.Append('"');
                        i++;
                    }
                    else
                        start = false;
                }
                else
                    stringBuilder.Append(c);
            }

            return result;
        }
        
        public static void CsvContentToDataNonAlloc(string content, List<List<string>> result, char separator = ',', char linefeed = '\n')
        {
            result.Clear();
            result.Add(new List<string>());
            var stringBuilder = new StringBuilder();
            var start = false;
            var index = 0;
            for (var i = 0; i < content.Length; i++)
            {
                var c = content[i];
                
                if (i == content.Length - 1 || (c == linefeed && !start))
                {
                    result[index].Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                    
                    index++;
                    if (i < content.Length - 1)
                        result.Add(new List<string>());
                }
                else if (c == separator && !start)
                {
                    result[index].Add(stringBuilder.ToString());
                    stringBuilder.Clear();
                }
                else if (c == '"')
                {
                    if (!start)
                        start = true;
                    else if (i < content.Length - 1 && content[i + 1] == '"')
                    {
                        stringBuilder.Append('"');
                        i++;
                    }
                    else
                        start = false;
                }
                else
                    stringBuilder.Append(c);
            }
        }
        
        public static (int length0, int length1) CsvContentToDataNonAlloc(string content, string[,] result, char separator = ',', char linefeed = '\n')
        {
            int length0 = 0, length1 = 0;
            
            var stringBuilder = new StringBuilder();
            var start = false;
            int index0 = 0, index1 = 0;
            for (var i = 0; i < content.Length; i++)
            {
                var c = content[i];
                
                if (i == content.Length - 1 || (c == linefeed && !start))
                {
                    result[index0, index1] = stringBuilder.ToString();
                    stringBuilder.Clear();
                    
                    index0++;
                    index1++;
                    length1 = Mathf.Max(length1, index1);
                    index1 = 0;
                }
                else if (c == separator && !start)
                {
                    result[index0, index1] = stringBuilder.ToString();
                    index1++;
                    stringBuilder.Clear();
                }
                else if (c == '"')
                {
                    if (!start)
                        start = true;
                    else if (i < content.Length - 1 && content[i + 1] == '"')
                    {
                        stringBuilder.Append('"');
                        i++;
                    }
                    else
                        start = false;
                }
                else
                    stringBuilder.Append(c);
            }

            length0 = index0;
            return (length0, length1);
        }
        
        public static string DataToCsvContent(List<List<string>> data, char separator = ',', char linefeed = '\n')
        {
            var result = new StringBuilder();
            
            foreach (var row in data)
            {
                for (var i = 0; i < row.Count; i++)
                {
                    var value = row[i]; 
                    
                    if (value.Contains('"') || value.Contains(separator) || value.Contains(linefeed))
                        result.Append('"').Append(value.Replace(@"""", @"""""")).Append('"');
                    else
                        result.Append(value);
                    
                    if (i < row.Count - 1)
                        result.Append(separator);
                }
                result.Append(linefeed);
            }

            return result.ToString();
        }
        
        public static string DataToCsvContent(string[,] data, char separator = ',', char linefeed = '\n')
        {
            var result = new StringBuilder();

            for (var i = 0; i < data.GetLength(0); i++)
            {
                for (var j = 0; j < data.GetLength(1); j++)
                {
                    var value = data[i, j]; 
                    
                    if (value.Contains('"') || value.Contains(separator) || value.Contains(linefeed))
                        result.Append('"').Append(value.Replace(@"""", @"""""")).Append('"');
                    else
                        result.Append(value);
                    
                    if (j < data.GetLength(1) - 1)
                        result.Append(separator);
                }
                result.Append(linefeed);
            }
            
            return result.ToString();
        }
    }
}