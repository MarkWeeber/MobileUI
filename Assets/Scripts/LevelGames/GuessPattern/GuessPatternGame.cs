using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuessPatternGame : MonoBehaviour
{
    [SerializeField] private Transform _guessPatterPanel;
    [SerializeField] private int _minRandomNumber = 1;
    [SerializeField] private int _maxRandomNumber = 99;
    [SerializeField] private ProgressBarSliderUI _progressBarSliderUI;

    private PatternFrameUI[] _patternFrameUIs;
    private PatternFrameUI _patterFrameUI;
    private GameLevelsAsset _gameLevelsAsset;
    private LevelSceneInfo _levelSceneInfo;
    private int _newRandomNumber;
    private int[] _selectedPattern;
    private int _clickedCount = 0;
    private int _currentStage = 0;

    private void Start()
    {
        GetLevelData();
        InitializeProgressBar();
        InitializePatternFrames();
        RandomizeNumbers(registerCallbacks: true);
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
        GeneralInformationWindowUI.Instance.CallSimpleWindow(_levelSceneInfo.InfoMessage);
    }

    private void InitializePatternFrames()
    {
        _patternFrameUIs = _guessPatterPanel.GetComponentsInChildren<PatternFrameUI>();
        _selectedPattern = new int[_patternFrameUIs.Length];
    }

    private void InitializeProgressBar()
    {
        _progressBarSliderUI.StageCount = _levelSceneInfo.LevelStageCount;
        _progressBarSliderUI.Initialize();
    }

    private void RandomizeNumbers(bool registerCallbacks = false, bool reset = false)
    {
        for (int i = 0; i < _patternFrameUIs.Length; i++)
        {
            _patterFrameUI = _patternFrameUIs[i];
            _newRandomNumber = Random.Range(_minRandomNumber, _maxRandomNumber + 1);
            _patterFrameUI.SetNumber(_newRandomNumber);
            if (reset)
            {
                _patterFrameUI.ClearClick();
            }
            if (registerCallbacks)
            {
                _patterFrameUI.OnClicked += OnPatternFrameClicked;
            }
        }
    }

    private void OnPatternFrameClicked(PatternFrameUI patterFrameUI)
    {
        if (_clickedCount == _patternFrameUIs.Length - 1) // last member
        {
            _selectedPattern[_clickedCount] = patterFrameUI.Number;
            _clickedCount = 0;
            if (IsArraySortedAscending(_selectedPattern))
            {
                // win condition
                ResetPattern(true);
            }
            else
            {
                ResetPattern();
            }
            ClearArray(_selectedPattern);
        }
        else
        {
            _selectedPattern[_clickedCount] = patterFrameUI.Number;
            _clickedCount++;
        }

    }

    private void ResetPattern(bool won = false)
    {
        RandomizeNumbers(reset: true);
        if (won)
        {
            _currentStage++;
            _progressBarSliderUI.PushProgress();
            if (_currentStage >= _levelSceneInfo.LevelStageCount)
            {
                Debug.Log("LEVEL WIN");
            }
        }
        else
        {
            Debug.Log("TRY AGAIN");
        }
    }

    private bool IsArraySortedAscending(int[] array)
    {
        return array.SequenceEqual(array.OrderBy(n => n));
    }

    private void ClearArray(int[] array, int number = 0)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = number;
        }
    }
}
