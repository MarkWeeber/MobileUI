using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EquationGame : MonoBehaviour
{
    [SerializeField] private ProgressBarSliderUI _progressBarSliderUI;

    private LocalSaveManager _localSaveManager;
    private TimerUI _timerUI;
    private ScoresUI _scoresUI;
    private GameLevelsAsset _gameLevelsAsset;
    private LevelSceneInfo _levelSceneInfo;

    private void Start()
    {
        _localSaveManager = LocalSaveManager.Instance;
        _timerUI = TimerUI.Instance;
        _scoresUI = ScoresUI.Instance;
        GetLevelData();
        AnnounceOnStart();
    }

    private void GetLevelData()
    {
        _gameLevelsAsset = GameContext.Instance.GameLevelsAsset;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        _levelSceneInfo = _gameLevelsAsset.LevelSceneInfos.Where(n => n.LevelSceneBuildIndex == currentSceneIndex).First();
    }

    private void AnnounceOnStart()
    {
        GeneralInformationWindowUI.Instance.CallWindow(message: _levelSceneInfo.InfoMessage, action: () => { _timerUI.StartTimer(); });
    }
}
