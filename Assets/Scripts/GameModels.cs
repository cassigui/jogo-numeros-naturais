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
public class PlayerReport
{
    public string playerName; // O nome do aluno dono deste histórico
    public List<LevelData> levels = new List<LevelData>();
}

[Serializable]
public class GameReport
{
    // Armazena o nome de quem está jogando AGORA nesta sessão
    public string currentPlayerName; 
    
    // Lista com o histórico de todos os alunos que já jogaram no computador
    public List<PlayerReport> players = new List<PlayerReport>();
}