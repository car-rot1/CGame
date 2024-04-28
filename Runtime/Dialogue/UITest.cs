using System;
using System.Collections.Generic;
using CGame;
using TMPro;
using UnityEngine.UI;

public class UITest : SingletonMonoBehaviour<UITest>
{
    public TextMeshProUGUI authorText;
    public TextMeshProUGUI contentText;
    public Button nextDialogueButton;

    public List<TextMeshProUGUI> optionTexts;
    public List<Button> optionButtons;

    public void ShowContent(string author, string content, Action callback = null)
    {
        authorText.text = author;
        contentText.text = content;
        
        if (callback == null)
            nextDialogueButton.gameObject.SetActive(false);
        else
        {
            nextDialogueButton.gameObject.SetActive(true);
            nextDialogueButton.onClick.RemoveAllListeners();
            nextDialogueButton.onClick.AddListener(() => callback?.Invoke());
        }
        foreach (var optionText in optionTexts)
            optionText.gameObject.SetActive(false);
        foreach (var optionButton in optionButtons)
            optionButton.gameObject.SetActive(false);
    }

    public void ShowOption(List<string> options, Action<int> callback)
    {
        for (var i = 0; i < options.Count; i++)
        {
            var optionText = optionTexts[i];
            optionText.gameObject.SetActive(true);
            optionText.text = options[i];
        }
        for (var i = 0; i < options.Count; i++)
        {
            var index = i;
            var optionButton = optionButtons[i];
            optionButton.gameObject.SetActive(true);
            optionButton.onClick.RemoveAllListeners();
            optionButton.onClick.AddListener(() => callback?.Invoke(index));
        }
    }
}
