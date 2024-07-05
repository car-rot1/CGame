using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CGame.Editor
{
    [InitializeOnLoad]
    public static class ComponentScriptDragAndDropExtension
    {
        static ComponentScriptDragAndDropExtension()
        {
            DragAndDrop.AddDropHandler(HierarchyDropHandler);
        }

        private static DragAndDropVisualMode HierarchyDropHandler(int dropTargetInstanceID, HierarchyDropFlags dropMode, Transform parentForDraggedObjects, bool perform)
        {
            var obj = EditorUtility.InstanceIDToObject(dropTargetInstanceID);
            if ((dropMode & HierarchyDropFlags.DropBetween) == 0 && obj != null && obj is GameObject)
                return DragAndDropVisualMode.None;

            var componentScripts = DragAndDrop.objectReferences
                .OfType<MonoScript>()
                .Where(monoScript => monoScript.GetClass().IsSubclassOf(typeof(Component)))
                .ToArray();
            if (perform)
            {
                foreach (var componentScript in componentScripts)
                {
                    var go = new GameObject(componentScript.name);

                    if ((dropMode & HierarchyDropFlags.DropBetween) == HierarchyDropFlags.DropBetween)
                    {
                        if (obj != null && obj is GameObject gameObject)
                        {
                            go.transform.SetParent(gameObject.transform.parent);
                            if ((dropMode & HierarchyDropFlags.DropAfterParent) == HierarchyDropFlags.DropAfterParent)
                                go.transform.SetSiblingIndex(0);
                            else
                                go.transform.SetSiblingIndex(gameObject.transform.GetSiblingIndex() + 1);
                        }
                        else
                            go.transform.SetParent(null);
                    }
                    else if (parentForDraggedObjects != null)
                        go.transform.SetParent(parentForDraggedObjects);
                    go.transform.localPosition = Vector3.zero;
                    
                    Undo.RegisterCreatedObjectUndo(go, go.name);
                    Undo.AddComponent(go, componentScript.GetClass());
                    Selection.activeGameObject = go;

                    EditorApplication.delayCall += () =>
                    {
                        var sceneHierarchyWindowType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
                        EditorWindow.GetWindow(sceneHierarchyWindowType).SendEvent(EditorGUIUtility.CommandEvent("Rename"));
                    };
                }
            }

            return componentScripts.Length > 0 ? DragAndDropVisualMode.Generic : DragAndDropVisualMode.None;
        }
    }
}
