using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace CGame
{
    public static class NewJsonFileController
    {
        public const string Extension = ".json";
        
        private static bool CheckFilePath(string path)
        {
            path = Path.ChangeExtension(path, Extension);
            return path.GetPathState().HasFlag(PathState.File);
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
                return string.IsNullOrEmpty(text) ? null : JsonConvert.DeserializeObject<IEnumerable<T>>(text);
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

                values.AddRange(JsonConvert.DeserializeObject<List<T>>(text));
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

                var result = JsonConvert.DeserializeObject<T[]>(text);
                for (var i = 0; i < result.Length; i++)
                {
                    values[i] = result[i];
                }

                return result.Length;
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

            var json = JsonConvert.SerializeObject(values);

            var serializer = new JsonSerializer();
            var tr = new StringReader(json);
            var jtr = new JsonTextReader(tr);
            var obj = serializer.Deserialize(jtr);
            var textWriter = new StringWriter();
            var jsonWriter = new JsonTextWriter(textWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            };
            serializer.Serialize(jsonWriter, obj);

            File.WriteAllText(path, textWriter.ToString(), Encoding.UTF8);
#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }
    }
}