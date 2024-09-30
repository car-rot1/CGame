using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace CGame.Editor
{
    public class PropertyHandlerExtension
    {
        private readonly object _propertyHandle;
        private readonly Type _propertyHandleType;
        
        public PropertyHandlerExtension(object propertyHandle)
        {
            _propertyHandle = propertyHandle;
            _propertyHandleType = _propertyHandle.GetType();
        }
        
        private PropertyInfo _propertyDrawerPropertyInfo;
        public PropertyDrawer PropertyDrawer
        {
            get
            {
                if (_propertyDrawerPropertyInfo == null)
                    _propertyDrawerPropertyInfo = _propertyHandleType.GetProperty("propertyDrawer", BindingFlags.Instance | BindingFlags.NonPublic);
                return (PropertyDrawer)_propertyDrawerPropertyInfo!.GetValue(_propertyHandle);
            }
        }
        
        private PropertyInfo _decoratorDrawersPropertyInfo;
        public List<DecoratorDrawer> DecoratorDrawers
        {
            get
            {
                if (_decoratorDrawersPropertyInfo == null)
                    _decoratorDrawersPropertyInfo = _propertyHandleType.GetProperty("decoratorDrawers", BindingFlags.Instance | BindingFlags.NonPublic);
                return (List<DecoratorDrawer>)_decoratorDrawersPropertyInfo!.GetValue(_propertyHandle);
            }
        }

        private MethodInfo _getHeightMethodInfo;
        public float GetHeight(SerializedProperty property, GUIContent label, bool includeChildren)
        {
            if (_getHeightMethodInfo == null)
                _getHeightMethodInfo = _propertyHandleType.GetMethod("GetHeight");
            return (float)_getHeightMethodInfo!.Invoke(_propertyHandle, new object[] { property, label, includeChildren });
        }
    }
}