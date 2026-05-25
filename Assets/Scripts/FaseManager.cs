using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FaseManager : MonoBehaviour
{
    [Header("Configurações de UI")]
    public CanvasGroup canvasGroup;
    public float duration = 0.5f;
    public float delay = 0.5f;
    [SerializeField] private SlotItem[] batterySlots;
    [SerializeField] private PowerButton meuBotaoPower;

    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private GameObject painelDoVideo;
    [SerializeField] private HeartAnimation scriptDasVidas; 

    [Header("Modais de Fim de Jogo")]
    [Tooltip("Arraste aqui o Painel/Modal de Game Over da sua UI")]
    [SerializeField] private GameObject modalGameOver;
    [Tooltip("Arraste aqui o Painel/Modal de Sucesso/Vitória da sua UI")]
    [SerializeField] private GameObject modalSucesso;

    [Header("Sistema de Vidas")]
    [Tooltip("Coloque os Sprites em ordem: Posição 0 = 0 vidas, Posição 1 = 1 vida, até o máximo")]
    [SerializeField] private Sprite[] spritesVidas; 
    private int vidasAtuais;

    [Header("Configurações de Áudio")]
    [SerializeField] private AudioSource audioSource; 
    [SerializeField] private AudioClip errorSound;    
    [SerializeField] private AudioClip successSound;  

    void Start()
    {
        if (painelDoVideo != null) painelDoVideo.SetActive(false);
        if (modalGameOver != null) modalGameOver.SetActive(false);
        
        // Garante que o modal de sucesso comece DESATIVADO
        if (modalSucesso != null) modalSucesso.SetActive(false);

        if (spritesVidas != null && spritesVidas.Length > 0)
        {
            vidasAtuais = spritesVidas.Length - 1; 
        }

        if (batterySlots == null || batterySlots.Length == 0)
        {
            batterySlots = Object.FindObjectsByType<SlotItem>(FindObjectsSortMode.None);
        }

        if (meuBotaoPower == null)
        {
            meuBotaoPower = Object.FindAnyObjectByType<PowerButton>();
        }

        if (scriptDasVidas == null)
        {
            scriptDasVidas = Object.FindAnyObjectByType<HeartAnimation>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            StartCoroutine(FadeRoutine());
        }
    }

    public void checkSlots()
    {
        int batteriesFound = 0;
        foreach (SlotItem slot in batterySlots)
        {
            if (slot != null)
            {
                if (slot.isOcupped)
                {
                    batteriesFound++;
                }
            }
            else
            {
                Debug.LogError("Existe um slot vazio (Null) na lista do FaseManager no Inspector!");
            }
        }

        if (batteriesFound == 4)
        {   
            Debug.LogError("TESTE");
            TocarSom(successSound, "Sucesso");

            if (meuBotaoPower != null)
            {
                meuBotaoPower.LigarBotao();
                StartCoroutine(PlayVideoRoutine());
            }
        }
        else
        {
            Debug.LogWarning($"⚠️ Ação negada. Você colocou apenas {batteriesFound} de 4 pilhas necessárias.");
            TocarSom(errorSound, "Erro");

            ErrouCombinacao();
        }
    }

    private void ErrouCombinacao()
    {
        if (vidasAtuais > 0)
        {
            vidasAtuais--; 

            if (scriptDasVidas != null && spritesVidas != null && vidasAtuais < spritesVidas.Length)
            {
                scriptDasVidas.AtualizarSpriteVidas(spritesVidas[vidasAtuais]);
            }

            if (vidasAtuais <= 0)
            {
                GameOver();
            }
        }
    }

    private void GameOver()
    {
        Debug.LogError("💀 GAME OVER! Ativando modal de pergunta.");
        
        if (modalGameOver != null)
        {
            modalGameOver.SetActive(true);
            RectTransform modalRect = modalGameOver.GetComponent<RectTransform>();
            if (modalRect != null) modalRect.SetAsLastSibling();
        }
        else
        {
            BotaoReiniciarFase();
        }
    }

    public void BotaoReiniciarFase()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BotaoVoltarMenu()
    {
        SceneManager.LoadScene("Menu"); 
    }

    public void BotaoProximaFase(string nomeDaProximaCena)
    {
        SceneManager.LoadScene(nomeDaProximaCena);
    }

    private void TocarSom(AudioClip clip, string nomeDoEfeito)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    IEnumerator PlayVideoRoutine()
    {
        if (scriptDasVidas != null)
        {
            scriptDasVidas.EsconderCoracoes();
        }

        yield return new WaitForSeconds(1f);

        if (videoPlayer != null && painelDoVideo != null)
        {
            painelDoVideo.SetActive(true); 
            videoPlayer.Play();   

            videoPlayer.loopPointReached -= AoTerminarOVideoDeVitoria;
            videoPlayer.loopPointReached += AoTerminarOVideoDeVitoria;
        }
    }

    private void AoTerminarOVideoDeVitoria(VideoPlayer source)
    {
        videoPlayer.loopPointReached -= AoTerminarOVideoDeVitoria;
        if (painelDoVideo != null) painelDoVideo.SetActive(false);
        if (modalSucesso != null)
        {
            modalSucesso.SetActive(true);
            
            RectTransform modalRect = modalSucesso.GetComponent<RectTransform>();
            if (modalRect != null) modalRect.SetAsLastSibling();
        }
        else
        {
            BotaoVoltarMenu();
        }
    }

    IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(delay);
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }
}