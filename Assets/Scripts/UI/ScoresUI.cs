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

    public void ShowPanel()
    {
        _background.gameObject.SetActive(true);
    }

    private void HidePanel()
    {
        _background.gameObject.SetActive(false);
        ClearScores();
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

    public void AddScores(Score[] scores)
    {
        int reversedOrder = scores.Length;
        foreach (var score in scores)
        {
            _instantiatedScoreBoardElement = Instantiate(_scoreBoardElementPrefab, _contentTransform).GetComponent<ScoreBoardElementUI>();
            _instantiatedScoreBoardElement.SetNumber(reversedOrder);
            _instantiatedScoreBoardElement.SetPlayerName(score.PlayerName);
            _instantiatedScoreBoardElement.SetPlayerLastScore(TimerUI.GetTimeString(score.LastTime));
            _instantiatedScoreBoardElement.SetPlayerBestScore(TimerUI.GetTimeString(score.BestTime));
            reversedOrder--;
        }
    }
}
