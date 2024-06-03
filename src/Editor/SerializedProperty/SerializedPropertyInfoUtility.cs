using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CGame.Editor
{
    //todo 这个在搜索过程中可以利用SerializedProperty的Path属性；
    public static class SerializedPropertyInfoUtility
    {
        private const BindingFlags AllBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        
        private static readonly Dictionary<Object, List<SerializedPropertyInfo>> SerializedPropertyInfoDic = new();
        
        private static void Add(Object targetObject)
        {
            if (SerializedPropertyInfoDic.ContainsKey(targetObject))
                return;
            SerializedPropertyInfoDic.Add(targetObject, new List<SerializedPropertyInfo>());
            
            var serializedObject = new SerializedObject(targetObject);
            
            var depthSerializedProperties = new List<List<(SerializedProperty serializedProperty, bool visible)>>();
            var iterator = serializedObject.GetIterator();
            while (iterator.Next(true))
            {
                if (depthSerializedProperties.Count <= iterator.depth)
                    depthSerializedProperties.Add(new List<(SerializedProperty serializedProperty, bool visible)>());
                depthSerializedProperties[iterator.depth].Add((iterator.Copy(), false));
            }

            iterator.Reset();
            while (iterator.NextVisible(true))
            {
                var serializedProperties = depthSerializedProperties[iterator.depth];
                for (var i = 0; i < serializedProperties.Count; i++)
                {
                    if (!serializedProperties[i].serializedProperty.propertyPath.Equals(iterator.propertyPath))
                        continue;
                    serializedProperties[i] = (serializedProperties[i].serializedProperty, true);
                    break;
                }
            }
            
            var depthSerializedPropertyInfos = new List<List<SerializedPropertyInfo>>();
            for (var depth = 0; depth < depthSerializedProperties.Count; depth++)
            {
                if (depthSerializedPropertyInfos.Count <= depth)
                {
                    depthSerializedPropertyInfos.Add(new List<SerializedPropertyInfo>());
                }
                
                foreach (var (serializedProperty, visible) in depthSerializedProperties[depth])
                {
                    if (depth == 0)
                    {
                        var fieldInfo = targetObject.GetType().GetField(serializedProperty.name, AllBindingFlags);
                        if (fieldInfo != null)
                        {
                            depthSerializedPropertyInfos[serializedProperty.depth].Add(
                                new SerializedPropertyInfo(
                                    serializedProperty,
                                    visible,
                                    fieldInfo,
                                    targetObject,
                                    null)
                            );
                        }
                        else
                        {
                            depthSerializedPropertyInfos[serializedProperty.depth].Add(new SerializedPropertyInfo(
                                serializedProperty,
                                visible,
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
                                        visible,
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
                                    visible,
                                    null,
                                    null,
                                    null)
                            );
                        }
                    }
                }
            }
            
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
            if (obj == null)
                return null;
            Add(obj);
            return SerializedPropertyInfoDic[obj];
        }
        
        public static List<SerializedPropertyInfo> Get(SerializedObject serializedObject)
        {
            if (serializedObject == null)
                return null;
            return Get(serializedObject.targetObject);
        }
    }
}