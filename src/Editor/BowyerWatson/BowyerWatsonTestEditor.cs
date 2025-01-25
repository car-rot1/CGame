using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BowyerWatson.Editor
{
    [CustomEditor(typeof(BowyerWatsonTest))]
    public class BowyerWatsonTestEditor : UnityEditor.Editor
    {
        private Type _type;
        private MethodInfo _createRandomPoints;
        private MethodInfo _bowyerWatsonInit;
        private MethodInfo _bowyerWatsonHandle;
        private MethodInfo _bowyerWatsonHandleItem;
        private MethodInfo _bowyerWatsonCheck;
        private MethodInfo _aStarInit;
        private MethodInfo _aStarHandle;
        private MethodInfo _funnelAlgorithmHandle;
        
        private SerializedProperty _index;
        
        private SceneView _sceneView;
        
        private void OnEnable()
        {
            _type = serializedObject.targetObject.GetType();
            _createRandomPoints = _type.GetMethod("CreateRandomPoints", BindingFlags.Instance | BindingFlags.Public);
            _bowyerWatsonInit = _type.GetMethod("BowyerWatsonInit", BindingFlags.Instance | BindingFlags.Public);
            _bowyerWatsonHandle = _type.GetMethod("BowyerWatsonHandle", BindingFlags.Instance | BindingFlags.Public);
            _bowyerWatsonHandleItem = _type.GetMethod("BowyerWatsonHandleItem", BindingFlags.Instance | BindingFlags.Public);
            _bowyerWatsonCheck = _type.GetMethod("BowyerWatsonCheck", BindingFlags.Instance | BindingFlags.Public);
            _aStarInit = _type.GetMethod("AStarInit", BindingFlags.Instance | BindingFlags.Public);
            _aStarHandle = _type.GetMethod("AStarHandle", BindingFlags.Instance | BindingFlags.Public);
            _funnelAlgorithmHandle = _type.GetMethod("FunnelAlgorithmHandle", BindingFlags.Instance | BindingFlags.Public);
            
            _index = serializedObject.FindProperty("index");

            var array = Resources.FindObjectsOfTypeAll<SceneView>();
            if (array is { Length: > 0 })
            {
                _sceneView = array[0];
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("CreateRandomPoints"))
            {
                _createRandomPoints.Invoke(serializedObject.targetObject, null);
                RefreshSceneView();
            }
            
            if (GUILayout.Button("BowyerWatsonInit"))
            {
                _bowyerWatsonInit.Invoke(serializedObject.targetObject, null);
                RefreshSceneView();
            }

            if (GUILayout.Button("BowyerWatsonHandle"))
            {
                _bowyerWatsonHandle.Invoke(serializedObject.targetObject, null);
                RefreshSceneView();
            }
            
            if (GUILayout.Button("BowyerWatsonHandleItem"))
            {
                _bowyerWatsonHandleItem.Invoke(serializedObject.targetObject, null);
                _index.intValue++;
                RefreshSceneView();
            }
            
            if (GUILayout.Button("BowyerWatsonCheck"))
            {
                _bowyerWatsonCheck.Invoke(serializedObject.targetObject, null);
                RefreshSceneView();
            }
            
            if (GUILayout.Button("AStarInit"))
            {
                _aStarInit.Invoke(serializedObject.targetObject, null);
                RefreshSceneView();
            }
            
            if (GUILayout.Button("AStarHandle"))
            {
                _aStarHandle.Invoke(serializedObject.targetObject, null);
                RefreshSceneView();
            }
            
            if (GUILayout.Button("FunnelAlgorithmHandle"))
            {
                _funnelAlgorithmHandle.Invoke(serializedObject.targetObject, null);
                RefreshSceneView();
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private void RefreshSceneView()
        {
            if (_sceneView != null)
            {
                _sceneView.Repaint();
            }
        }
    }
}