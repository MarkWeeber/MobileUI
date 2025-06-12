using UnityEngine;

[CreateAssetMenu(fileName = "Scene Management Asset", menuName = "Custom Assets/Scene Management Asset")]
public class ScenesManagementSO : ScriptableObject
{
    public int MainSceneBuildIndex = 0;
    public LevelSceneInfo[] LevelSceneInfos;
}

[System.Serializable]
public struct LevelSceneInfo
{
    public int LevelSceneBuildIndex;
    public string InfoMessage;
    public int LevelStageCount;
}