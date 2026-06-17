using UnityEngine;
using System.IO;
using System.Linq;

public class StatsManager : MonoBehaviour
{
    // Singleton para garantir acesso único de qualquer lugar
    public static StatsManager Instance { get; private set; }

    private string filePath;
    public GameReport report = new GameReport();

    public void AtualizarNomeDoJogador(string novoNome)
    {
        if (this.report != null) 
        {
            this.report.playerName = novoNome;
            Debug.Log($"[StatsManager] Nome atualizado na memória: {novoNome}");
        }
    }
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

    // Chama isso no Start de cada FaseManager
    public void RegisterLevelStart(string levelName)
    {
        LevelData data = report.levels.FirstOrDefault(l => l.levelName == levelName);

        if (data == null)
        {
            data = new LevelData { levelName = levelName, timesPlayed = 1, totalErrors = 0 };
            report.levels.Add(data);
        }
        else
        {
            data.timesPlayed++;
        }
        SaveStats();
    }

    // Chama isso sempre que o jogador perder uma vida
    public void RegisterError(string levelName)
    {
        LevelData data = report.levels.FirstOrDefault(l => l.levelName == levelName);
        if (data != null)
        {
            data.totalErrors++;
            SaveStats();
        }
    }

    private void SaveStats()
    {
        string json = JsonUtility.ToJson(report, true);
        File.WriteAllText(filePath, json);
    }

    private void LoadStats()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            report = JsonUtility.FromJson<GameReport>(json);
        }
    }

    public void RegisterWin(string levelName, int remainingLives)
    {
        if (Instance == null)
        {
            Debug.LogWarning("StatsManager.Instance estava nulo. Tentando recuperar...");
            Instance = Object.FindAnyObjectByType<StatsManager>();
            if (Instance == null) return; // Sai para evitar o erro se não achar nada
        }

        LevelData data = report.levels.FirstOrDefault(l => l.levelName == levelName);
        if (data != null)
        {
            data.wins++;
            data.totalRemainingLivesAtWin = remainingLives;
            SaveStats();
        }
    }
}