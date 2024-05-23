using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CGame.Editor
{
    public class DropDownPopupWindow : PopupWindowContent
    {
        private class DropDownTreeNode
        {
            public readonly string path;
            public readonly string text;
            public bool hasValue;
            public readonly List<DropDownTreeNode> children = new();

            private DropDownTreeNode(string path, string text)
            {
                this.path = path;
                this.text = text;
                hasValue = false;
            }

            private static readonly StringBuilder _stringBuilder = new();
        
            public static DropDownTreeNode ToDropDownTreeNode(IEnumerable<string> contents, char splitChar = '/')
            {
                var root = new DropDownTreeNode(null, null);
            
                var allContent = new List<string>();
                foreach (var content in contents)
                {
                    if (!allContent.Contains(content))
                        allContent.Add(content);
                }
            
                foreach (var content in allContent)
                {
                    var node = root;
                    _stringBuilder.Clear();
                    var contentParts = content.Split(splitChar);
                    foreach (var contentPart in contentParts)
                    {
                        _stringBuilder.Append(contentPart);
                        var tempNode = node.children.Find(n => n.text.Equals(contentPart));
                        if (tempNode == null)
                        {
                            tempNode = new DropDownTreeNode(_stringBuilder.ToString(), contentPart);
                            node.children.Add(tempNode);
                        }
                        node = tempNode;
                        _stringBuilder.Append(splitChar);
                    }
                    node.hasValue = true;
                }

                return root;
            }
        }

        private readonly List<string> _allContent;
        private readonly Action<string> _callback;
        private readonly char _separator;
        private readonly bool _hasSearch;
        private readonly bool _autoClose;
        
        private string _inputSearchText;
        private DropDownTreeNode _root;

        private static float _width;
        public override Vector2 GetWindowSize()
        {
            var size = base.GetWindowSize();
            size.x = _width;
            return size;
        }

        private const float SearchTextHeight = 20f;
        private const float ItemHeight = 18f;

        private readonly Color HoverColor = new(55 / 255f, 55 / 255f, 55 / 255f);
        private readonly Color DefaultColor = new(49 / 255f, 49 / 255f, 49 / 255f);
        
        private readonly Dictionary<string, bool> _foldoutDic = new();
        private Vector2 _scrollPosition;
        
        public static void Show(Rect activatorRect, IReadOnlyCollection<string> content, Action<string> callback, char splitChar = '/', bool hasSearch = true, bool autoClose = true)
        {
            _width = activatorRect.width;
            PopupWindow.Show(activatorRect, new DropDownPopupWindow(content, callback, splitChar, hasSearch, autoClose));
        }
        
        private DropDownPopupWindow(IReadOnlyCollection<string> contents, Action<string> callback, char separator = '/', bool hasSearch = true, bool autoClose = true)
        {
            _allContent = new List<string>(contents);
            _root = DropDownTreeNode.ToDropDownTreeNode(contents, separator);
            
            _callback = callback;
            _separator = separator;
            _hasSearch = hasSearch;
            _autoClose = autoClose;
        }

        private float? _lastHeight;

        public override void OnGUI(Rect rect)
        {
            EditorGUIExtension.DrawSolidRect(rect, DefaultColor);
            
            if (_hasSearch)
            {
                rect.yMin += EditorGUIExtension.ControlVerticalSpacing;
                var searchRect = rect;
                searchRect.height = SearchTextHeight;
                
                EditorGUI.BeginChangeCheck();
                _inputSearchText = EditorGUI.TextField(searchRect, _inputSearchText, EditorStyles.toolbarSearchField);
                if (EditorGUI.EndChangeCheck())
                {
                    IEnumerable<string> contents = _allContent;
                    if (!string.IsNullOrEmpty(_inputSearchText))
                    {
                        contents = _allContent.Where(content => content.Contains(_inputSearchText, StringComparison.OrdinalIgnoreCase));
                        foreach (var s in contents)
                        {
                            foreach (var s1 in s.Split(_separator))
                            {
                                _foldoutDic[s1] = true;
                            }
                        }
                    }
                    _root = DropDownTreeNode.ToDropDownTreeNode(contents, _separator);
                }

                rect.yMin += SearchTextHeight;
            }
            rect.yMin += EditorGUIExtension.ControlVerticalSpacing;

            if (_lastHeight != null)
            {
                var scrollContentRect = new Rect(rect) { height = _lastHeight.Value };
                _scrollPosition = GUI.BeginScrollView(rect, _scrollPosition, scrollContentRect, GUIStyle.none, GUI.skin.verticalScrollbar);
            }
            
            var contentRect = new Rect(rect) { height = ItemHeight };
            var contentHeight = DrawNode(_root, contentRect, _foldoutDic);

            _lastHeight = contentHeight;

            if (_lastHeight != null)
                GUI.EndScrollView();
        }

        private float DrawNode(DropDownTreeNode node, Rect rect, Dictionary<string, bool> foldoutDic)
        {
            var result = 0f;
            
            if (node.path == null)
            {
                foreach (var child in node.children)
                {
                    var r = DrawNode(child, rect, foldoutDic) + EditorGUIExtension.ControlVerticalSpacing;
                    result += r;
                    rect.y += r;
                }

                return result;
            }
            
            var currentEvent = Event.current;
            if (node.children.Count <= 0)
            {
                if (rect.Contains(currentEvent.mousePosition))
                {
                    EditorGUIExtension.DrawSolidRect(new Rect(rect) { xMin = 0 }, HoverColor);
                    if (currentEvent.type is EventType.MouseMove)
                        currentEvent.Use();
                }
                else
                {
                    EditorGUIExtension.DrawSolidRect(new Rect(rect) { xMin = 0 }, DefaultColor);
                }

                if (GUI.Button(rect, node.text, EditorStyles.label))
                {
                    _callback?.Invoke(node.path);
                    if (_autoClose)
                        editorWindow.Close();
                }

                return rect.height;
            }
            
            if (rect.Contains(currentEvent.mousePosition))
            {
                EditorGUIExtension.DrawSolidRect(new Rect(rect) { xMin = 0 }, HoverColor);
                if (currentEvent.type is EventType.MouseMove)
                    currentEvent.Use();
            }
            else
            {
                EditorGUIExtension.DrawSolidRect(new Rect(rect) { xMin = 0 }, DefaultColor);
            }

            foldoutDic.TryAdd(node.path, false);
            if (node.hasValue)
            {
                var r = new Rect(rect);
                r.xMin += EditorStyles.foldout.padding.left;
                if (currentEvent.type is EventType.MouseDown && r.Contains(currentEvent.mousePosition))
                {
                    _callback?.Invoke(node.path);
                    if (_autoClose)
                        editorWindow.Close();
                }
                foldoutDic[node.path] = EditorGUI.Foldout(rect, foldoutDic[node.path], node.text);
            }
            else
                foldoutDic[node.path] = EditorGUI.Foldout(rect, foldoutDic[node.path], node.text, true);
            
            result += rect.height;
            if (foldoutDic[node.path])
            {
                rect.xMin += EditorGUIExtension.IndentPerLevel;
                rect.y += rect.height + EditorGUIExtension.ControlVerticalSpacing;
                foreach (var child in node.children)
                {
                    var r = DrawNode(child, rect, foldoutDic) + EditorGUIExtension.ControlVerticalSpacing;
                    result += r;
                    rect.y += r;
                }
            }

            return result;
        }
    }
}