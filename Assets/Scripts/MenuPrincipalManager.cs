using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    [SerializeField] private string nomeDoLevelDeJogo;
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;
    
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
        SceneManager.LoadScene(nomeDoLevelDeJogo);
    }
    
    public void AbrirOpcoes()
    {
        painelMenuInicial.SetActive(false);
        painelOpcoes.SetActive(true);
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
}