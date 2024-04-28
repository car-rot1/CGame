using UnityEditor;

namespace CGame.Editor
{
    public static class NodeGraphViewMenu
    {
        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var nodeGraphViewData = EditorUtility.InstanceIDToObject(instanceID) as GraphViewData;
            if (nodeGraphViewData == null)
                return false;
            
            NodeGraphEditorWindow.Open(nodeGraphViewData);
            return true;
        }

        [MenuItem("NodeGraphView/Open NodeGraphViewWindow")]
        public static void OpenNodeGraphViewWindow() => NodeGraphEditorWindow.Open(null);
    }
}