using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CGame.Editor
{
    [InitializeOnLoad]
    public static class SingletonDragAndDropExtension
    {
        static SingletonDragAndDropExtension()
        {
            DragAndDrop.AddDropHandler(HierarchyDropHandler);
        }

        private static DragAndDropVisualMode HierarchyDropHandler(int dropTargetInstanceID, HierarchyDropFlags dropMode, Transform parentForDraggedObjects, bool perform)
        {
            var singletonMonoBehaviours = StageUtility.GetCurrentStageHandle().FindComponentsOfType<SingletonMonoBehaviour>();
            foreach (var objectReference in DragAndDrop.objectReferences)
            {
                switch (objectReference)
                {
                    case MonoScript monoScript:
                    {
                        if (monoScript.GetClass().IsSubclassOf(typeof(SingletonMonoBehaviour)))
                        {
                            if (singletonMonoBehaviours.Any(singletonMonoBehaviour => monoScript.GetClass() == singletonMonoBehaviour.GetType()))
                            {
                                return DragAndDropVisualMode.Rejected;
                            }
                        }
                        break;
                    }
                    case GameObject gameObject:
                    {
                        if (gameObject.TryGetComponent<SingletonMonoBehaviour>(out var targetSingletonMonoBehaviour))
                        {
                            if (singletonMonoBehaviours.Any(singletonMonoBehaviour => targetSingletonMonoBehaviour.GetType() == singletonMonoBehaviour.GetType()))
                            {
                                return DragAndDropVisualMode.Rejected;
                            }
                        }
                        break;
                    }
                }
            }
            return DragAndDropVisualMode.None;
        }
    }
}