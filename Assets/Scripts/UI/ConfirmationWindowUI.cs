using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfirmationWindowUI : SingletonBehaviour<ConfirmationWindowUI>
{
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private Transform _backgroundPanel;
    [SerializeField] private Button _confirmButton;

    private Action _confirmationAction;
    private Action _lastAction;

    protected override void Initialize()
    {
        dontDestroyOnload = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void CallConfirmationWindow(string message, Action action)
    {
        _backgroundPanel.gameObject.SetActive(true);
        _titleText.text = message;
        _confirmationAction += action;
        _lastAction = action;
    }

    public void OnConfirmButtonClick()
    {
        if (_confirmationAction != null)
        {
            _confirmationAction.Invoke();
            _confirmationAction -= _lastAction;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _backgroundPanel.gameObject.SetActive(false);
    }
}
