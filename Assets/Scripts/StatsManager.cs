using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;

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
            Instance = UnityEngine.Object.FindAnyObjectByType<StatsManager>();
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

    public void ExportarRelatorioFormatado()
    {
        // 1. Carrega os dados mais recentes do disco
        LoadStats();

        if (report == null || report.players == null || report.players.Count == 0)
        {
            Debug.LogWarning("[StatsManager] Não há dados de alunos para exportar.");
            return;
        }

        // =========================================================================
        // NOVO: Descobre o caminho real para a pasta Downloads do Windows/Mac
        // =========================================================================
        string pastaHomeUsuario = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string caminhoDownloads = Path.Combine(pastaHomeUsuario, "Downloads");

        // Validação extra: Se por algum motivo bizarro a pasta Downloads não existir, 
        // usamos o persistentDataPath como plano de fundo de segurança.
        if (!Directory.Exists(caminhoDownloads))
        {
            caminhoDownloads = Application.persistentDataPath;
        }

        string dataFormatada = DateTime.Now.ToString("dd-MM-yyyy_HH-mm");
        string nomeArquivoTxt = $"Relatorio_Alunos_{dataFormatada}.txt";
        string caminhoExportacao = Path.Combine(caminhoDownloads, nomeArquivoTxt);
        // =========================================================================

        try
        {
            using (StreamWriter writer = new StreamWriter(caminhoExportacao))
            {
                // --- CABEÇALHO DO DOCUMENTO ---
                writer.WriteLine("=======================================================================");
                writer.WriteLine("                      RELATÓRIO GERAL DE DESEMPENHO                    ");
                writer.WriteLine("=======================================================================");
                writer.WriteLine($" Emitido em: {DateTime.Now.ToString("dd/MM/yyyy às HH:mm:ss")}");
                writer.WriteLine($" Total de Alunos Registrados: {report.players.Count}");
                writer.WriteLine($" Último Aluno Ativo: {report.currentPlayerName}");
                writer.WriteLine("=======================================================================\n");

                // --- CORPO DO RELATÓRIO (Por Aluno) ---
                foreach (var aluno in report.players)
                {
                    writer.WriteLine($"-----------------------------------------------------------------------");
                    writer.WriteLine($" ALUNO: {aluno.playerName.ToUpper()}");
                    writer.WriteLine($"-----------------------------------------------------------------------");

                    if (aluno.levels == null || aluno.levels.Count == 0)
                    {
                        writer.WriteLine("   * Este aluno ainda não jogou nenhuma fase.");
                        writer.WriteLine();
                        continue;
                    }

                    writer.WriteLine(string.Format("   {0,-12} | {1,-10} | {2,-10} | {3,-8} | {4,-12}",
                        "FASE", "PARTIDAS", "ERROS", "VITÓRIAS", "VIDAS FINAIS"));
                    writer.WriteLine("   -----------------------------------------------------------------");

                    foreach (var fase in aluno.levels)
                    {
                        writer.WriteLine(string.Format("   {0,-12} | {1,-10} | {2,-10} | {3,-8} | {4,-12}",
                            fase.levelName,
                            fase.timesPlayed,
                            fase.totalErrors,
                            fase.wins,
                            fase.totalRemainingLivesAtWin));
                    }
                    writer.WriteLine();
                }

                writer.WriteLine("=======================================================================");
                writer.WriteLine("                          FIM DO RELATÓRIO                             ");
                writer.WriteLine("=======================================================================");
            }

            Debug.Log($"[SUCESSO] Relatório exportado direto para os Downloads em:\n{caminhoExportacao}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[ERRO] Falha ao exportar para a pasta Downloads: {e.Message}");
        }
    }
}