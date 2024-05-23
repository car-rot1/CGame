using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_UNITASK
using Cysharp.Threading.Tasks;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CGame
{
    public class SceneSwitch : SingletonClass<SceneSwitch>
    {
        private string[] sceneNames;
        private readonly List<(Func<IEnumerator> task, string name)> taskList = new();
        public Action<string> OnSwitchSceneBegin;
        public Action<string> OnSwitchSceneEnd;

        public Action<float, float> OnProcessChange;
        private float _process;
        private float Process
        {
            get => _process;
            set
            {
                if (Mathf.Approximately(_process, value))
                    return;
                OnProcessChange?.Invoke(_process, value);
                _process = value;
            }
        }

        private GameObject _defaultLoadMaskGameObject;

        protected override void Init()
        {
            var count = SceneManager.sceneCountInBuildSettings;
            sceneNames = new string[count];
            for (var i = 0; i < count; i++)
                sceneNames[i] = SceneUtility.GetScenePathByBuildIndex(i).Split('/')[^1].Split('.')[0];
            
            _defaultLoadMaskGameObject = Resources.Load<GameObject>("LoadMask");
        }

        public void AddTask(Func<IEnumerator> task, string name) => taskList.Add((task, name));
        public void RemoveTask(Func<IEnumerator> task, string name) => taskList.Remove((task, name));
        
        public void LoadScene(string sceneName, GameObject loadMask = null, float loadMinTime = 1.5f, float loadSceneMix = 0.3f, float loadAssetMix = 0.3f)
        {
            if (loadMask == null)
                loadMask = _defaultLoadMaskGameObject;

            CoroutineObject.StartCoroutine(LoadSceneCoroutine(sceneName, loadMask, loadMinTime, loadSceneMix, loadAssetMix));
        }
        
        public void LoadScene(int sceneIndex, GameObject loadMask = null, float loadMinTime = 1.5f, float loadSceneMix = 0.3f, float loadAssetMix = 0.3f)
        {
            if (loadMask == null)
                loadMask = _defaultLoadMaskGameObject;

            CoroutineObject.StartCoroutine(LoadSceneCoroutine(sceneNames[sceneIndex], loadMask, loadMinTime, loadSceneMix, loadAssetMix));
        }

        private IEnumerator LoadSceneCoroutine(string sceneName, GameObject loadMask, float loadMinTime, float loadSceneMix, float loadAssetMix)
        {
            OnSwitchSceneBegin?.Invoke(sceneName);
            
            var loadTimeMix = 1 - loadSceneMix - loadAssetMix;

            // GameGlobal.Pause();
            // Time.timeScale = 0;
            GameObject loadMaskGameObject;
            if (loadMask == null)
            {
                loadMaskGameObject = new GameObject();
                loadMaskGameObject.AddComponent<RectTransform>();
                var canvas = loadMaskGameObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 999;
                loadMaskGameObject.AddComponent<CanvasRenderer>();
                loadMaskGameObject.AddComponent<Image>().color = new Color32(50, 50, 50, 255);
            }
            else
            {
                loadMaskGameObject = Object.Instantiate(loadMask);
            }
            Object.DontDestroyOnLoad(loadMaskGameObject);
            Debug.Log("生成加载壁纸");

            yield return new WaitForSecondsRealtime(0.2f);
            
            var startTime = Time.unscaledTime;
            var endTime = startTime + loadMinTime - 0.2f;
            Process = 0f;
            
            #region 加载场景：占比：loadSceneMix

            var asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            asyncOperation.allowSceneActivation = false;
            while (asyncOperation.progress < 0.9f)
            {
                Process = asyncOperation.progress / 0.9f * loadSceneMix;
                Debug.Log("进度：" + Process);
                yield return null;
            }
            asyncOperation.allowSceneActivation = true;
            Debug.Log("场景加载完成");

            #endregion

            #region 加载资源：占比：loadAssetMix

            yield return null;
            Debug.Log("加载需要的资源");
            for (var i = 0; i < taskList.Count; i++)
            {
                Debug.Log($"加载{taskList[i].name}中......");
                Debug.Log($"当前时间：{Time.unscaledTime}");
                Process = loadSceneMix + (i + 1.0f) / taskList.Count * loadAssetMix;
                Debug.Log("进度：" + Process);
                Debug.Log($"当前时间：{Time.unscaledTime}");
                yield return taskList[i].task();
            }
            taskList.Clear();
            Debug.Log("加载完毕");

            #endregion

            #region 剩余时间：占比：loadTimeMix

            Debug.Log("真实耗时：" + (Time.unscaledTime - startTime));

            while (Time.unscaledTime < endTime)
            {
                Process = Mathf.Min(1.0f, loadSceneMix + loadAssetMix + Mathf.InverseLerp(startTime, endTime, Time.unscaledTime) * loadTimeMix);
                Debug.Log("进度：" + Process);
                yield return null;
            }
            Process = 1.0f;
            Debug.Log("进度：" + Process);

            #endregion

            Object.Destroy(loadMaskGameObject);
            Debug.Log("销毁加载壁纸");

            Debug.Log("总耗时：" + Mathf.Max(loadMinTime, Time.unscaledTime - startTime));
            Debug.Log(Process);

            // GameGlobal.Play();
            // Time.timeScale = 1;
            
            OnSwitchSceneEnd?.Invoke(sceneName);
        }
        
        /* 缺少宏定义判断来判断是否有UniTask包 */
#if UNITY_UNITASK
        private async UniTask LoadSceneAsync(string sceneName, GameObject loadMask, float loadMinTime, float loadSceneMix, float loadAssetMix)
        {
            var loadTimeMix = 1 - loadSceneMix - loadAssetMix;
        
            // GameGlobal.Pause();
            // Time.timeScale = 0;
        
            GameObject loadMaskGameObject;
            if (loadMask == null)
            {
                loadMaskGameObject = new GameObject();
                loadMaskGameObject.AddComponent<RectTransform>();
                var canvas = loadMaskGameObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 999;
                loadMaskGameObject.AddComponent<CanvasRenderer>();
                loadMaskGameObject.AddComponent<Image>().color = new Color32(50, 50, 50, 255);
            }
            else
            {
                loadMaskGameObject = Object.Instantiate(loadMask);
            }
            Object.DontDestroyOnLoad(loadMaskGameObject);
            Debug.Log("生成加载壁纸");
            
            await new WaitForSecondsRealtime(0.2f);
        
            var startTime = Time.unscaledTime;
            var endTime = startTime + loadMinTime - 0.2f;
            Process = 0f;
        
            #region 加载场景：占比：loadSceneMix
        
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            asyncOperation.allowSceneActivation = false;
            while (asyncOperation.progress < 0.9f)
            {
                Process = asyncOperation.progress / 0.9f * loadSceneMix;
                Debug.Log("进度：" + Process);
                await UniTask.NextFrame();
            }
            asyncOperation.allowSceneActivation = true;
            Debug.Log("场景加载完成");
        
            #endregion
        
            #region 加载资源：占比：loadAssetMix
        
            await UniTask.NextFrame();
            Debug.Log("加载需要的资源");
            for (var i = 0; i < taskInfoList.Count; i++)
            {
                Debug.Log($"加载{taskInfoList[i].Name}中......");
                Debug.Log($"当前时间：{Time.unscaledTime}");
                Process = loadSceneMix + (i + 1.0f) / taskInfoList.Count * loadAssetMix;
                Debug.Log("进度：" + Process);
                Debug.Log($"当前时间：{Time.unscaledTime}");
                await taskInfoList[i].AsyncTask.AsIEnumerator();
            }
            taskInfoList.Clear();
            Debug.Log("加载完毕");
        
            #endregion
        
            #region 剩余时间：占比：loadTimeMix
        
            Debug.Log("真实耗时：" + (Time.unscaledTime - startTime));
        
            while (Time.unscaledTime < endTime)
            {
                Process = Mathf.Min(1.0f, loadSceneMix + loadAssetMix + Mathf.InverseLerp(startTime, endTime, Time.unscaledTime) * loadTimeMix);
                Debug.Log("进度：" + Process);
                await UniTask.NextFrame();
            }
            Process = 1.0f;
            Debug.Log("进度：" + Process);
        
            #endregion
        
            Object.Destroy(loadMaskGameObject);
            Debug.Log("销毁加载壁纸");
        
            Debug.Log("总耗时：" + Mathf.Max(loadMinTime, Time.unscaledTime - startTime));
            Debug.Log(Process);
        
            // GameGlobal.Play();
            // Time.timeScale = 1;
        }
#endif
    }
}
