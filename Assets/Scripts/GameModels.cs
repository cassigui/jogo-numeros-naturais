using System;
using System.Collections.Generic;

[Serializable]
public class LevelData
{
    public string levelName;
    public int timesPlayed;
    public int totalErrors;
    public int wins;
    public int totalRemainingLivesAtWin;
}

[Serializable]
public class GameReport
{   
    public string playerName;
    public List<LevelData> levels = new List<LevelData>();
}