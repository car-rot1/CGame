using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CGame.Editor
{
    public static class SerializedPropertyInfoUtility
    {
        private const BindingFlags AllBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        
        private static readonly Dictionary<Object, List<SerializedPropertyInfo>> SerializedPropertyInfoDic = new();
        private static readonly Dictionary<Object, List<SerializedPropertyInfo>> VisibleSerializedPropertyInfoDic = new();

        private static List<List<SerializedPropertyInfo>> ToDepthSerializedPropertyInfos(List<List<SerializedProperty>> depthSerializedProperties, Object targetObject)
        {
            var depthSerializedPropertyInfos = new List<List<SerializedPropertyInfo>>();
            for (var depth = 0; depth < depthSerializedProperties.Count; depth++)
            {
                if (depthSerializedPropertyInfos.Count <= depth)
                {
                    depthSerializedPropertyInfos.Add(new List<SerializedPropertyInfo>());
                }
                
                foreach (var serializedProperty in depthSerializedProperties[depth])
                {
                    if (depth == 0)
                    {
                        var fieldInfo = targetObject.GetType().GetField(serializedProperty.name, AllBindingFlags);
                        if (fieldInfo != null)
                        {
                            depthSerializedPropertyInfos[serializedProperty.depth].Add(
                                new SerializedPropertyInfo(
                                    serializedProperty,
                                    fieldInfo,
                                    targetObject,
                                    null)
                            );
                        }
                        else
                        {
                            depthSerializedPropertyInfos[serializedProperty.depth].Add(new SerializedPropertyInfo(
                                serializedProperty,
                                null,
                                null,
                                null)
                            );
                        }
                    }
                    else
                    {
                        FieldInfo fieldInfo = null;
                        foreach (var lastInfo in depthSerializedPropertyInfos[serializedProperty.depth - 1].Where(lastInfo => lastInfo.fieldInfo != null))
                        {
                            fieldInfo = lastInfo.fieldInfo.FieldType.GetField(serializedProperty.name, AllBindingFlags);
                            if (fieldInfo != null)
                            {

                                depthSerializedPropertyInfos[serializedProperty.depth].Add(
                                    new SerializedPropertyInfo(
                                        serializedProperty,
                                        fieldInfo,
                                        lastInfo.Value,
                                        lastInfo)
                                );
                                break;
                            }
                        }

                        if (fieldInfo == null)
                        {
                            depthSerializedPropertyInfos[serializedProperty.depth].Add(
                                new SerializedPropertyInfo(
                                    serializedProperty,
                                    null,
                                    null,
                                    null)
                            );
                        }
                    }
                }
            }
            return depthSerializedPropertyInfos;
        }
        
        private static void Add(Object targetObject)
        {
            if (SerializedPropertyInfoDic.ContainsKey(targetObject))
                return;
            SerializedPropertyInfoDic.Add(targetObject, new List<SerializedPropertyInfo>());
            
            var serializedObject = new SerializedObject(targetObject);
            
            var depthSerializedProperties = new List<List<SerializedProperty>>();
            var iterator = serializedObject.GetIterator();
            while (iterator.Next(true))
            {
                if (depthSerializedProperties.Count <= iterator.depth)
                    depthSerializedProperties.Add(new List<SerializedProperty>());
                depthSerializedProperties[iterator.depth].Add(iterator.Copy());
            }

            var depthSerializedPropertyInfos = ToDepthSerializedPropertyInfos(depthSerializedProperties, targetObject);
            
            foreach (var serializedPropertyInfos in depthSerializedPropertyInfos)
            {
                foreach (var serializedPropertyInfo in serializedPropertyInfos)
                {
                    SerializedPropertyInfoDic[targetObject].Add(serializedPropertyInfo);
                }
            }
        }

        public static List<SerializedPropertyInfo> Get(Object obj)
        {
            Add(obj);
            return SerializedPropertyInfoDic[obj];
        }
        
        public static List<SerializedPropertyInfo> Get(SerializedObject serializedObject)
        {
            return Get(serializedObject.targetObject);
        }
        
        private static void AddVisible(Object targetObject)
        {
            if (VisibleSerializedPropertyInfoDic.ContainsKey(targetObject))
                return;
            VisibleSerializedPropertyInfoDic.Add(targetObject, new List<SerializedPropertyInfo>());
            
            var serializedObject = new SerializedObject(targetObject);
            
            var depthSerializedProperties = new List<List<SerializedProperty>>();
            var iterator = serializedObject.GetIterator();
            while (iterator.NextVisible(true))
            {
                if (depthSerializedProperties.Count <= iterator.depth)
                    depthSerializedProperties.Add(new List<SerializedProperty>());
                depthSerializedProperties[iterator.depth].Add(iterator.Copy());
            }

            var depthSerializedPropertyInfos = ToDepthSerializedPropertyInfos(depthSerializedProperties, targetObject);
            
            foreach (var serializedPropertyInfos in depthSerializedPropertyInfos)
            {
                foreach (var serializedPropertyInfo in serializedPropertyInfos)
                {
                    VisibleSerializedPropertyInfoDic[targetObject].Add(serializedPropertyInfo);
                }
            }
        }
        
        public static List<SerializedPropertyInfo> GetVisible(Object obj)
        {
            AddVisible(obj);
            return VisibleSerializedPropertyInfoDic[obj];
        }
        
        public static List<SerializedPropertyInfo> GetVisible(SerializedObject serializedObject)
        {
            return GetVisible(serializedObject.targetObject);
        }
    }
}