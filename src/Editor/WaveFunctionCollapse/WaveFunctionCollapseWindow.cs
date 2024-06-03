using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CGame.Editor
{
    [Serializable]
    public class GridItemData
    {
        public string guid;
        public int i;
        public int j;

        public GridItemData()
        {
            guid = "";
            i = j = -1;
        }

        public GridItemData(string guid, int i, int j)
        {
            this.guid = guid;
            this.i = i;
            this.j = j;
        }
    }

    [Serializable]
    public class WaveFunctionCollapseWindowData
    {
        public float width;
        public float height;
        public List<GridItemData> allGridItemData;

        public WaveFunctionCollapseWindowData()
        {
            width = height = 0;
            allGridItemData = new List<GridItemData>();
        }
        
        public WaveFunctionCollapseWindowData(float width, float height)
        {
            this.width = width;
            this.height = height;
            allGridItemData = new List<GridItemData>();
        }
    }
    
    public class WaveFunctionCollapseWindow : EditorWindow
    {
        [MenuItem("WFC/Window")]
        public static void OpenWindow()
        {
            var window = GetWindow<WaveFunctionCollapseWindow>();
            window.titleContent = new GUIContent(nameof(WaveFunctionCollapseWindow));
            window.Show();
        }
        
        private const float GenerateIdButtonWidth = 80f;
        private const float OpenBorderToggleWidth = 100f;
        private const float BorderWidthTextFieldWidth = 120f;
        private const float ImportButtonWidth = 50f;
        private const float ExportButtonWidth = 50f;
        private const float BackgroundColorFieldWidth = 120f;
        
        private const float ButtonHeight = 18f;
        
        private float ItemWidth => 50f * _scale;
        private float ItemHeight => 50f * _scale;

        private Vector2 _offset;
        private float _scale = 1f;
        
        private void OnGUI()
        {
            var rect = new Rect(Vector2.zero, position.size);
            
            var verticalRects = rect.VerticalSplit(ButtonHeight, -1);

            var current = Event.current;
            if (verticalRects[1].Contains(current.mousePosition))
            {
                switch (current.type)
                {
                    case EventType.MouseDrag:
                    {
                        if (current.button == 2)
                        {
                            _offset += current.delta;
                            current.Use();
                        }
                        break;
                    }
                    case EventType.ScrollWheel:
                    {
                        _scale += current.delta.y * -0.01f;
                        current.Use();
                        break;
                    }
                }
            }
            
            verticalRects[1].width += ItemWidth;
            if (_offset.x > 0)
                verticalRects[1].x += _offset.x % ItemWidth - ItemWidth;
            else if (_offset.x < 0)
                verticalRects[1].x += _offset.x % ItemWidth;

            verticalRects[1].height += ItemHeight;
            if (_offset.y > 0)
                verticalRects[1].y += _offset.y % ItemHeight - ItemHeight;
            else if (_offset.y < 0)
                verticalRects[1].y += _offset.y % ItemHeight;
            
            EditorGUIExtension.DrawSolidRect(verticalRects[1], _backgroundColor);
            DrawItem(verticalRects[1]);
            DrawButton(verticalRects[0]);
        }
        
        private readonly Color _buttonsBackgroundColor = new(56 / 255f, 56 / 255f, 56 / 255f);

        private bool _openBorder = true;
        private bool _borderWidthIsDrag;
        
        private float _borderWidth = 1f;
        private float BorderWidth
        {
            get => _borderWidth;
            set => _borderWidth = Mathf.Clamp(value, 1f, 10f);
        }

        private Color _backgroundColor = new(56 / 255f, 56 / 255f, 56 / 255f);
        
        private void DrawButton(Rect rect)
        {
            EditorGUIExtension.DrawSolidRect(rect, _buttonsBackgroundColor);
            var horizontalRects = rect.HorizontalSplit(GenerateIdButtonWidth, 10, OpenBorderToggleWidth, 10,
                BorderWidthTextFieldWidth, 10, ImportButtonWidth, 10, ExportButtonWidth, 10, BackgroundColorFieldWidth -1);
            if (GUI.Button(horizontalRects[0], "GenerateId"))
            {
                var waveBlockHashSet = new HashSet<WaveBlock>();
                foreach (var (_, waveBlock) in _waveBlockDic)
                {
                    if (waveBlock == null)
                        continue;
                    
                    waveBlockHashSet.Add(waveBlock);
                }
                
                foreach (var waveBlock in waveBlockHashSet)
                {
                    foreach (Direction direction in Direction.MainFour)
                    {
                        var index = waveBlock.directionIdInfos.FindIndex(info => info.direction == direction);
                        if (index != -1)
                            waveBlock.directionIdInfos[index] = new DirectionIdInfo(direction, -1);
                        else
                            waveBlock.directionIdInfos.Add(new DirectionIdInfo(direction, -1));
                    }
                }

                var startId = 0;
                foreach (var (gridIndex, waveBlock) in _waveBlockDic)
                {
                    if (waveBlock == null)
                        continue;

                    foreach (Direction direction in Direction.MainFour)
                    {
                        var directionVector = direction.ToVector();
                        var targetGridIndex = new Vector2Int(gridIndex.x - directionVector.y, gridIndex.y + directionVector.x);
                        if (_waveBlockDic.TryGetValue(targetGridIndex, out var targetWaveBlock))
                        {
                            var currentIndex = waveBlock.directionIdInfos.FindIndex(d => d.direction == direction);
                            var targetIndex = targetWaveBlock.directionIdInfos.FindIndex(d => d.direction == direction.Reverse());
                            if (waveBlock.directionIdInfos[currentIndex].id == -1 && targetWaveBlock.directionIdInfos[targetIndex].id == -1)
                            {
                                var id = startId++;
                                waveBlock.directionIdInfos[currentIndex] = new DirectionIdInfo(direction,id);
                                targetWaveBlock.directionIdInfos[targetIndex] = new DirectionIdInfo(direction.Reverse(), id);
                            }
                            else if (waveBlock.directionIdInfos[currentIndex].id == -1)
                            {
                                waveBlock.directionIdInfos[currentIndex] = new DirectionIdInfo(direction,
                                    targetWaveBlock.directionIdInfos[targetIndex].id);
                            }
                            else if (targetWaveBlock.directionIdInfos[targetIndex].id == -1)
                            {
                                targetWaveBlock.directionIdInfos[targetIndex] = new DirectionIdInfo(direction.Reverse(), waveBlock.directionIdInfos[currentIndex].id);
                            }
                        }
                    }
                }
                
                foreach (var waveBlock in waveBlockHashSet)
                {
                    EditorUtility.SetDirty(waveBlock);
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            EditorGUIUtility.labelWidth = 80;
            _openBorder = EditorGUI.Toggle(horizontalRects[2], "OpenBroder", _openBorder);
            
            if (_openBorder)
            {
                var borderWidthRects = horizontalRects[4].HorizontalSplit(80, -1);
                EditorGUIUtility.AddCursorRect(borderWidthRects[0],MouseCursor.SlideArrow);

                var current = Event.current;
                if (borderWidthRects[0].Contains(current.mousePosition) && current.type is EventType.MouseDown)
                {
                    _borderWidthIsDrag = true;
                    current.Use();
                }
                if (current.type is EventType.MouseUp)
                {
                    _borderWidthIsDrag = false;
                }
                
                if (_borderWidthIsDrag && current.type is EventType.MouseDrag)
                {
                    BorderWidth += current.delta.x * 0.1f;
                    current.Use();
                }
                EditorGUI.LabelField(borderWidthRects[0], "BorderWidth");
                BorderWidth = EditorGUI.FloatField(borderWidthRects[1], BorderWidth);
            }

            if (GUI.Button(horizontalRects[6], "Import"))
            {
                var path = EditorUtility.OpenFilePanel("Import Data", Application.dataPath, "json");
                if (!string.IsNullOrWhiteSpace(path))
                {
                    _waveBlockDic.Clear();
                    var data = JsonConvert.DeserializeObject<WaveFunctionCollapseWindowData>(File.ReadAllText(path));
                    position = new Rect(position.position, new Vector2(data.width, data.height));
                    foreach (var gridItemData in data.allGridItemData)
                    {
                        _waveBlockDic[new Vector2Int(gridItemData.i, gridItemData.j)] = AssetDatabase
                            .LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(gridItemData.guid))
                            .GetComponent<WaveBlock>();
                    }
                    foreach (var (gridIndex, waveBlock) in _waveBlockDic)
                    {
                        data.allGridItemData.Add(new GridItemData(
                            AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(waveBlock.gameObject)),
                            gridIndex.x, gridIndex.y));
                    }
                }
            }

            if (GUI.Button(horizontalRects[8], "Export"))
            {
                var path = EditorUtility.SaveFilePanel("Export Data", Application.dataPath, "WFCWindowData", "json");
                if (!string.IsNullOrWhiteSpace(path))
                {
                    var data = new WaveFunctionCollapseWindowData(position.width, position.height);
                    foreach (var (gridIndex, waveBlock) in _waveBlockDic)
                    {
                        data.allGridItemData.Add(new GridItemData(
                            AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(waveBlock.gameObject)),
                            gridIndex.x, gridIndex.y));
                    }
                    File.WriteAllText(path, JsonConvert.SerializeObject(data));
                }
            }

            _backgroundColor = EditorGUI.ColorField(horizontalRects[10], _backgroundColor);
        }

        private Vector2Int _selectGridIndex = new(-1, -1);
        private WaveBlock _dragWaveBlock;
        private WaveBlock _copyWaveBlock;
        private const string CopyCommand = "Copy";
        private const string PasteCommand = "Paste";

        private readonly Dictionary<Vector2Int, WaveBlock> _waveBlockDic = new();
        
        private void DrawItem(Rect rect)
        {
            var gridRects = rect.GridSplitForSize(ItemWidth, ItemHeight, ToIntType.Ceil);

            var current = Event.current;
            switch (current.type)
            {
                case EventType.DragExited:
                {
                    _dragWaveBlock = null;
                    current.Use();
                    break;
                }
                case EventType.KeyDown:
                {
                    if (current.keyCode is KeyCode.Delete && _selectGridIndex is { x: >= 0, y: >= 0 })
                    {
                        _waveBlockDic.Remove(_selectGridIndex);
                        current.Use();
                    }
                    break;
                }
                case EventType.ValidateCommand:
                {
                    if (current.commandName == CopyCommand && _waveBlockDic.ContainsKey(_selectGridIndex))
                    {
                        current.Use();
                    }
                    else if (current.commandName == PasteCommand && _copyWaveBlock != null)
                    {
                        current.Use();
                    }
                    break;
                }
                case EventType.ExecuteCommand:
                {
                    if (current.commandName == CopyCommand)
                    {
                        _copyWaveBlock = _waveBlockDic[_selectGridIndex];
                        current.Use();
                    }
                    else if (current.commandName == PasteCommand)
                    {
                        _waveBlockDic[_selectGridIndex] = _copyWaveBlock;
                        current.Use();
                    }
                    break;
                }
            }

            for (var i = 0; i < gridRects.GetLength(0); i++)
            {
                for (var j = 0; j < gridRects.GetLength(1); j++)
                {
                    var gridIndex = RectIndexToGridIndex(new Vector2Int(i, j));
                    
                    if (_waveBlockDic.TryGetValue(gridIndex, out var waveBlock))
                    {
                        EditorGUI.DrawTextureTransparent(gridRects[i, j], waveBlock.sprite.GetPartTexture(), ScaleMode.ScaleToFit);
                    }

                    if (current.mousePosition.y >= ButtonHeight && gridRects[i, j].Contains(current.mousePosition))
                    {
                        switch (current.type)
                        {
                            case EventType.MouseDown:
                            {
                                if (current.button == 0)
                                {
                                    _selectGridIndex = gridIndex;
                                    Selection.activeGameObject = _waveBlockDic.TryGetValue(_selectGridIndex, out var value) && value != null ? value.gameObject : null;
                                    current.Use();
                                }
                                else if (current.button == 1)
                                {
                                    _selectGridIndex = gridIndex;
                                    if (Selection.activeGameObject != null && Selection.activeGameObject.TryGetComponent<WaveBlock>(out var selectWaveBlock))
                                        _waveBlockDic[gridIndex] = selectWaveBlock;
                                    else
                                        _waveBlockDic[gridIndex] = null;
                                    Selection.activeGameObject = _waveBlockDic.TryGetValue(_selectGridIndex, out var value) && value != null ? value.gameObject : null;
                                    current.Use();
                                }
                                break;
                            }
                            case EventType.MouseDrag:
                            {
                                if (current.button == 0 && waveBlock != null)
                                {
                                    DragAndDrop.objectReferences = new Object[] { waveBlock.gameObject };
                                    DragAndDrop.StartDrag("ItemDrag");
                                    _waveBlockDic.Remove(gridIndex);
                                    current.Use();
                                }
                                break;
                            }
                            case EventType.DragUpdated:
                            {
                                if (DragAndDrop.objectReferences.Length <= 0)
                                    break;

                                var obj = DragAndDrop.objectReferences[0];
                                if (obj is GameObject gameObject && gameObject.TryGetComponent<WaveBlock>(out var dragWaveBlock))
                                {
                                    if (DragAndDrop.visualMode == DragAndDropVisualMode.None)
                                        DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                                    _dragWaveBlock = dragWaveBlock;
                                    current.Use();
                                }
                                break;
                            }
                            case EventType.DragPerform:
                            {
                                var indexI = i;
                                var indexJ = j;
                                var useEvent = false;
                                foreach (var obj in DragAndDrop.objectReferences)
                                {
                                    if (obj is GameObject gameObject && gameObject.TryGetComponent<WaveBlock>(out var dragWaveBlock))
                                    {
                                        var gridItemIndex = RectIndexToGridIndex(new Vector2Int(indexI, indexJ));
                                        _waveBlockDic[gridItemIndex] = dragWaveBlock;
                                        indexJ++;
                                        useEvent = true;
                                    }
                                    if (indexJ >= gridRects.GetLength(1) - 1)
                                    {
                                        indexI++;
                                        indexJ = 0;
                                    }
                                }

                                if (useEvent)
                                {
                                    _dragWaveBlock = null;
                                    current.Use();
                                }
                                break;
                            }
                        }
                    }
                }
            }
            DrawGrid(rect);
            var selectRectIndex = GridIndexToRectIndex(_selectGridIndex);
            if (selectRectIndex is { x: >= 0, y: >= 0 })
                EditorGUIExtension.DrawBorders(gridRects[selectRectIndex.x, selectRectIndex.y], BorderWidth, Color.red);
            if (rect.Contains(current.mousePosition) && _dragWaveBlock != null)
            {
                EditorGUI.DrawTextureTransparent(new Rect(current.mousePosition, new Vector2(ItemWidth * 0.5f, ItemHeight * 0.5f)), _dragWaveBlock.sprite.GetPartTexture(), ScaleMode.ScaleToFit);
            }
        }

        private void DrawGrid(Rect rect)
        {
            if (_openBorder)
            {
                rect.x -= BorderWidth / 2f;
                rect.y -= BorderWidth / 2f;
                EditorGUIExtension.DrawGridsForSize(rect, BorderWidth, Color.black, ItemWidth, ItemHeight, ToIntType.Ceil);
            }
        }

        private Vector2Int RectIndexToGridIndex(Vector2Int rectIndex)
        {
            return new Vector2Int(
                _offset.y switch
                {
                    > 0 => rectIndex.x - 1 - Mathf.FloorToInt(_offset.y / ItemHeight),
                    < 0 => rectIndex.x - Mathf.CeilToInt(_offset.y / ItemHeight),
                    _ => rectIndex.x
                },
                _offset.x switch
                {
                    > 0 => rectIndex.y - 1 - Mathf.FloorToInt(_offset.x / ItemWidth),
                    < 0 => rectIndex.y - Mathf.CeilToInt(_offset.x / ItemWidth),
                    _ => rectIndex.y
                }
            );
        }
        
        private Vector2Int GridIndexToRectIndex(Vector2Int gridIndex)
        {
            return new Vector2Int(
                _offset.y switch
                {
                    > 0 => gridIndex.x + 1 + Mathf.FloorToInt(_offset.y / ItemHeight),
                    < 0 => gridIndex.x + Mathf.CeilToInt(_offset.y / ItemHeight),
                    _ => gridIndex.x
                },
                _offset.x switch
                {
                    > 0 => gridIndex.y + 1 + Mathf.FloorToInt(_offset.x / ItemWidth),
                    < 0 => gridIndex.y + Mathf.CeilToInt(_offset.x / ItemWidth),
                    _ => gridIndex.y
                }
            );
        }
    }
}