using UnityEditor;
using UnityEngine;

namespace CGame.Editor
{
    [CustomEditor(typeof(SingletonMonoBehaviour), true)]
    public class SingletonMonoBehaviourEditor : UnityEditor.Editor
    {
        private bool _logError;
    
        private void OnEnable()
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
    
        private void OnDisable()
        {
            EditorApplication.update -= Update;
            DragAndDrop.RemoveDropHandler(InspectorDropHandler);
        }
    }
}
