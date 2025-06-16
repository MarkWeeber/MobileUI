using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneControlsUI : MonoBehaviour
{
    [SerializeField] InputField _playerNameInputField;

    private ProfileControl _profileControl;
    private string _enteredPlayerName;

    private void Start()
    {
        _profileControl = ProfileControl.Instance;
    }

    public void SubmitPlayerName(UnityEventContainer unityEventContainer)
    {
        _enteredPlayerName = _playerNameInputField.text;
        if (_enteredPlayerName == "")
        {
            LogUI.Instance.SendLogInformation("Please Enter Your Name", LogUI.MessageType.WARNING);
            return;
        }
        _profileControl.SubmitPlayerName(_enteredPlayerName);
        unityEventContainer.Invoke();
    }

    public void LoadSceneIndex(int index)
    {
        SceneManager.LoadScene(index);
    }
}
