using System.IO;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using UnityEngine;
#endif
using UnityEditor;

#if UNITY_EDITOR
namespace CGame.Config
{
    /*
     * 暂时不使用，这个应该配合编辑器使用，而不是静态类，静态类加载它的时候，运行阶段会出现很多问题。
     * 使用时应该类似，odin下拉框，设置下拉框的项等。
     */
#if ODIN_INSPECTOR
    public abstract class ConfigBase<T> : SerializedScriptableObject where T : ConfigBase<T>
#else
    public abstract class ConfigBase<T> : ScriptableObject where T : ConfigBase<T>
#endif
    {
        private static string _name = "c.game.config." + typeof(T).Name;
        private static string _path = "Assets/CGame/Config/" + typeof(T).Name + ".asset";

        public static T Instance
        {
            get
            {
                T data;
                if (EditorBuildSettings.TryGetConfigObject(_name, out data))
                    return data;
                
                if (File.Exists(_path))
                    data = AssetDatabase.LoadAssetAtPath<T>(_path);

                if (data == null)
                {
                    //show save file dialog and return user selected path name
                    _path = EditorUtility.SaveFilePanelInProject("新建配置文件", typeof(T).Name, "asset", "选择新建文件的位置");
                    //initialise config data object
                    data = CreateInstance<T>();
                    //create new asset from data at specified path
                    //asset MUST be saved with the AssetDatabase before adding to EditorBuildSettings
                    AssetDatabase.CreateAsset(data, _path);
                }
                EditorBuildSettings.AddConfigObject(_name, data, false);
                return data;
            }
        }
    }
}
#endif
