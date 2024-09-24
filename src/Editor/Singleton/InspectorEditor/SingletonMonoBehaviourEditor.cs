#if ODIN_INSPECTOR_3
using Sirenix.OdinInspector.Editor;
#endif
using UnityEditor;
using UnityEngine;

namespace CGame.Editor
{
    [CustomEditor(typeof(SingletonMonoBehaviour), true)]
#if ODIN_INSPECTOR_3
    public class SingletonMonoBehaviourEditor : OdinEditor
#else
    public class SingletonMonoBehaviourEditor : UnityEditor.Editor
#endif
    {
        private bool _logError;
    
#if ODIN_INSPECTOR_3
        protected override void OnEnable()
#else
        private void OnEnable()
#endif
        {
            EditorApplication.update += Update;
            DragAndDrop.AddDropHandler(InspectorDropHandler);
        }
    
        private DragAndDropVisualMode InspectorDropHandler(Object[] targets, bool perform)
        {
            foreach (var objectReference in DragAndDrop.objectReferences)
            {
                if (objectReference is MonoScript monoScript && monoScript.GetClass() == target.GetType())
                {
                    if (!_logError)
                    {
                        _logError = true;
                        // Debug.LogError("该组件为单例，只能添加一个", target);
                    }
    
                    return DragAndDropVisualMode.None;
                }
            }
    
            return DragAndDropVisualMode.None;
        }
    
        private void Update()
        {
            if (EditorWindow.mouseOverWindow == null || EditorWindow.mouseOverWindow.titleContent?.text != "Inspector")
            {
                _logError = false;
                return;
            }
    
            foreach (var objectReference in DragAndDrop.objectReferences)
            {
                if (objectReference is MonoScript monoScript && monoScript.GetClass() == target.GetType())
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    return;
                }
            }
        }
    
#if ODIN_INSPECTOR_3
        protected override void OnDisable()
#else
        private void OnDisable()
#endif
        {
            EditorApplication.update -= Update;
            DragAndDrop.RemoveDropHandler(InspectorDropHandler);
        }
    }
}
