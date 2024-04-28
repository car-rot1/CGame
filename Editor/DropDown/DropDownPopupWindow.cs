using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CGame.Editor
{
    public class DropDownPopupWindow : PopupWindowContent
    {
        private string _inputSearchText;
        private readonly List<string> _allContent;
        private readonly List<string> _targetContent;
        private readonly Action<string> _callback;

        private readonly float _itemHeight;
        private readonly float _itemSpace;

        public static void Show(Rect activatorRect, IReadOnlyCollection<string> content, Action<string> callback, float itemHeight = 18f, float itemSpace = 2f)
        {
            PopupWindow.Show(activatorRect, new DropDownPopupWindow(content, callback, itemHeight, itemSpace));
        }

        private DropDownPopupWindow(IReadOnlyCollection<string> content, Action<string> callback, float itemHeight = 18f, float itemSpace = 2f)
        {
            _allContent = new List<string>(content);
            _targetContent = new List<string>(content);
            _callback = callback;

            _itemHeight = itemHeight;
            _itemSpace = itemSpace;
        }

        public override void OnGUI(Rect rect)
        {
            var verticalRects = rect.VerticalSplit(_itemHeight, _itemSpace, -1);

            EditorGUI.BeginChangeCheck();
            _inputSearchText = EditorGUI.TextField(verticalRects[0], _inputSearchText);
            if (EditorGUI.EndChangeCheck())
            {
                _targetContent.Clear();
                foreach (var content in _allContent.Where(content => content.Contains(_inputSearchText)))
                {
                    _targetContent.Add(content);
                }
            }

            if (!string.IsNullOrEmpty(_inputSearchText))
            {
                foreach (var content in _targetContent)
                {
                    verticalRects = verticalRects[2].VerticalSplit(_itemHeight, _itemSpace, -1);
                    if (GUI.Button(verticalRects[0], content))
                    {
                        _callback?.Invoke(content);
                    }
                }
            }
            else
            {
                foreach (var content in _allContent)
                {
                    verticalRects = verticalRects[2].VerticalSplit(_itemHeight, _itemSpace, -1);
                    if (GUI.Button(verticalRects[0], content))
                    {
                        _callback?.Invoke(content);
                    }
                }
            }
        }
    }
}