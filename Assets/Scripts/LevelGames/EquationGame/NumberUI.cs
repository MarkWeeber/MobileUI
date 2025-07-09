using TMPro;
using UnityEngine;

public class NumberUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _numberText;

    public void SetNumber(int number)
    {
        _numberText.text = number.ToString();
    }
}
