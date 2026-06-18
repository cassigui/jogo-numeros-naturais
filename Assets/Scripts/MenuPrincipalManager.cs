using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 
using System.IO;

public class MenuPrincipal : MonoBehaviour
{
    [SerializeField] private string nomeDoLevelDeJogo;
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject modalNome;

    [Header("Componente de Input de Nome")]
    [SerializeField] private TMP_InputField inputFieldNome;
    [SerializeField] private string nomeArquivoJson = "game_report.json";

    [Header("Configurações de Áudio")]
    [SerializeField] private AudioSource audioSourceEfeitos;
    [SerializeField] private AudioSource audioSourceMusica;
    [SerializeField] private AudioClip musicaDeFundo;

    [Header("Efeitos Sonoros")]
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip returnSound;
    [SerializeField] private AudioClip startGameSound;

    void Start()
    {
        TocarMusicaDeFundo();
    }

    private void TocarMusicaDeFundo()
    {
        if (audioSourceMusica != null && musicaDeFundo != null)
        {
            audioSourceMusica.clip = musicaDeFundo;
            audioSourceMusica.loop = true;
            audioSourceMusica.playOnAwake = false;
            audioSourceMusica.volume = 0.05f;

            audioSourceMusica.Play();
        }
    }

    public void PlayHover()
    {
        if (audioSourceEfeitos != null && hoverSound != null)
            audioSourceEfeitos.PlayOneShot(hoverSound);
    }

    public void StartSound()
    {
        if (audioSourceEfeitos != null && startGameSound != null)
            audioSourceEfeitos.PlayOneShot(startGameSound);
    }

    public void PlayClick()
    {
        if (audioSourceEfeitos != null && clickSound != null)
            audioSourceEfeitos.PlayOneShot(clickSound);
    }

    public void PlayReturn()
    {
        if (audioSourceEfeitos != null && returnSound != null)
            audioSourceEfeitos.PlayOneShot(returnSound);
    }

    public void Jogar()
    {
        SalvarNomeNoReport();
        SceneManager.LoadScene(nomeDoLevelDeJogo);
    }

    public void AbrirOpcoes()
    {
        // painelMenuInicial.SetActive(false);
        painelOpcoes.SetActive(true);
    }

    public void FecharModalNome()
    {
        painelMenuInicial.SetActive(true);
        modalNome.SetActive(false);
    }

    public void BotaoExportarRelatorioGeral()
    {
        PlayClick();

        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.ExportarRelatorioFormatado();
            Debug.Log("[Menu] Solicitação de exportação enviada ao StatsManager.");
        }
        else
        {
            Debug.LogError("[Menu] Não foi possível exportar: StatsManager não foi encontrado na cena!");
        }
    }

    private void SalvarNomeNoReport()
    {
        if (inputFieldNome == null || string.IsNullOrEmpty(inputFieldNome.text))
        {
            Debug.LogWarning("Input de nome vazio. Nome não foi salvo.");
            return;
        }

        string nomeDigitado = inputFieldNome.text;
        string caminhoArquivo = Path.Combine(Application.persistentDataPath, nomeArquivoJson);
        GameReport relatorioGeral = new GameReport();

        // 1. Se o arquivo já existe, carrega o histórico de todo mundo
        if (File.Exists(caminhoArquivo))
        {
            string jsonAntigo = File.ReadAllText(caminhoArquivo);
            relatorioGeral = JsonUtility.FromJson<GameReport>(jsonAntigo);
        }

        // 2. Define quem é o jogador ativo do momento
        relatorioGeral.currentPlayerName = nomeDigitado;

        // 3. Procura se esse aluno já existe na lista geral
        PlayerReport alunoExistente = relatorioGeral.players.Find(p => p.playerName == nomeDigitado);

        if (alunoExistente == null)
        {
            // Se é um aluno novo, cria o registro dele na lista
            PlayerReport novoAluno = new PlayerReport();
            novoAluno.playerName = nomeDigitado;
            relatorioGeral.players.Add(novoAluno);
            Debug.Log($"Novo aluno '{nomeDigitado}' cadastrado no sistema.");
        }

        // 4. Salva o arquivo atualizado de volta no disco
        string novoJson = JsonUtility.ToJson(relatorioGeral, true);
        File.WriteAllText(caminhoArquivo, novoJson);

        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.RecarregarDadosDoDisco();
            Debug.Log("[Menu] StatsManager notificado e atualizado com o aluno ativo.");
        }
    }

    public void AbrirModalNome()
    {
        painelMenuInicial.SetActive(false);
        modalNome.SetActive(true);
    }

    public void FecharOpcoes()
    {
        painelMenuInicial.SetActive(true);
        painelOpcoes.SetActive(false);
    }

    public void SairJogo()
    {
        Debug.Log("Sair do jogo");
        Application.Quit();
    }

    public void BotaoVoltarMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}