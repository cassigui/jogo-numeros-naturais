using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;

public class HelperManager : MonoBehaviour
{
    // [SerializeField] private int levelGame;
    [SerializeField] private GameObject level;
    [SerializeField] private GameObject optionsModal;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;

    [SerializeField] private VideoPlayer tutorialVideoPlayer; 
    [SerializeField] private GameObject videoCanvasOuObjeto;
    [SerializeField] private Button botaoReiniciar;

    public void PlayHover()
    {
        if (audioSource != null && hoverSound != null)
            audioSource.PlayOneShot(hoverSound);
    }

    public void PlayClick()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }

    public void OpenModal()
    {
        level.SetActive(false);
        optionsModal.SetActive(true);
        IniciarVideoTutorial();
    }

    public void CloseModal()
    {
        if (tutorialVideoPlayer != null)
            tutorialVideoPlayer.Stop();

        if (videoCanvasOuObjeto != null)
            videoCanvasOuObjeto.SetActive(false);

        if (botaoReiniciar != null)
            botaoReiniciar.gameObject.SetActive(false);

        level.SetActive(true);
        optionsModal.SetActive(false);
    }

    private void IniciarVideoTutorial()
    {
        if (botaoReiniciar != null)
            botaoReiniciar.gameObject.SetActive(false);

        if (tutorialVideoPlayer != null)
        {
            tutorialVideoPlayer.frame = 0;
            if (videoCanvasOuObjeto != null)
                videoCanvasOuObjeto.SetActive(true);

            tutorialVideoPlayer.Play();
            
            tutorialVideoPlayer.loopPointReached -= AoTerminarOVideo;
            tutorialVideoPlayer.loopPointReached += AoTerminarOVideo;
        }
        else
        {
            level.SetActive(true);
        }
    }

    private void AoTerminarOVideo(VideoPlayer source)
    {
        tutorialVideoPlayer.loopPointReached -= AoTerminarOVideo;

        if (botaoReiniciar != null)
            botaoReiniciar.gameObject.SetActive(true);
    }

    public void ReiniciarVideo()
    {
        PlayClick();

        if (botaoReiniciar != null)
            botaoReiniciar.gameObject.SetActive(false);

        if (tutorialVideoPlayer != null)
        {
            tutorialVideoPlayer.frame = 0; 
            tutorialVideoPlayer.Play();
            
            tutorialVideoPlayer.loopPointReached -= AoTerminarOVideo;
            tutorialVideoPlayer.loopPointReached += AoTerminarOVideo;
        }
    }
}