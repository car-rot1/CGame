using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace CGame
{
    public static class LogEditor
    {
        [Conditional("UNITY_EDITOR")]
        public static void Error(object message) => Debug.unityLogger.Log(LogType.Error, message);
        
        [Conditional("UNITY_EDITOR")]
        public static void Error(object message, Object context) => Debug.unityLogger.Log(LogType.Error, message, context);
        
        [Conditional("UNITY_EDITOR")]
        public static void Assert(object message) => Debug.unityLogger.Log(LogType.Assert, message);
        
        [Conditional("UNITY_EDITOR")]
        public static void Assert(object message, Object context) => Debug.unityLogger.Log(LogType.Assert, message, context);
        
        [Conditional("UNITY_EDITOR")]
        public static void Warning(object message) => Debug.unityLogger.Log(LogType.Warning, message);
        
        [Conditional("UNITY_EDITOR")]
        public static void Warning(object message, Object context) => Debug.unityLogger.Log(LogType.Warning, message, context);
        
        [Conditional("UNITY_EDITOR")]
        public static void Message(object message) => Debug.unityLogger.Log(LogType.Log, message);
        
        [Conditional("UNITY_EDITOR")]
        public static void Message(object message, Object context) => Debug.unityLogger.Log(LogType.Log, message, context);
        
        [Conditional("UNITY_EDITOR")]
        public static void Exception(object message) => Debug.unityLogger.Log(LogType.Exception, message);
        
        [Conditional("UNITY_EDITOR")]
        public static void Exception(object message, Object context) => Debug.unityLogger.Log(LogType.Exception, message, context);

        static LogEditor()
        {
            // Application.logMessageReceived += (condition, trace, type) =>
            // {
            //     
            // };
        }
    }
}