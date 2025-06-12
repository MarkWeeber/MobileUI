using UnityEngine;

public class SceneManagement : SingletonBehaviour<SceneManagement>
{
    [SerializeField] private ScenesManagementSO _sceneManagementAsset;
    public ScenesManagementSO ScenesManagementSO { get => _sceneManagementAsset; }

    protected override void Initialize()
    {
        dontDestroyOnload = true;
        if (_sceneManagementAsset == null)
        {
            Debug.LogError("Please put reference to Scene management settings asset!");
            LogUI.Instance.SendLogInformation("SCENE MANAGEMENT IS MISSING", LogUI.MessageType.ERROR);
        }
    }
}
