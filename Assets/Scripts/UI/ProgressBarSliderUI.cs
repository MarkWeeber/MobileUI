using UnityEngine;
using UnityEngine.UI;

public class ProgressBarSliderUI : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private GameObject _checkMarkPrefab;
    [SerializeField] private int _levelCount = 2;

    private CheckMarkPosition[] checkMarkPositions;
    private RectTransform _rectTransform;
    private GameObject _instantiatedObject;
    private Vector2 _setAnchor = new Vector2(0.5f, 0.5f);
    private ProgressMarkUI _progressMarkUI;


    private void Start()
    {
        checkMarkPositions = new CheckMarkPosition[_levelCount];
        PlaceCheckMarksAtStart();
        _slider.onValueChanged.AddListener(SliderValueChanged); // for testing purposes from UI inspector, need to comment this line before build
    }

    private void PlaceCheckMarksAtStart()
    {
        if (_levelCount > 1)
        {
            for (int i = 0; i < _levelCount; i++)
            {
                _setAnchor.x = (float)(i / (float)(_levelCount - 1));
                CreateAndPlaceCheckMark(_setAnchor, i);
            }
        }
        else if (_levelCount == 1)
        {
            _setAnchor.x = 0.5f;
            CreateAndPlaceCheckMark(_setAnchor, 0);
        }
    }

    private void CreateAndPlaceCheckMark(Vector2 setAnchor, int index)
    {
        _instantiatedObject = GameObject.Instantiate(_checkMarkPrefab, _slider.transform);
        _rectTransform = _instantiatedObject.GetComponent<RectTransform>();
        _rectTransform.anchorMin = setAnchor;
        _rectTransform.anchorMax = setAnchor;
        _rectTransform.anchoredPosition = Vector2.zero;
        _progressMarkUI = _instantiatedObject.GetComponent<ProgressMarkUI>();
        checkMarkPositions[index].setAnchor = setAnchor;
        checkMarkPositions[index].progressMarkUI = _progressMarkUI;
    }

    private void SliderValueChanged(float newValue)
    {
        bool currentLevelFound = (newValue == 1f)?true:false;
        for (int i = _levelCount - 1; i >= 0; i--)
        {
            if (currentLevelFound)
            {
                checkMarkPositions[i].progressMarkUI.SetComplete();
                continue;
            }
            float currentX = checkMarkPositions[i].setAnchor.x;
            if (newValue > currentX)
            {
                checkMarkPositions[i].progressMarkUI.SetComplete();
                currentLevelFound = true;
            }
            else
            {
                if ((i > 0 && newValue > checkMarkPositions[i - 1].setAnchor.x) || (i == 0 && newValue == 0)) // there are still some checkmarks left
                {
                    checkMarkPositions[i].progressMarkUI.SetPending();
                    currentLevelFound = true;
                }
                else
                {
                    checkMarkPositions[i].progressMarkUI.SetIncomplete();
                }
            }
        }
    }

    private struct CheckMarkPosition
    {
        public Vector2 setAnchor;
        public ProgressMarkUI progressMarkUI;
    }
}
