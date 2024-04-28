using System;
using System.Collections.Generic;
using UnityEngine;

namespace CGame
{
    public abstract class UIManagerBase<TSelf, TUI> : SingletonMonoBehaviour<TSelf> where TSelf : UIManagerBase<TSelf, TUI> where TUI : Component, IDynamicUI
    {
        private readonly Dictionary<string, List<TUI>> _openUIDic = new();

        public event Action<TUI> OnOpenUI;
        public event Action<TUI> OnCloseUI;
        public event Action<TUI> OnShowUI; 
        public event Action<TUI> OnHideUI; 

        public RectTransform RectTransform { get; private set; }

        protected virtual void Awake()
        {
            RectTransform = (RectTransform)transform;
            // if (PreloadAssetPaths == null)
            //     return;
            //
            // Debug.Log("预加载资源");
            // foreach (var preloadAssetPath in PreloadAssetPaths)
            // {
            //     if (_loader.TryLoadAsset(preloadAssetPath, out var asyncTask))
            //     {
            //         SceneSwitch.Instance.AddTaskInfo(new TaskInfo(asyncTask, preloadAssetPath));
            //     }
            // }
        }

        public TUI Open(TUI ui, RectTransform parent = null, object param = default)
        {
            if (parent == null)
                parent = RectTransform;
            
            var dynamicUI = Instantiate(ui, parent);
            _openUIDic.TryAdd(ui.Key, new List<TUI>());
            _openUIDic[ui.Key].Add(dynamicUI);

            dynamicUI.Open(param);
            OnOpenUI?.Invoke(dynamicUI);

            return dynamicUI;
        }

        public bool Close(TUI ui, object param = default)
        {
            if (!_openUIDic.TryGetValue(ui.Key, out var list) || !list.Remove(ui))
                return false;

            CloseUI(ui, param);
            return true;
        }

        public bool Close(string key, int index, object param = default)
        {
            if (!_openUIDic.TryGetValue(key, out var list) || index < 0 || index >= list.Count)
                return false;

            var ui = list[index];
            list.RemoveAt(index);
            
            CloseUI(ui, param);
            return true;
        }

        protected void CloseUI(TUI ui, object param = default)
        {
            ui.Close(param);
            OnCloseUI?.Invoke(ui);
            Destroy(ui.gameObject);
        }

        public bool Show(TUI ui, object param = default)
        {
            if (!_openUIDic.TryGetValue(ui.Key, out var list) || !list.Contains(ui))
                return false;

            ShowUI(ui, param);
            return true;
        }
        
        public bool Show(string key, int index, object param = default)
        {
            if (!_openUIDic.TryGetValue(key, out var list) || index < 0 || index >= list.Count)
                return false;

            ShowUI(list[index], param);
            return true;
        }

        protected void ShowUI(TUI ui, object param)
        {
            ui.gameObject.SetActive(true);
            ui.Show(param);
            OnShowUI?.Invoke(ui);
        }

        public bool Hide(TUI ui, object param = default)
        {
            if (!_openUIDic.TryGetValue(ui.Key, out var list) || !list.Contains(ui))
                return false;
            
            HideUI(ui, param);
            return true;
        }

        public bool Hide(string key, int index, object param = default)
        {
            if (!_openUIDic.TryGetValue(key, out var list) || index < 0 || index >= list.Count)
                return false;

            HideUI(list[index], param);
            return true;
        }

        protected void HideUI(TUI ui, object param)
        {
            ui.gameObject.SetActive(false);
            ui.Hide(param);
            OnHideUI?.Invoke(ui);
        }

        public TUI GetOpenUI(string key, int index)
        {
            if (!_openUIDic.TryGetValue(key, out var list) || index < 0 || index >= list.Count)
                return null;
            return list[index];
        }

        public bool TryGetOpenUI(string key, int index, out TUI ui)
        {
            if (!_openUIDic.TryGetValue(key, out var list) || index < 0 || index >= list.Count)
            {
                ui = null;
                return false;
            }

            ui = list[index];
            return true;
        }
    }
}