using System.IO;
using UnityEditor;
using UnityEngine;

namespace CGame
{
    public static class AssetDatabaseExtension
    {
        public static string GetAssetAbsolutePath(Object assetObject) => Path.Combine(Application.dataPath[..^6], AssetDatabase.GetAssetPath(assetObject));
        public static string GetAssetAbsolutePath(int instanceID) => Path.Combine(Application.dataPath[..^6], AssetDatabase.GetAssetPath(instanceID));
    }
}