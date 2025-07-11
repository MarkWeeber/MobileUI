using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableHolderUI : MonoBehaviour, IDropHandler
{
    public DraggableNumberUI HoldingDraggable { get; set; }
    private DraggableNumberUI _incomingHoldingDraggable;

    private static List<DraggableHolderUI> _instances = new List<DraggableHolderUI>();

    private void Start()
    {
        _instances.Add(this);
        var childNumber = GetComponentInChildren<DraggableNumberUI>();
        if (childNumber != null)
        {
            HoldingDraggable = childNumber;
            childNumber.SetHousingHolder(this);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent<DraggableNumberUI>(out _incomingHoldingDraggable))
        {
            if (HoldingDraggable == null && HoldingDraggable != _incomingHoldingDraggable)
            {
                OnDraggableAccepted(_incomingHoldingDraggable);
                _incomingHoldingDraggable.SetHousingHolder(this);
                HoldingDraggable = _incomingHoldingDraggable;
            }
        }
    }

    private static void OnDraggableAccepted(DraggableNumberUI draggableNumber)
    {
        foreach (var instance in _instances)
        {
            if (instance.HoldingDraggable == draggableNumber)
            {
                instance.HoldingDraggable = null;
            }
        }
    }

}
