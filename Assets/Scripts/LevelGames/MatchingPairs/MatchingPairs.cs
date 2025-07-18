using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchingPairs : MonoBehaviour
{
    [SerializeField] private ProgressBarSliderUI _progressBarSliderUI;

    private LocalSaveManager _localSaveManager;
    private TimerUI _timerUI;
    private ScoresUI _scoresUI;
    private GameLevelsAsset _gameLevelsAsset;
    private LevelSceneInfo _levelSceneInfo;
    private int _currentStage = 0;
    private FlipCardUI[] _cardUIs;
    private FlipCardUI _previousCard;

    private System.Random _random;

    private void Start()
    {
        _localSaveManager = LocalSaveManager.Instance;
        _timerUI = TimerUI.Instance;
        _scoresUI = ScoresUI.Instance;
        _random = new System.Random();
        GetLevelData();
        AnnounceOnStart();
        InitializeProgressBar();
        GetAllCards();
        AssignCallbacks();
    }

    private void OnDestroy()
    {
        RemoveCallBacks();
    }

    private void GetLevelData()
    {
        _gameLevelsAsset = GameContext.Instance.GameLevelsAsset;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        _levelSceneInfo = _gameLevelsAsset.LevelSceneInfos.Where(n => n.LevelSceneBuildIndex == currentSceneIndex).First();
    }

    private void AnnounceOnStart()
    {
        GeneralInformationWindowUI.Instance.CallWindow(message: _levelSceneInfo.InfoMessage, action: () => { _timerUI.StartTimer(); RestartDeck(); });
    }

    private void InitializeProgressBar()
    {
        _progressBarSliderUI.StageCount = _levelSceneInfo.LevelStageCount;
        _progressBarSliderUI.Initialize();
    }

    private void GetAllCards()
    {
        _cardUIs = GetComponentsInChildren<FlipCardUI>();
    }

    private void AssignCallbacks()
    {
        foreach (var card in _cardUIs)
        {
            card.OnFrontFaced += OnCardFlipped;
        }
    }

    private void RemoveCallBacks()
    {
        foreach (var card in _cardUIs)
        {
            card.OnFrontFaced += OnCardFlipped;
        }
    }

    private void RestartDeck()
    {
        ShuffleCards();
        FlipEachCard();
    }

    private void FlipEachCard()
    {
        for (int i = 0; i < _cardUIs.Length; i++)
        {
            _cardUIs[i].Restart();
        }
        FlipCardUI.InteractionEnabled = true;
    }

    private void ShuffleCards()
    {
        int cardsCount = _cardUIs.Length;
        for (int i = 0; i < cardsCount; i++)
        {
            _cardUIs[i].transform.SetSiblingIndex(_random.Next(cardsCount));
        }
    }

    private async void OnCardFlipped(FlipCardUI card)
    {
        // check if more than one picked
        if (_previousCard == null)
        {
            // not same card
            if (_previousCard != card)
            {
                _previousCard = card;
            }
            FlipCardUI.InteractionEnabled = true;
        }
        // different type
        else if (_previousCard.Type != card.Type)
        {
            // flip back both
            _previousCard.Restart();
            card.Restart(enableInteractionsOnEnd: true);
        }
        // same types
        else
        {
            // shrink both
            _previousCard.Shrink();
            card.Shrink(enableInteractionsOnEnd: true);
            // check if all cards are shrunk, if so then push progress
            if (AllCardsWin())
            {
                _currentStage++;
                _progressBarSliderUI.PushProgress();
                // restart deck if still stages left, otherwise call level win
                if (_currentStage >= _levelSceneInfo.LevelStageCount) // last stage win
                {
                    await OnLevelWin();
                }
                // still rounds left
                else
                {
                    RestartDeck();
                }
            }
        }
    }

    private async Task OnLevelWin()
    {
        _localSaveManager.SubmitScore(_levelSceneInfo.LevelId, _timerUI.Timer);
        // TO-DO call scores board
        _scoresUI.ShowPanel();
        // get all scores on this level
        var allScoresOnThisLevel = await _localSaveManager.GetScores(_levelSceneInfo.LevelId);
        // feed the scores to score board
        _scoresUI.AddScores(allScoresOnThisLevel);
    }

    private bool AllCardsWin()
    {
        bool result = true;
        for (int i = 0; i < _cardUIs.Length; i++)
        {
            if (!_cardUIs[i].CardShrunk)
            {
                result = false;
                break;
            }
        }
        return result;
    }


}
