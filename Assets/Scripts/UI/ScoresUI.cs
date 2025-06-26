using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoresUI : SingletonBehaviour<ScoresUI>
{
    [SerializeField] private GameObject _scoreBoardElementPrefab;
    [SerializeField] private Transform _background;
    [SerializeField] private Transform _contentTransform;

    private ScoreBoardElementUI _instantiatedScoreBoardElement;

    protected override void Initialize()
    {
        dontDestroyOnload = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HidePanel();
    }

    private void ShowPanel()
    {
        _background.gameObject.SetActive(true);
    }

    private void HidePanel()
    {
        _background.gameObject.SetActive(false);
    }

    private void ClearScores()
    {
        foreach (Transform _tranform in _contentTransform.gameObject.GetComponentInChildren<Transform>())
        {
            if (_tranform == _contentTransform)
            {
                continue;
            }
            Destroy(_tranform.gameObject);
        }
    }

    public void AddScore(int number, string playerName, int lastScore, int bestScore)
    {
        _instantiatedScoreBoardElement = Instantiate(_scoreBoardElementPrefab, _contentTransform).GetComponent<ScoreBoardElementUI>();
        _instantiatedScoreBoardElement.SetNumber(number);
        _instantiatedScoreBoardElement.SetPlayerName(playerName);
        _instantiatedScoreBoardElement.SetPlayerLastScore(lastScore);
        _instantiatedScoreBoardElement.SetPlayerBestScore(bestScore);
    }

}
