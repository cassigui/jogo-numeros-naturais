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
            ZerarTodosOsBlocos();
            if (txtNomeDoJogador != null) txtNomeDoJogador.text = "Aluno: Não Registrado";
            return;
        }

        try
        {
            string jsonTexto = File.ReadAllText(caminhoArquivo);
            GameReport dadosDoRelatorio = JsonUtility.FromJson<GameReport>(jsonTexto);

            if (dadosDoRelatorio == null || string.IsNullOrEmpty(dadosDoRelatorio.currentPlayerName))
            {
                ZerarTodosOsBlocos();
                if (txtNomeDoJogador != null) txtNomeDoJogador.text = "Aluno: Sem dados";
                return;
            }

            // 1. Exibe o nome do usuário atual na parte superior da tela
            string usuarioAtual = dadosDoRelatorio.currentPlayerName;
            if (txtNomeDoJogador != null) txtNomeDoJogador.text = $"Aluno: {usuarioAtual}";

            // 2. Busca na lista global APENAS o histórico do usuário atual
            PlayerReport dadosDoUsuarioAtual = dadosDoRelatorio.players.Find(p => p.playerName == usuarioAtual);

            // 3. Distribui os dados das fases (apenas as deste usuário) nos blocos visuais
            foreach (var bloco in blocosFases)
            {
                if (bloco == null) continue;

                // Se o usuário atual tiver dados salvos e a lista de fases dele não for nula
                if (dadosDoUsuarioAtual != null && dadosDoUsuarioAtual.levels != null)
                {
                    LevelData statusFase = dadosDoUsuarioAtual.levels.Find(x => x.levelName == bloco.NomeDaCenaFase);

                    if (statusFase != null)
                    {
                        bloco.AtualizarVisual(
                            statusFase.wins,
                            statusFase.totalErrors,
                            statusFase.timesPlayed,
                            statusFase.totalRemainingLivesAtWin
                        );
                        continue;
                    }
                }

                // Se o usuário atual nunca jogou essa fase específica, zera o bloco visual
                bloco.DefinirComoSemDados();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Erro ao processar o relatório segmentado por usuário: {e.Message}");
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