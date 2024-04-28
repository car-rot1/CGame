using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace CGame
{
    public struct TaskInfo
    {
        public readonly ICustomAsyncTask AsyncTask;
        public readonly string Name;

        public TaskInfo(ICustomAsyncTask asyncTask, string name)
        {
            AsyncTask = asyncTask;
            Name = name;
        }
    }

    public class SceneSwitch : SingletonClass<SceneSwitch>
    {
        private readonly List<TaskInfo> taskInfoList = new();

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
            _defaultLoadMaskGameObject = Resources.Load<GameObject>("LoadMask");
        }

        public void AddTaskInfo(TaskInfo taskInfo) => taskInfoList.Add(taskInfo);

        public void LoadScene(string sceneName, GameObject loadMask = null, float loadMinTime = 1.5f, float loadSceneMix = 0.3f, float loadAssetMix = 0.3f)
        {
            if (loadMask == null)
                loadMask = _defaultLoadMaskGameObject;

            CoroutineObject.StartCoroutine(LoadSceneCoroutine(sceneName, loadMask, loadMinTime, loadSceneMix, loadAssetMix));
        }

        private IEnumerator LoadSceneCoroutine(string sceneName, GameObject loadMask, float loadMinTime, float loadSceneMix, float loadAssetMix)
        {
            var loadTimeMix = 1 - loadSceneMix - loadAssetMix;

            // GameGlobal.Pause();
            // Time.timeScale = 0;

            Process = 0f;

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

            var currentLoadTime = 0f;

            #region 加载场景：占比：loadSceneMix

            var asyncOperation = SceneManager.LoadSceneAsync(sceneName);
            asyncOperation.allowSceneActivation = false;
            while (asyncOperation.progress < 0.9f)
            {
                currentLoadTime += Time.unscaledDeltaTime;
                Process = asyncOperation.progress / 0.9f * loadSceneMix;
                Debug.Log("进度：" + Process);
                yield return null;
            }
            asyncOperation.allowSceneActivation = true;
            Debug.Log("场景加载完成");

            #endregion

            #region 加载资源：占比：loadAssetMix

            currentLoadTime += Time.unscaledDeltaTime;
            yield return null;
            Debug.Log("加载需要的资源");
            for (var i = 0; i < taskInfoList.Count; i++)
            {
                Debug.Log($"加载{taskInfoList[i].Name}中......");
                Debug.Log($"当前时间：{currentLoadTime}");
                currentLoadTime += Time.unscaledDeltaTime;
                Process = loadSceneMix + (i + 1.0f) / taskInfoList.Count * loadAssetMix;
                Debug.Log("进度：" + Process);
                Debug.Log($"当前时间：{currentLoadTime}");
                yield return taskInfoList[i].AsyncTask.AsIEnumerator();
            }
            taskInfoList.Clear();
            Debug.Log("加载完毕");

            #endregion

            #region 剩余时间：占比：loadTimeMix

            Debug.Log("真实耗时：" + currentLoadTime);

            var remainder = loadMinTime - currentLoadTime;
            if (remainder <= 0)
            {
                Process = 1.0f;
                Debug.Log("进度：" + Process);
            }
            else
            {
                var currentTime = 0f;
                while (currentTime < remainder)
                {
                    currentTime += Time.unscaledDeltaTime;
                    Process = Mathf.Min(1.0f, loadSceneMix + loadAssetMix + currentTime / remainder * loadTimeMix);
                    Debug.Log("进度：" + Process);
                    yield return null;
                }
            }

            #endregion

            Object.Destroy(loadMaskGameObject);
            Debug.Log("销毁加载壁纸");

            Debug.Log("总耗时：" + Mathf.Max(loadMinTime, currentLoadTime));
            Debug.Log(Process);

            // GameGlobal.Play();
            // Time.timeScale = 1;
        }
        
        /* 缺少宏定义判断来判断是否有UniTask包 */
        // private async UniTask LoadSceneAsync(string sceneName, GameObject loadMask, float loadMinTime, float loadSceneMix, float loadAssetMix)
        // {
        //     var loadTimeMix = 1 - loadSceneMix - loadAssetMix;
        //
        //     // GameGlobal.Pause();
        //     // Time.timeScale = 0;
        //
        //     Process = 0f;
        //
        //     GameObject loadMaskGameObject;
        //     if (loadMask == null)
        //     {
        //         loadMaskGameObject = new GameObject();
        //         loadMaskGameObject.AddComponent<RectTransform>();
        //         var canvas = loadMaskGameObject.AddComponent<Canvas>();
        //         canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        //         canvas.sortingOrder = 999;
        //         loadMaskGameObject.AddComponent<CanvasRenderer>();
        //         loadMaskGameObject.AddComponent<Image>().color = new Color32(50, 50, 50, 255);
        //     }
        //     else
        //     {
        //         loadMaskGameObject = Object.Instantiate(loadMask);
        //     }
        //     Object.DontDestroyOnLoad(loadMaskGameObject);
        //     Debug.Log("生成加载壁纸");
        //
        //     var currentLoadTime = 0f;
        //
        //     #region 加载场景：占比：loadSceneMix
        //
        //     var asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        //     asyncOperation.allowSceneActivation = false;
        //     while (asyncOperation.progress < 0.9f)
        //     {
        //         currentLoadTime += Time.unscaledDeltaTime;
        //         Process = asyncOperation.progress / 0.9f * loadSceneMix;
        //         Debug.Log("进度：" + Process);
        //         await UniTask.NextFrame();
        //     }
        //     asyncOperation.allowSceneActivation = true;
        //     Debug.Log("场景加载完成");
        //
        //     #endregion
        //
        //     #region 加载资源：占比：loadAssetMix
        //
        //     currentLoadTime += Time.unscaledDeltaTime;
        //     await UniTask.NextFrame();
        //     Debug.Log("加载需要的资源");
        //     for (var i = 0; i < taskInfoList.Count; i++)
        //     {
        //         Debug.Log($"加载{taskInfoList[i].Name}中......");
        //         Debug.Log($"当前时间：{currentLoadTime}");
        //         currentLoadTime += Time.unscaledDeltaTime;
        //         Process = loadSceneMix + (i + 1.0f) / taskInfoList.Count * loadAssetMix;
        //         Debug.Log("进度：" + Process);
        //         Debug.Log($"当前时间：{currentLoadTime}");
        //         await taskInfoList[i].AsyncTask.AsIEnumerator();
        //     }
        //     taskInfoList.Clear();
        //     Debug.Log("加载完毕");
        //
        //     #endregion
        //
        //     #region 剩余时间：占比：loadTimeMix
        //
        //     Debug.Log("真实耗时：" + currentLoadTime);
        //
        //     var remainder = loadMinTime - currentLoadTime;
        //     if (remainder <= 0)
        //     {
        //         Process = 1.0f;
        //         Debug.Log("进度：" + Process);
        //     }
        //     else
        //     {
        //         var currentTime = 0f;
        //         while (currentTime < remainder)
        //         {
        //             currentTime += Time.unscaledDeltaTime;
        //             Process = Mathf.Min(1.0f, loadSceneMix + loadAssetMix + currentTime / remainder * loadTimeMix);
        //             Debug.Log("进度：" + Process);
        //             await UniTask.NextFrame();
        //         }
        //     }
        //
        //     #endregion
        //
        //     Object.Destroy(loadMaskGameObject);
        //     Debug.Log("销毁加载壁纸");
        //
        //     Debug.Log("总耗时：" + Mathf.Max(loadMinTime, currentLoadTime));
        //     Debug.Log(Process);
        //
        //     // GameGlobal.Play();
        //     // Time.timeScale = 1;
        // }
    }
}
