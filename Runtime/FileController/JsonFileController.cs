using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace CGame
{
    public static class JsonFileController
    {
        public const string Extension = ".json";
        
        public const string Pattern = @"\{([^{}]*)\}";
        
        private static bool CheckFilePath(string path)
        {
            path = Path.ChangeExtension(path, Extension);
            return !path.GetPathState().ContainsAll(PathState.Invalid, PathState.Directory);
        }

        public static IEnumerable<T> GetValue<T>(string path)
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return null;
            }
            
            if (path.GetPathState().HasFlag(PathState.Exist))
            {
                var text = File.ReadAllText(path, Encoding.UTF8);
                if (string.IsNullOrEmpty(text))
                    return null;
            
                var matchValues = Regex.Matches(text, Pattern);

                var values = new T[matchValues.Count];
                for (var i = 0; i < matchValues.Count; i++)
                    values[i] = JsonUtility.FromJson<T>(matchValues[i].Value);

                return values;
            }
            
            Debug.LogError("文件不存在！");
            return null;
        }
        
        public static bool GetValueNonAlloc<T>(string path, List<T> values)
        {
            values.Clear();
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return false;
            }
            
            if (path.GetPathState().HasFlag(PathState.Exist))
            {
                var text = File.ReadAllText(path, Encoding.UTF8);
                if (string.IsNullOrEmpty(text))
                    return true;
            
                var matchValues = Regex.Matches(text, Pattern);
                
                for (var i = 0; i < matchValues.Count; i++)
                    values.Add(JsonUtility.FromJson<T>(matchValues[i].Value));

                return true;
            }
            
            Debug.LogError("文件不存在！");
            return false;
        }

        public static int GetValueNonAlloc<T>(string path, T[] values)
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return -1;
            }
            
            if (path.GetPathState().HasFlag(PathState.Exist))
            {
                var text = File.ReadAllText(path, Encoding.UTF8);
                if (string.IsNullOrEmpty(text))
                    return 0;
            
                var matchValues = Regex.Matches(text, Pattern);
                
                for (var i = 0; i < matchValues.Count; i++)
                    values[i] = JsonUtility.FromJson<T>(matchValues[i].Value);

                return matchValues.Count;
            }
            
            Debug.LogError("文件不存在！");
            return -1;
        }

        public static void SetValue<T>(string path, IEnumerable<T> values)
        {
            if (!CheckFilePath(path))
            {
                Debug.LogError("路径有误！");
                return;
            }

            var array = values as T[] ?? values.ToArray();
            if (array.Length <= 1)
            {
                File.WriteAllText(path, JsonUtility.ToJson(array[0]), Encoding.UTF8);
                return;
            }

            var stringBuilder = new StringBuilder();

            stringBuilder.Append("{");
            stringBuilder.AppendFormat("\"{0}_Array_{1}\":", typeof(T).Name, array.Length);
            stringBuilder.Append("[");
            stringBuilder.Append(string.Join(',', array.Select(o => JsonUtility.ToJson(o))));
            stringBuilder.Append("]");
            stringBuilder.Append("}");

            var json = stringBuilder.ToString();
            var serializer = new JsonSerializer();
            var tr = new StringReader(json);
            var jtr = new JsonTextReader(tr);
            var obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                var textWriter = new StringWriter();
                var jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                json = textWriter.ToString();
            }
            File.WriteAllText(path, json, Encoding.UTF8);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
    }
}