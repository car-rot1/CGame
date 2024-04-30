using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using Object = UnityEngine.Object;

namespace CGame.Editor
{
    public class SerializedPropertyInfo
    {
        public SerializedProperty serializedProperty;
        public FieldInfo fieldInfo;
        public object Value => fieldInfo.GetValue(owner);
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
            
            var depthSerializedProperties = new List<List<SerializedProperty>>();
            var iterator = serializedObject.GetIterator();
            while (iterator.Next(true))
            {
                if (depthSerializedProperties.Count <= iterator.depth)
                    depthSerializedProperties.Add(new List<SerializedProperty>());
                depthSerializedProperties[iterator.depth].Add(iterator.Copy());
            }
            
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
                            depthSerializedPropertyInfos[serializedProperty.depth].Add(new SerializedPropertyInfo
                            {
                                serializedProperty = serializedProperty,
                                fieldInfo = fieldInfo,
                                owner = targetObject
                            });
                        }
                        else
                        {
                            depthSerializedPropertyInfos[serializedProperty.depth].Add(new SerializedPropertyInfo
                            {
                                serializedProperty = serializedProperty
                            });
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
                                depthSerializedPropertyInfos[serializedProperty.depth].Add(new SerializedPropertyInfo
                                {
                                    serializedProperty = serializedProperty,
                                    fieldInfo = fieldInfo,
                                    owner = lastInfo.Value
                                });
                                break;
                            }
                        }

                        if (fieldInfo == null)
                        {
                            depthSerializedPropertyInfos[serializedProperty.depth].Add(new SerializedPropertyInfo
                            {
                                serializedProperty = serializedProperty
                            });
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
            Add(obj);
            return SerializedPropertyInfoDic[obj];
        }
        
        public static List<SerializedPropertyInfo> Get(SerializedObject serializedObject)
        {
            return Get(serializedObject.targetObject);
        }
    }
}