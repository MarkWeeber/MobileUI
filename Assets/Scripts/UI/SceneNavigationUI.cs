using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigationUI : SingletonBehaviour<SceneNavigationUI>
{
    [SerializeField] private string _restartConfirmationWindowText = "Restart Current Level?";
    [SerializeField] private string _goToMainMenuConfirmationWindowText = "Go to Main Menu?";
    [SerializeField] private Transform _backgroundPanel;

    protected override void Initialize()
    {
        dontDestroyOnload = true;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void RestartButtonOnClick()
    {
        ConfirmationWindowUI.Instance.CallConfirmationWindow(_restartConfirmationWindowText, RestartScene);
    }

    public void GoToMainMenuButtonOnClick()
    {
        ConfirmationWindowUI.Instance.CallConfirmationWindow(_goToMainMenuConfirmationWindowText, LoadMainMenu);
    }

    private void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _backgroundPanel.gameObject.SetActive(false);
    }
}
