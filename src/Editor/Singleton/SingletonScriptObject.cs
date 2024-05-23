using System;
using UnityEditor;
using UnityEngine;

namespace CGame.Editor
{
    [CreateAssetMenu]
    public class SingletonScriptObject<T> : ScriptableObject where T : SingletonScriptObject<T>
    {
        private static readonly Lazy<T> LazyInstance = new(GetInstance);
        public static T Instance => LazyInstance.Value;

        private static T GetInstance()
        {
            var assets = AssetDatabase.FindAssets($"t:{typeof(T).Name}");

            if (assets == null || assets.Length == 0)
            {
                Debug.LogError($"不存在{typeof(T).Name}单例，请先创建该单例对应的ScriptObject");
                return null;
            }

            if (assets.Length > 1)
            {
                Debug.LogError($"{typeof(T).Name}组件为单例组件，项目资源中不能存在多个，请删除多余的ScriptObject");
                return null;
            }

            var result = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(assets[0]));
            result.Init();
            return result;
        }

        protected virtual void Init()
        {
        }
    }
}