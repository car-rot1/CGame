using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace CGame.Editor
{
    [InitializeOnLoad]
    public static class CGameIcon
    {
        private const float LargeIconSize = 128f;
        
        private static Texture2D _cGameFileIcon;
        private static bool download;
        public static Texture2D CGameFileIcon
        {
            get
            {
                if (!download)
                {
                    download = true;
                    DownLoadImage("https://s11.ax1x.com/2024/01/12/pFCfn7F.png", www =>
                    {
                        if (www.result == UnityWebRequest.Result.Success)
                            _cGameFileIcon = DownloadHandlerTexture.GetContent(www);
                    });
                }
                return _cGameFileIcon;
            }
        }
        
        private static void DownLoadImage(string url, Action<UnityWebRequest> callback)
        {
            UnityWebRequestTexture.GetTexture(url).SendWebRequest().completed += asyncOperation =>
            {
                if (asyncOperation is not UnityWebRequestAsyncOperation unityWebRequestAsyncOperation)
                    return;
                var www = unityWebRequestAsyncOperation.webRequest;
                callback?.Invoke(www);
                www.Dispose();
            };
        }
        
        static CGameIcon()
        {
            EditorApplication.projectWindowItemOnGUI += OnDrawProjectWindowItem;
        }

        private static void OnDrawProjectWindowItem(string guid, Rect rect)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(path);
            if (asset != null && asset.name is "CGame" && CGameFileIcon != null)
            {
                var isSmall = rect.width > rect.height;
                if (isSmall)
                    rect.width = rect.height;
                else
                    rect.height = rect.width;
                
                if (rect.width >= LargeIconSize)
                    DrawLarge(rect);
                else
                    DrawSmall(rect);
            }
        }

        private static void DrawLarge(Rect rect)
        {
	        var offset = (rect.width - LargeIconSize) * 0.5f;
	        rect = new Rect(rect.x + offset, rect.y + offset, LargeIconSize, LargeIconSize);
            var miniRect = new Rect(rect.center, rect.size * 0.5f);
            if (CGameFileIcon != null)
                GUI.DrawTexture(miniRect, CGameFileIcon);
        }

        private static void DrawSmall(Rect rect)
        {
            var miniRect = new Rect(rect.center, rect.size * 0.5f);
            if (CGameFileIcon != null)
                GUI.DrawTexture(miniRect, CGameFileIcon);
        }
    }
}