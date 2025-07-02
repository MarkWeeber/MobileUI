using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneControlsUI : MonoBehaviour
{
    [SerializeField] InputField _playerNameInputField;

    //private ProfileControl _profileControl;
    private LocalSaveManager _localSaveManager;
    private string _enteredPlayerName;

    private void Start()
    {
        //_profileControl = ProfileControl.Instance;
        _localSaveManager = LocalSaveManager.Instance;
    }

    public async void SubmitPlayerName(UnityEventContainer unityEventContainer)
    {
        _enteredPlayerName = _playerNameInputField.text;
        if (_enteredPlayerName == "")
        {
            LogUI.Instance.SendLogInformation("Please Enter Your Name", LogUI.MessageType.WARNING);
            return;
        }
        await _localSaveManager.SubmitProfileName(_enteredPlayerName);
        //_profileControl.SubmitPlayerName(_enteredPlayerName);
        unityEventContainer.Invoke();
    }

    public void LoadSceneIndex(int index)
    {
        SceneManager.LoadScene(index);
    }
}
