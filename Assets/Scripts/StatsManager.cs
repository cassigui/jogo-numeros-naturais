using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class StatsManager : MonoBehaviour
{
    // Singleton para garantir acesso único de qualquer lugar
    public static StatsManager Instance { get; private set; }

    private string filePath;
    public GameReport report = new GameReport(); // Sua variável correta se chama 'report'

    void Awake()
    {
        // Padrão Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Faz com que o objeto não seja destruído ao mudar de cena

            filePath = Path.Combine(Application.persistentDataPath, "game_report.json");
            LoadStats();
            Debug.Log("StatsManager inicializado. Caminho: " + filePath);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Função pública para forçar o StatsManager a reler o JSON (útil após o menu salvar o nome)
    public void RecarregarDadosDoDisco()
    {
        LoadStats();
    }

    // Helper interno para buscar o PlayerReport do aluno ativo do momento
    private PlayerReport ObterAlunoAtivo()
    {
        if (report == null || string.IsNullOrEmpty(report.currentPlayerName))
        {
            Debug.LogWarning("[StatsManager] Nenhum aluno ativo definido no currentPlayerName.");
            return null;
        }

        // Busca o aluno na lista
        PlayerReport aluno = report.players.Find(p => p.playerName == report.currentPlayerName);
        
        // Se por algum motivo o aluno não existir na lista, cria ele por segurança
        if (aluno == null)
        {
            aluno = new PlayerReport { playerName = report.currentPlayerName };
            report.players.Add(aluno);
        }

        return aluno;
    }

    // Helper interno para buscar ou criar uma fase para o aluno ativo
    private LevelData ObterOuCriarFaseDoAluno(PlayerReport aluno, string levelName)
    {
        if (aluno.levels == null) aluno.levels = new List<LevelData>();

        LevelData data = aluno.levels.Find(l => l.levelName == levelName);
        if (data == null)
        {
            data = new LevelData { levelName = levelName, timesPlayed = 0, totalErrors = 0, wins = 0, totalRemainingLivesAtWin = 0 };
            aluno.levels.Add(data);
        }
        return data;
    }

    // Chama isso no Start de cada FaseManager
    public void RegisterLevelStart(string levelName)
    {
        PlayerReport alunoAtivo = ObterAlunoAtivo();
        if (alunoAtivo != null)
        {
            LevelData data = ObterOuCriarFaseDoAluno(alunoAtivo, levelName);
            data.timesPlayed++;
            SaveStats();
        }
    }

    // Chama isso sempre que o jogador perder uma vida
    public void RegisterError(string levelName)
    {
        PlayerReport alunoAtivo = ObterAlunoAtivo();
        if (alunoAtivo != null)
        {
            LevelData data = ObterOuCriarFaseDoAluno(alunoAtivo, levelName);
            data.totalErrors++;
            SaveStats();
        }
    }

    public void RegisterWin(string levelName, int remainingLives)
    {
        if (Instance == null)
        {
            Instance = Object.FindAnyObjectByType<StatsManager>();
            if (Instance == null) return;
        }

        PlayerReport alunoAtivo = ObterAlunoAtivo();
        if (alunoAtivo != null)
        {
            LevelData data = ObterOuCriarFaseDoAluno(alunoAtivo, levelName);
            data.wins++;
            data.totalRemainingLivesAtWin = remainingLives;
            SaveStats();
        }
    }

    public void SaveStats()
    {
        string json = JsonUtility.ToJson(report, true);
        File.WriteAllText(filePath, json);
    }

    public void LoadStats()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            report = JsonUtility.FromJson<GameReport>(json);
            
            // Garante que a lista de jogadores nunca fique nula
            if (report == null) report = new GameReport();
            if (report.players == null) report.players = new List<PlayerReport>();
        }
        else
        {
            report = new GameReport();
        }
    }
}