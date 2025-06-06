using System.Collections.Generic;

[System.Serializable]
public class ProgressData
{
    [System.Serializable]
    public struct ProgressMetric
    {
        public int level;
        public int maxScore;
        public int lastScore;

    }
    public string ProfileName = "";
    public List<ProgressMetric> ProgressMetrics = new List<ProgressMetric>();

    public ProgressData(string profileName)
    {
        ProfileName = profileName;
        ProgressMetrics = new List<ProgressMetric>();
    }
}
