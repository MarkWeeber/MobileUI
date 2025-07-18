using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FlipCardUI : MonoBehaviour, IPointerClickHandler
{
    public static Action<bool> InteractionSwitch;
    private static bool _interactionEnabled;
    public static bool InteractionEnabled
    {
        get { return _interactionEnabled; }
        set { InteractionSwitch?.Invoke(value); _interactionEnabled = value; }
    }
    public enum CardType { First, Second, Third, Fourth, Fifth, Sixth }

    [SerializeField] private CardType _cardType;
    [SerializeField] private Image _basePanelImage;
    [SerializeField] private Image _backImage;
    [SerializeField] private Image _cardImage;
    [SerializeField] private float _flipDuration = 1.0f;
    [SerializeField] private float _shrinkDuration = 1.0f;
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _completeColor = Color.green;

    private Color enabledColor = Color.gray;
    private Color disabledColor = Color.red;

    public CardType Type { get => _cardType; }
    public Action<FlipCardUI> OnFrontFaced;
    public bool CardShrunk = false;

    private bool _frontFacing = true;
    private bool _shrunk = false;
    private Image _currentImage { get => (_frontFacing) ? _cardImage : _backImage; }

    private void Start()
    {
        InteractionSwitch += OnInteractionSwitch;
    }

    private void OnDestroy()
    {
        InteractionSwitch -= OnInteractionSwitch;
    }

    public void Restart(bool enableInteractionsOnEnd = false)
    {
        _basePanelImage.color = _normalColor;
        _frontFacing = true;
        _cardImage.enabled = _frontFacing;
        _backImage.enabled = !_frontFacing;
        _cardImage.transform.localScale = Vector3.one;
        _backImage.transform.localScale = Vector3.one;
        StartCoroutine(StartCoroutineWithDelay(FlippingRoutine(_flipDuration, enableInteractionsOnEnd), _flipDuration));
    }

    public void Shrink(bool enableInteractionsOnEnd = false)
    {
        InteractionEnabled = false;
        _basePanelImage.color = _completeColor;
        StartCoroutine(ShrinkRoutine(_shrinkDuration, enableInteractionsOnEnd));
        CardShrunk = true;
    }

    private void OnInteractionSwitch(bool interactionsEnabled)
    {
        if (interactionsEnabled)
        {
            _basePanelImage.color = enabledColor;
        }
        else
        {
            _basePanelImage.color = disabledColor;
        }
    }

    IEnumerator FlippingRoutine(float duration, bool enableInteractionsOnEnd = false)
    {
        float elapsed = 0f;
        float xScale = 0f;
        // current active image rotation effect
        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            xScale = Mathf.Lerp(1f, 0f, elapsed / (duration / 2f));
            _currentImage.transform.localScale = new Vector3(xScale, 1f, 1f);
            yield return null;
        }
        FlipImages();
        // second half of rotation
        elapsed = elapsed - (duration / 2f);
        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            xScale = Mathf.Lerp(0f, 1f, elapsed / (duration / 2f));
            _currentImage.transform.localScale = new Vector3(xScale, 1f, 1f);
            yield return null;
        }
        CheckIfFrontFacingAfterFlip();
        if (enableInteractionsOnEnd) InteractionEnabled = true;
    }

    IEnumerator ShrinkRoutine(float duration, bool enableInteractionsOnEnd = false)
    {
        float scale = 1f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            scale = Mathf.Lerp(1f, 0f, elapsed / duration);
            _currentImage.transform.localScale = new Vector3(scale, scale, scale);
            yield return null;
        }
        _shrunk = true;
        if (enableInteractionsOnEnd) InteractionEnabled = true;
    }

    IEnumerator StartCoroutineWithDelay(IEnumerator routine, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(routine);
    }

    private void FlipImages()
    {
        _currentImage.enabled = false;
        _frontFacing = !_frontFacing;
        _currentImage.enabled = true;
    }

    private void CheckIfFrontFacingAfterFlip()
    {
        if (_frontFacing)
        {
            OnFrontFaced?.Invoke(this);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_shrunk && _interactionEnabled)
        {
            InteractionEnabled = false;
            StartCoroutine(FlippingRoutine(_flipDuration, true));
        }
    }
}
