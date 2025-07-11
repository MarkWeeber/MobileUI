using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EquationGame : MonoBehaviour
{
    [SerializeField] private ProgressBarSliderUI _progressBarSliderUI;
    [SerializeField] private DraggableHolderUI[] _equationMemebers;
    [SerializeField] private DraggableHolderUI[] _variantMemebers;
    [SerializeField] private GameObject _draggableNumberPrefab;
    [SerializeField] private TMP_Text _firstOperator;
    [SerializeField] private TMP_Text _secondOperator;

    private LocalSaveManager _localSaveManager;
    private TimerUI _timerUI;
    private ScoresUI _scoresUI;
    private GameLevelsAsset _gameLevelsAsset;
    private LevelSceneInfo _levelSceneInfo;
    private DraggableNumberUI _instantiatedDraggableNumber;

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

    #region equation set-up
    private void SetUpEquation()
    {
        
    }
    #endregion

    #region equiation checking

    #endregion

    #region fixed equation examples

    private enum Operator
    {
        Add, Subtract, Multiply, Divide
    }

    private struct EquationRow
    {
        public int a;
        public Operator firstOperator;
        public int b;
        public Operator secondOperator;
        public int c;
        public int answer;
        public EquationRow(int a, Operator firstOperator, int b, Operator secondOperator, int c, int answer)
        {
            this.a = a;
            this.firstOperator = firstOperator;
            this.b = b;
            this.secondOperator = secondOperator;
            this.c = c;
            this.answer = answer;
        }
    }

    private EquationRow[] equations = new EquationRow[]
    {
        // Multiplication first
        new EquationRow(2, Operator.Add, 3, Operator.Multiply, 4, 14),    // 2 + 3×4 = 2 + 12 = 14
        new EquationRow(5, Operator.Subtract, 2, Operator.Multiply, 6, -7), // 5 - 2×6 = 5 - 12 = -7
        new EquationRow(4, Operator.Add, 5, Operator.Divide, 5, 5),         // 4 + 5/5 = 4 + 1 = 5
        new EquationRow(10, Operator.Subtract, 6, Operator.Divide, 2, 7),  // 10 - 6/2 = 10 - 3 = 7
        
        // Division first
        new EquationRow(12, Operator.Add, 6, Operator.Divide, 3, 14),      // 12 + 6/3 = 12 + 2 = 14
        new EquationRow(20, Operator.Subtract, 15, Operator.Divide, 5, 17),// 20 - 15/5 = 20 - 3 = 17
        
        // Mixed operators
        new EquationRow(3, Operator.Multiply, 4, Operator.Add, 5, 17),     // 3×4 + 5 = 12 + 5 = 17
        new EquationRow(6, Operator.Multiply, 2, Operator.Subtract, 3, 9), // 6×2 - 3 = 12 - 3 = 9
        new EquationRow(8, Operator.Divide, 2, Operator.Add, 6, 10),       // 8/2 + 6 = 4 + 6 = 10
        new EquationRow(9, Operator.Divide, 3, Operator.Subtract, 1, 2),   // 9/3 - 1 = 3 - 1 = 2
        
        // Negative numbers
        new EquationRow(-5, Operator.Add, 3, Operator.Multiply, 4, 7),     // -5 + 3×4 = -5 + 12 = 7
        new EquationRow(4, Operator.Subtract, 6, Operator.Multiply, 2, -8),// 4 - 6×2 = 4 - 12 = -8
        new EquationRow(-8, Operator.Divide, 4, Operator.Add, 5, 3),       // -8/4 + 5 = -2 + 5 = 3
        new EquationRow(10, Operator.Add, -4, Operator.Divide, 2, 8),      // 10 + (-4)/2 = 10 - 2 = 8
        
        // Larger numbers (still within range)
        new EquationRow(15, Operator.Add, 5, Operator.Multiply, 10, 65),    // 15 + 5×10 = 15 + 50 = 65
        new EquationRow(20, Operator.Subtract, 4, Operator.Multiply, 5, 0), // 20 - 4×5 = 20 - 20 = 0
        new EquationRow(25, Operator.Divide, 5, Operator.Add, 10, 15),      // 25/5 + 10 = 5 + 10 = 15
        new EquationRow(30, Operator.Subtract, 10, Operator.Divide, 5, 28), // 30 - 10/5 = 30 - 2 = 28
        
        // Edge cases
        new EquationRow(0, Operator.Add, 5, Operator.Multiply, 10, 50),     // 0 + 5×10 = 0 + 50 = 50
        new EquationRow(10, Operator.Multiply, 0, Operator.Add, 5, 5),      // 10×0 + 5 = 0 + 5 = 5
        new EquationRow(-10, Operator.Multiply, 2, Operator.Add, 20, 0),    // -10×2 + 20 = -20 + 20 = 0
        
        // More complex examples
        new EquationRow(7, Operator.Multiply, 3, Operator.Subtract, 5, 16), // 7×3 - 5 = 21 - 5 = 16
        new EquationRow(12, Operator.Divide, 4, Operator.Multiply, 3, 9),   // 12/4 × 3 = 3 × 3 = 9
        new EquationRow(6, Operator.Add, 4, Operator.Multiply, 5, 26),      // 6 + 4×5 = 6 + 20 = 26
        new EquationRow(8, Operator.Subtract, 4, Operator.Divide, 2, 6)     // 8 - 4/2 = 8 - 2 = 6
    };
    #endregion
}
