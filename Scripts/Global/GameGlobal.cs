using System;

public static class GameGlobal
{
    public static event Action OnPlay;
    public static event Action OnPause;
    public static event Action<float> OnSetGameTimeScale;

    public static void Play() => OnPlay?.Invoke();
    public static void Pause() => OnPause?.Invoke();
    public static void SetGameTimeScale(float timeScale) => OnSetGameTimeScale?.Invoke(timeScale);
}
