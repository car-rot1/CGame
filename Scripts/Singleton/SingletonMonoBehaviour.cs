using System;
using System.Collections.Generic;
using UnityEngine;

namespace CGame
{
    public abstract class SingletonMonoBehaviour : MonoBehaviour
    {
        
    }

    public abstract class SingletonMonoBehaviour<T> : SingletonMonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        private static readonly Lazy<T> LazyInstance = new(GetInstance);
        public static T Instance => LazyInstance.Value;
        private static T GetInstance()
        {
            var result = FindObjectsOfType<T>();
            
            if (result == null || result.Length == 0)
            {
                Debug.LogError($"不存在{typeof(T).Name}单例，请先创建该单例对应的游戏物体");
                return null;
            }
            if (result.Length > 1)
            {
                Debug.LogError($"{typeof(T).Name}组件为单例组件，场景中不能存在多个，请删除多余的游戏物体或组件");
                return null;
            }
            result[0].Init();
            return result[0];
        }
        protected virtual void Init() { }
    }
}