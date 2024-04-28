using UnityEngine;
using CGame;

public class QuadTreeTest : MonoBehaviour
{
    private QuadTreeNodeTransform _root;

    public QuadTreeNodeTransform Root => _root ??= new QuadTreeNodeTransform(new Rect(-8, -5, 16, 10), 1, 5);

    private void Awake()
    {
        foreach (Transform child in transform)
        {
            Root.AddPoint(child);
        }
    }

    public void UpdatePoint()
    {
        // Print(Root);
    }

    private void Print(QuadTreeNodeTransform node)
    {
        Debug.Log(node);
        if (node.ChildrenNode == null)
            return;
        foreach (var childNode in node.ChildrenNode)
        {
            Print(childNode);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        DrawNodeRect(Root);
    }

    private void DrawNodeRect(QuadTreeNodeTransform node)
    {
        GizmosExtension.DrawRect(node.Rect);
        if (node.ChildrenNode == null)
            return;
        foreach (var childNode in node.ChildrenNode)
        {
            DrawNodeRect(childNode);
        }
    }
}
