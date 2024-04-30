using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CGame.Editor
{
    public class SerializedPropertyInfo
    {
        public SerializedProperty serializedProperty;
        public Type type;
        public object value;
        public object owner;

        private SerializedPropertyInfo()
        {
            
        }
        
        private const BindingFlags AllBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        
        private static readonly Dictionary<Object, List<SerializedPropertyInfo>> SerializedPropertyInfoDic = new();

        private static void Add(Object targetObject)
        {
            if (SerializedPropertyInfoDic.ContainsKey(targetObject))
                return;
            
            SerializedPropertyInfoDic.Add(targetObject, new List<SerializedPropertyInfo>());
            
            var depthSerializedPropertyInfos = new List<List<SerializedPropertyInfo>>();
            var serializedObject = new SerializedObject(targetObject);
            
            var serializedProperties = new List<SerializedProperty>();
            var iterator = serializedObject.GetIterator();
            while (iterator.Next(true))
                serializedProperties.Add(iterator.Copy());
            serializedProperties.Sort((serializedProperty0, serializedProperty1) => serializedProperty0.depth - serializedProperty1.depth);
            
            foreach (var serializedProperty in serializedProperties)
            {
                if (depthSerializedPropertyInfos.Count <= serializedProperty.depth)
                {
                    depthSerializedPropertyInfos.Add(new List<SerializedPropertyInfo>());
                }

                if (serializedProperty.depth == 0)
                {
                    var fieldInfo = targetObject.GetType().GetField(serializedProperty.name, AllBindingFlags);
                    if (fieldInfo != null)
                    {
                        depthSerializedPropertyInfos[serializedProperty.depth].Add(new SerializedPropertyInfo
                        {
                            serializedProperty = serializedProperty,
                            type = fieldInfo.FieldType,
                            value = fieldInfo.GetValue(targetObject),
                            owner = targetObject
                        });
                    }
                }
                else
                {
                    foreach (var lastInfo in depthSerializedPropertyInfos[serializedProperty.depth - 1])
                    {
                        var fieldInfo = lastInfo.type.GetField(serializedProperty.name, AllBindingFlags);
                        if (fieldInfo != null)
                        {
                            depthSerializedPropertyInfos[serializedProperty.depth].Add(new SerializedPropertyInfo
                            {
                                serializedProperty = serializedProperty,
                                type = fieldInfo.FieldType,
                                value = fieldInfo.GetValue(lastInfo.value),
                                owner = lastInfo.value
                            });
                            break;
                        }
                    }
                }
            }
            
            foreach (var depthSerializedPropertyInfo in depthSerializedPropertyInfos)
            {
                foreach (var info in depthSerializedPropertyInfo)
                {
                    SerializedPropertyInfoDic[targetObject].Add(info);
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
    }
}