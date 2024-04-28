using CGame;
using UnityEngine;
using UnityEngine.UI;

public class ProcessSlider : MonoBehaviour
{
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.value = 0;
        SceneSwitch.Instance.OnProcessChange += ProcessChange;
    }

    private void ProcessChange(float oldValue, float newValue) => _slider.value = newValue;

    private void OnDestroy()
    {
        SceneSwitch.Instance.OnProcessChange -= ProcessChange;
    }
}
