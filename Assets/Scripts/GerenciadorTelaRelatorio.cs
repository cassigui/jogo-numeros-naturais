using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TMPro;

public class GerenciadorTelaRelatorio : MonoBehaviour
{
    [Header("Componentes de UI Globais")]
    [SerializeField] private TextMeshProUGUI txtNomeDoJogador; 

    [Header("Blocos de Fases Cadastrados")]
    [SerializeField] private List<ElementoRelatorioFase> blocosFases;

    [Header("Nome do Arquivo de Save")]
    [SerializeField] private string nomeArquivoJson = "game_report.json";

    void OnEnable()
    {
        CarregarEAtualizarRelatorio();
    }

    private void CarregarEAtualizarRelatorio()
    {
        string caminhoArquivo = Path.Combine(Application.persistentDataPath, nomeArquivoJson);

        if (!File.Exists(caminhoArquivo))
        {
            Debug.LogWarning($"Arquivo de relatório não encontrado em: {caminhoArquivo}. Zerando painéis.");
            ZerarTodosOsBlocos();
            if (txtNomeDoJogador != null) txtNomeDoJogador.text = "Nome do Aluno: Não Registrado";
            return;
        }

        try
        {
            string jsonTexto = File.ReadAllText(caminhoArquivo);
            GameReport dadosDoRelatorio = JsonUtility.FromJson<GameReport>(jsonTexto);

            if (dadosDoRelatorio == null)
            {
                ZerarTodosOsBlocos();
                return;
            }

            if (txtNomeDoJogador != null)
            {
                if (!string.IsNullOrEmpty(dadosDoRelatorio.playerName))
                {
                    txtNomeDoJogador.text = $"Nome do Aluno: {dadosDoRelatorio.playerName}";
                }
                else
                {
                    txtNomeDoJogador.text = "Nome do Aluno: Sem Nome";
                }
            }

            if (dadosDoRelatorio.levels == null)
            {
                ZerarTodosOsBlocos();
                return;
            }

            foreach (var bloco in blocosFases)
            {
                if (bloco == null) continue;

                LevelData statusFase = dadosDoRelatorio.levels.Find(x => x.levelName == bloco.NomeDaCenaFase);

                if (statusFase != null)
                {
                    bloco.AtualizarVisual(
                        statusFase.wins, 
                        statusFase.totalErrors, 
                        statusFase.timesPlayed, 
                        statusFase.totalRemainingLivesAtWin
                    );
                }
                else
                {
                    bloco.DefinirComoSemDados();
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao ler ou processar o JSON do relatório: {e.Message}");
            ZerarTodosOsBlocos();
        }
    }

    private void ZerarTodosOsBlocos()
    {
        foreach (var bloco in blocosFases)
        {
            if (bloco != null) bloco.DefinirComoSemDados();
        }
    }
}