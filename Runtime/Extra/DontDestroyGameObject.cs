using System.Collections.Generic;
using UnityEngine;

public class DontDestroyGameObject : MonoBehaviour
{
    [SerializeField] private string onlyKey;
    
    private static readonly Dictionary<string, GameObject> _dontDestroyGameObjects = new();
    
    private void Awake()
    {
        DontDestroyOnLoad();
    }
    
    private void DontDestroyOnLoad()
    {
        if (string.IsNullOrEmpty(onlyKey))
            onlyKey = gameObject.name;
        if (_dontDestroyGameObjects.ContainsKey(onlyKey))
            Destroy(gameObject);
        else
        {
            _dontDestroyGameObjects.Add(onlyKey, gameObject);
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_dontDestroyGameObjects[onlyKey] != gameObject)
            return;
        _dontDestroyGameObjects.Remove(onlyKey);
    }
}
