using TMPro;
using UnityEngine;

public class ScoreBoardElementUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _numberText;
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private TMP_Text _playerLastScore;
    [SerializeField] private TMP_Text _playerBestScore;

    public void SetNumber(int number)
    {
        _numberText.text = number.ToString();
    }

    private void SetPlayerName(string name)
    {
        _playerNameText.text = name;
    }

    private void SetPlayerLastScore(int score)
    {
        _playerLastScore.text = score.ToString();
    }

    private void SetPlayerBestScore(int score)
    {
        _playerBestScore.text = score.ToString();
    }
}
