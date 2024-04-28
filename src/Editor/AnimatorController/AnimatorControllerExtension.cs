using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Graphs;
using UnityEngine;

namespace CGame.Editor
{
    [InitializeOnLoad]
    public static class AnimatorControllerExtension
    {
        private static readonly Type AnimatorControllerToolType = typeof(Node).Assembly.GetType("UnityEditor.Graphs.AnimatorControllerTool");
        private static EditorWindow _currentAnimatorControllerTool;

        public static AnimatorController CurrentAnimatorController { get; private set; }
        public static int SelectedAnimatorLayerIndex { get; private set; }

        public static string[] AllStateName => CurrentAnimatorController == null ? null : CurrentAnimatorController.layers[SelectedAnimatorLayerIndex].stateMachine.states.Select(state => state.state.name).ToArray();
        
        static AnimatorControllerExtension()
        {
            EditorApplication.update += OnUpdate;
            Selection.selectionChanged += OnSelectionChanged;
        }

        private static async void OnUpdate()
        {
            if (Selection.count > 1)
                return;
            
            if (_currentAnimatorControllerTool == null)
            {
                var animatorControllerTools = Resources.FindObjectsOfTypeAll(AnimatorControllerToolType);
                _currentAnimatorControllerTool = animatorControllerTools.Length > 0 ? (EditorWindow)animatorControllerTools[0] : null;
                await Task.Delay(2000);
                Refresh();
            }
        }

        private static void OnSelectionChanged()
        {
            Refresh();
        }

        private static void Refresh()
        {
            if (_currentAnimatorControllerTool == null)
                return;

            CurrentAnimatorController =
                (AnimatorController)AnimatorControllerToolType.GetProperty("animatorController")!.GetValue(
                    _currentAnimatorControllerTool);

            SelectedAnimatorLayerIndex =
                (int)AnimatorControllerToolType.GetProperty("selectedLayerIndex")!.GetValue(
                    _currentAnimatorControllerTool);
        }
    }
}