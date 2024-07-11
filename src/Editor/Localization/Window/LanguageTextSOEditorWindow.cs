using System.Collections.Generic;
using CGame.Editor;
using UnityEditor;
using UnityEngine;

namespace CGame.Localization.Editor
{
    public class LanguageTextSOEditorWindow : EditorWindow
    {
        private LanguageTextSO _target;
        private LanguageTextSO Target
        {
            get => _target;
            set
            {
                if (_target == value)
                    return;
                _target = value;
                _selects.Clear();
                for (var i = 0; i < _target.languageTextInfos.Count; i++)
                {
                    _selects.Add(false);
                }
            }
        }
        
        [SerializeField] private List<bool> _selects = new();

        private LocalizationConfig _config;

        public static void Open(LanguageTextSO target = null)
        {
            var window = GetWindow<LanguageTextSOEditorWindow>();

            window.Target = target;
            window._config = LocalizationConfig.Instance;
            window.titleContent = new GUIContent(target == null ? "" : target.name);
            window.Show();
        }
        
        private const float TopPadding = 5;
        private const float RightPadding = 3;
        private const float BottomPadding = 20;
        private const float LeftPadding = 3;

        private const float VerticalMargin = 5;

        private const float FileObjectFieldHeight = 18;
        private const float ImportButtonWidth = 50;
        private const float ExportButtonWidth = 50;
        
        private const float AllButtonHeight = 18;
        private const float AllButtonWidth = 50;
        private const float NoneButtonWidth = 50;

        private const float TriggerWidth = 30;

        private const float RemoveButtonHeight = 18;
        private const float RemoveButtonWidth = 60;
        private const float AddButtonWidth = 60;

        private Vector2 _scrollPos;
        private readonly GenericMenu _genericMenu = new();

