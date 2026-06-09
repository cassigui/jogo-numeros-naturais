using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fase02 : MonoBehaviour
{
    [Header("Configurações de UI")]
    public CanvasGroup canvasGroup;
    [Tooltip("Tempo de espera antes de começar a surgir a fase")]
    public float delay = 1.5f; // Aumentei o padrão para dar mais tempo, mude no Inspector se quiser
    [Tooltip("Duração do efeito de surgimento (Fade In)")]
    public float duration = 1.0f; 
    
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

    [Header("Configurações de Fluxo")]
    [Tooltip("Digite o NOME EXATO da próxima cena/fase para onde o jogador vai ao vencer")]
    [SerializeField] private string nomeDaProximaFase = "Fase_03";

    [Tooltip("Marque esta caixinha APENAS na Fase 2 (Fase da Senha). Deixe desmarcada na Fase 1 (Pilhas).")]
    [SerializeField] private bool fasePorSenha = false;

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
        if (modalSucesso != null) modalSucesso.SetActive(false);

        if (spritesVidas != null && spritesVidas.Length > 0)
        {
            vidasAtuais = spritesVidas.Length - 1; 
        }

        if (!fasePorSenha)
        {
            if (batterySlots == null || batterySlots.Length == 0)
            {
                batterySlots = Object.FindObjectsByType<SlotItem>(FindObjectsSortMode.None);
            }
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

        // Configuração inicial do Fade
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false; // Bloqueia cliques enquanto surge
            canvasGroup.blocksRaycasts = false;
            StartCoroutine(FadeRoutine());
        }
    }

    // Usado na FASE 1 (Pilhas)
    public void checkSlots()
    {
        if (fasePorSenha)
        {
            TocarSom(errorSound, "Erro Senha");
            ErrouCombinacao();
            return;
        }

        int batteriesFound = 0;
        foreach (SlotItem slot in batterySlots)
        {
            if (slot != null && slot.isOcupped)
            {
                batteriesFound++;
            }
        }

        if (batteriesFound == 4)
        {   
            TocarSom(successSound, "Sucesso");
            if (meuBotaoPower != null) meuBotaoPower.LigarBotao();
            StartCoroutine(PlayVideoRoutine());
        }
        else
        {
            Debug.LogWarning($"⚠️ Ação negada. Você colocou apenas {batteriesFound} de 4 pilhas necessárias.");
            TocarSom(errorSound, "Erro Pilhas");
            ErrouCombinacao();
        }
    }

    // Usado na FASE 2 (Teclado/Senha)
    public void SenhaCorretaAcionarVitoria()
    {
        TocarSom(successSound, "Sucesso Senha");
        if (meuBotaoPower != null) meuBotaoPower.LigarBotao();
        StartCoroutine(PlayVideoRoutine());
    }

    // Usado na FASE 2 (Teclado/Senha)
    public void SenhaIncorretaErrou()
    {
        TocarSom(errorSound, "Erro Senha");
        ErrouCombinacao();
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

    public void BotaoProximaFase()
    {
        SceneManager.LoadScene(nomeDaProximaFase);
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
        canvasGroup.interactable = true; // Ativa os botões/interações após o término do fade
        canvasGroup.blocksRaycasts = true;
    }
}