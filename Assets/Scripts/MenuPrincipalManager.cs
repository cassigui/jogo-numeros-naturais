using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // <-- Importante para usar o InputField
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
        painelMenuInicial.SetActive(false);
        painelOpcoes.SetActive(true);
    }

    public void FecharModalNome()
    {
        painelMenuInicial.SetActive(true);
        modalNome.SetActive(false);
    }

    private void SalvarNomeNoReport()
    {
        if (inputFieldNome == null || string.IsNullOrEmpty(inputFieldNome.text))
        {
            Debug.LogWarning("Input de nome vazio ou não configurado. Nome não foi salvo.");
            return;
        }

        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.AtualizarNomeDoJogador(inputFieldNome.text);
            Debug.Log($"Nome '{inputFieldNome.text}' enviado para o StatsManager!");
        }
        else
        {
            string caminhoArquivo = Path.Combine(Application.persistentDataPath, nomeArquivoJson);
            GameReport relatorioAtual = new GameReport();

            if (File.Exists(caminhoArquivo))
            {
                string jsonAntigo = File.ReadAllText(caminhoArquivo);
                relatorioAtual = JsonUtility.FromJson<GameReport>(jsonAntigo);
            }

            relatorioAtual.playerName = inputFieldNome.text;
            File.WriteAllText(caminhoArquivo, JsonUtility.ToJson(relatorioAtual, true));
            Debug.Log($"Nome '{inputFieldNome.text}' salvo diretamente no arquivo via menu.");
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