        private void OnGUI()
        {
            var iconRect = new Rect(Vector2.zero, position.size).HorizontalEquallySplitForNum(5)[4].VerticalEquallySplitForNum(5)[4];
            if (CGameIcon.CGameFileIcon != null)
                GUI.DrawTexture(iconRect, CGameIcon.CGameFileIcon, ScaleMode.ScaleToFit);
            
            var rect = new Rect(new Vector2(RightPadding, TopPadding),
                new Vector2(position.width - RightPadding - LeftPadding, position.height - TopPadding - BottomPadding));

            var verticalRects = rect.VerticalSplit(
                FileObjectFieldHeight,
                VerticalMargin,
                AllButtonHeight,
                VerticalMargin,
                -1,
                VerticalMargin,
                RemoveButtonHeight);

            var rects = verticalRects[0].HorizontalSplit(-1, 10, ImportButtonWidth, 10, ExportButtonWidth);
            var obj = (LanguageTextSO)EditorGUI.ObjectField(rects[0], Target, typeof(LanguageTextSO), true);
            if (obj != null)
                Target = obj;
            if (GUI.Button(rects[2], "Import"))
            {
                _genericMenu.ClearItem();
                _genericMenu.AddItem(new GUIContent("Import CsvFile"), false, ImportCsvFile);
                _genericMenu.AddItem(new GUIContent("Import ExcelFile"), false, ImportExcelFile);
                _genericMenu.AddItem(new GUIContent("Import JsonFile"), false, ImportJsonFile);
                _genericMenu.ShowAsContext();
            }
            if (GUI.Button(rects[4], "Export"))
            {
                _genericMenu.ClearItem();
                _genericMenu.AddItem(new GUIContent("Export CsvFile"), false, ExportCsvFile);
                _genericMenu.AddItem(new GUIContent("Export ExcelFile"), false, ExportExcelFile);
                _genericMenu.AddItem(new GUIContent("Export JsonFile"), false, ExportJsonFile);
                _genericMenu.ShowAsContext();
            }
            
            rects = verticalRects[2].HorizontalSplit(AllButtonWidth, 10, NoneButtonWidth);
            if (GUI.Button(rects[0], "All"))
            {
                for (var i = 0; i < _selects.Count; i++)
                {
                    _selects[i] = true;
                }
            }
            if (GUI.Button(rects[2], "None"))
            {
                for (var i = 0; i < _selects.Count; i++)
                {
                    _selects[i] = false;
                }
            }

            var contentRect = new Rect(verticalRects[4]);
            contentRect.width -= GUI.skin.verticalScrollbar.fixedWidth;

            var idWidth = (contentRect.width - TriggerWidth - 10) / 2f;
            var valueWidth = (contentRect.width - TriggerWidth - 10) / 2f;
            
            var idLabelWidth = GUI.skin.label.CalcSize(new GUIContent("Id :")).x + 2;
            var valueLabelWidth = GUI.skin.label.CalcSize(new GUIContent("Value :")).x + 2;

            var idValueWidth = idWidth - idLabelWidth;
            var valueValueWidth = valueWidth - valueLabelWidth;
            
            contentRect.height = -VerticalMargin;
            foreach (var rowInfo in Target.languageTextInfos)
            {
                var idValueHeight = Mathf.Max(18, 3 + GUI.skin.textArea.CalcHeight(new GUIContent(rowInfo.id), idValueWidth - 2));
                var valueValueHeight = Mathf.Max(18, 3 + GUI.skin.textArea.CalcHeight(new GUIContent(rowInfo.text), valueValueWidth - 2));

                contentRect.height += Mathf.Max(idValueHeight, valueValueHeight) + VerticalMargin;
            }
            
            var itemRect = new Rect(contentRect);
            var isSelect = false;
            _scrollPos = GUI.BeginScrollView(verticalRects[4], _scrollPos, contentRect);
            for (var i = 0; i < Target.languageTextInfos.Count; i++)
            {
                var temp = Target.languageTextInfos[i];
                
                var idValueHeight = Mathf.Max(18, 3 + GUI.skin.textArea.CalcHeight(new GUIContent(temp.id), idValueWidth - 2));
                var valueValueHeight = Mathf.Max(18, 3 + GUI.skin.textArea.CalcHeight(new GUIContent(temp.text), valueValueWidth - 2));
                
                itemRect.height = Mathf.Max(idValueHeight, valueValueHeight);
                rects = itemRect.HorizontalSplit(TriggerWidth, idWidth, 10, valueWidth);
                itemRect.y += itemRect.height + VerticalMargin;
                
                EditorGUI.BeginChangeCheck();

                rects[0].height = 18;
                _selects[i] = EditorGUI.ToggleLeft(rects[0], "", _selects[i]);

                rects[1].height = idValueHeight;
                // EditorGUIUtility.labelWidth = idLabelWidth;
                // var idValue = EditorGUI.TextField(rects[1], "Id :", rowInfo.id, new GUIStyle(GUI.skin.textField) { wordWrap = true });
                var idRect = rects[1].HorizontalSplit(idLabelWidth, idValueWidth);
                EditorGUI.LabelField(idRect[0], "Id :", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
                var idValue = EditorGUI.TextArea(idRect[1], temp.id, new GUIStyle(GUI.skin.textField) { wordWrap = true });

                rects[3].height = valueValueHeight;
                // EditorGUIUtility.labelWidth = valueLabelWidth;
                // var valueValue = EditorGUI.TextField(rects[3], "Value :", rowInfo.value, new GUIStyle(GUI.skin.textField) { wordWrap = true });
                var valueRect = rects[3].HorizontalSplit(valueLabelWidth, valueValueWidth);
                EditorGUI.LabelField(valueRect[0], "Value :", new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperCenter });
                var valueValue = EditorGUI.TextArea(valueRect[1], temp.text, new GUIStyle(GUI.skin.textField) { wordWrap = true });
                
                if (EditorGUI.EndChangeCheck())
                {
                    if (!string.IsNullOrWhiteSpace(idValue) && Target.languageTextInfos.FindIndex(r => r.id == idValue) == -1)
                        temp.id = idValue;
                    if (!string.IsNullOrWhiteSpace(valueValue))
                        temp.text = valueValue;
                }

                if (_selects[i])
                    isSelect = true;

                Target.languageTextInfos[i] = temp;
            }
            GUI.EndScrollView();
            
            rects = verticalRects[6].HorizontalSplit(-1, RemoveButtonWidth, 10, AddButtonWidth);
            if (GUI.Button(rects[1], "Remove"))
            {
                if (!isSelect && Target.languageTextInfos.Count > 0)
                {
                    var index = Target.languageTextInfos.Count - 1;
                    Target.languageTextInfos.RemoveAt(index);
                    _selects.RemoveAt(index);
                }
                else
                {
                    for (var i = Target.languageTextInfos.Count - 1; i >= 0; i--)
                    {
                        if (_selects[i])
                        {
                            Target.languageTextInfos.RemoveAt(i);
                            _selects.RemoveAt(i);
                        }
                    }
                }
            }
            if (GUI.Button(rects[3], "Add"))
            {
                Target.languageTextInfos.Add(new LanguageTextInfo());
                _selects.Add(false);
            }
        }

