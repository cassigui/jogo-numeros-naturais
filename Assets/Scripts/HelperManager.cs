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
            // =========================================================================
            // CORREÇÃO CRÍTICA PARA BUILD DO WINDOWS (Media Foundation Engine Fix)
            // =========================================================================

            // 1. Força o reprodutor do Windows a parar qualquer requisição fantasma em segundo plano
            tutorialVideoPlayer.Stop();

            // 2. Desvincula temporariamente a textura de renderização para limpar o buffer do DirectX
            RenderTexture texturaTemporaria = tutorialVideoPlayer.targetTexture;
            tutorialVideoPlayer.targetTexture = null;
            tutorialVideoPlayer.targetTexture = texturaTemporaria;

            // 3. Recarrega o clipe diretamente da memória da Build
            VideoClip clipeDesejado = tutorialVideoPlayer.clip;
            tutorialVideoPlayer.clip = null;
            tutorialVideoPlayer.clip = clipeDesejado;

            // =========================================================================

            // Limpa e reinscreve os eventos assíncronos
            tutorialVideoPlayer.loopPointReached -= AoTerminarOVideo;
            tutorialVideoPlayer.prepareCompleted -= VideoPreparadoProntoParaRodar;

            tutorialVideoPlayer.loopPointReached += AoTerminarOVideo;
            tutorialVideoPlayer.prepareCompleted += VideoPreparadoProntoParaRodar;

            // Oculta o objeto visual enquanto o Windows recria o buffer do vídeo
            if (videoCanvasOuObjeto != null)
                videoCanvasOuObjeto.SetActive(false);

            // Diz para o Windows: "Aloque a memória para este novo arquivo agora"
            tutorialVideoPlayer.Prepare();
        }
        else
        {
            level.SetActive(true);
        }
    }

    private void VideoPreparadoProntoParaRodar(VideoPlayer source)
    {
        // Remove o evento para evitar que seja chamado repetidamente na mesma execução
        tutorialVideoPlayer.prepareCompleted -= VideoPreparadoProntoParaRodar;

        // Ativa o objeto visual agora que o Windows carregou o arquivo na memória
        if (videoCanvasOuObjeto != null)
            videoCanvasOuObjeto.SetActive(true);

        // Dá o play definitivo
        tutorialVideoPlayer.Play();
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