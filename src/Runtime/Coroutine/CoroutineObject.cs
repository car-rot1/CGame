using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CGame
{
    public static class CoroutineObject
    {
        private class CoroutineComponent : MonoBehaviour
        {
        }
        
        private static CoroutineComponent _coroutineComponent;
        
        public static void StartCoroutine(IEnumerator coroutine)
        {
            if (!Application.isPlaying)
                return;
            
            if (_coroutineComponent == null)
            {
                var go = new GameObject(nameof(CoroutineObject));
                _coroutineComponent = go.AddComponent<CoroutineComponent>();
                Object.DontDestroyOnLoad(go);
            }

            _coroutineComponent.StartCoroutine(coroutine);
        }
    }
}