        private readonly List<List<string>> _csvFileData = new();

        private void ImportCsvFile()
        {
            var path = EditorUtility.OpenFilePanel(nameof(ImportCsvFile), Application.dataPath, _config.localizationStringLoader.CsvFileInfo.fileExtension[1..]);
            if (string.IsNullOrWhiteSpace(path))
                return;
            
            foreach (var languageTextInfo in CsvFileController.GetValue<LanguageTextInfo>(path, _config.localizationStringLoader.CsvFileInfo.ignoreHead, _config.localizationStringLoader.CsvFileInfo.separator, _config.localizationStringLoader.CsvFileInfo.linefeed))
            {
                Target.languageTextInfos.Add(languageTextInfo);
                _selects.Add(false);
            }
        }
        
        private void ImportExcelFile()
        {
            var path = EditorUtility.OpenFilePanel(nameof(ImportCsvFile), Application.dataPath, _config.localizationStringLoader.ExcelFileInfo.fileExtension[1..]);
            if (string.IsNullOrWhiteSpace(path))
                return;

            var value = ExcelUtility.ReadExcel(path);
            for (var i = 0; i < value.GetLength(0); i++)
            {
                Target.languageTextInfos.Add(new LanguageTextInfo { id = value[i, 0].ToString(), text = value[i, 1].ToString() });
                _selects.Add(false);
            }
        }
        
        private void ImportJsonFile()
        {
            var path = EditorUtility.OpenFilePanel(nameof(ImportCsvFile), Application.dataPath, _config.localizationStringLoader.JsonFileInfo.fileExtension[1..]);
            if (string.IsNullOrWhiteSpace(path))
                return;

            foreach (var languageTextInfo in NewJsonFileController.GetValue<LanguageTextInfo>(path))
            {
                Target.languageTextInfos.Add(languageTextInfo);
                _selects.Add(false);
            }
        }
        
        private void ExportCsvFile()
        {
            var path = EditorUtility.SaveFilePanel(nameof(ExportCsvFile), Application.dataPath, _target.name, _config.localizationStringLoader.CsvFileInfo.fileExtension[1..]);
            if (string.IsNullOrWhiteSpace(path))
                return;

            CsvFileController.SetValue(path, _target.languageTextInfos, _config.localizationStringLoader.CsvFileInfo.ignoreHead, _config.localizationStringLoader.CsvFileInfo.separator, _config.localizationStringLoader.CsvFileInfo.linefeed);
        }
        
        private void ExportExcelFile()
        {
            var path = EditorUtility.SaveFilePanel(nameof(ExportCsvFile), Application.dataPath, _target.name, _config.localizationStringLoader.ExcelFileInfo.fileExtension[1..]);
            if (string.IsNullOrWhiteSpace(path))
                return;

            var value = new string[_target.languageTextInfos.Count, 2];
            for (var i = 0; i < _target.languageTextInfos.Count; i++)
            {
                var languageTextInfo = _target.languageTextInfos[i];
                value[i, 0] = languageTextInfo.id;
                value[i, 1] = languageTextInfo.text;
            }
            ExcelUtility.WriteExcel(path, value);
        }
        
        private void ExportJsonFile()
        {
            var path = EditorUtility.SaveFilePanel(nameof(ExportCsvFile), Application.dataPath, _target.name, _config.localizationStringLoader.JsonFileInfo.fileExtension[1..]);
            if (string.IsNullOrWhiteSpace(path))
                return;
            
            NewJsonFileController.SetValue(path, _target.languageTextInfos);
        }
    }
}