using System;
using System.Collections.Generic;

namespace CGame
{
    public static class GlobalEvent
    {
        private static readonly Dictionary<string, List<Delegate>> _listenerDic = new();

        public static void SendEvent<T>(T value)
        {
            if (!_listenerDic.TryGetValue(typeof(T).Name, out var listeners))
                return;

            foreach (var listener in listeners)
            {
                if (listener is Action<T> action)
                    action.Invoke(value);
            }
        }

        public static void SendEvent(string key)
        {
            if (!_listenerDic.TryGetValue(key, out var listeners))
                return;

            foreach (var listener in listeners)
            {
                if (listener is Action action)
                    action.Invoke();
            }
        }

        public static void SendEvent<T>(string key, T value)
        {
            if (!_listenerDic.TryGetValue(key, out var listeners))
                return;

            foreach (var listener in listeners)
            {
                if (listener is Action<T> action)
                    action.Invoke(value);
            }
        }

        public static void AddListener<T>(Action<T> action)
        {
            _listenerDic.TryAdd(typeof(T).Name, new List<Delegate>());
            _listenerDic[typeof(T).Name].Add(action);
        }

        public static void RemoveListener<T>(Action<T> action)
        {
            _listenerDic.TryAdd(typeof(T).Name, new List<Delegate>());
            _listenerDic[typeof(T).Name].Remove(action);
        }
        
        public static void AddListener(string key, Action action)
        {
            _listenerDic.TryAdd(key, new List<Delegate>());
            _listenerDic[key].Add(action);
        }

        public static void RemoveListener(string key, Action action)
        {
            _listenerDic.TryAdd(key, new List<Delegate>());
            _listenerDic[key].Remove(action);
        }
        
        public static void AddListener<T>(string key, Action<T> action)
        {
            _listenerDic.TryAdd(key, new List<Delegate>());
            _listenerDic[key].Add(action);
        }

        public static void RemoveListener<T>(string key, Action<T> action)
        {
            _listenerDic.TryAdd(key, new List<Delegate>());
            _listenerDic[key].Remove(action);
        }
    }
}