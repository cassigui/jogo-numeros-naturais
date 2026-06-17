using UnityEngine;
using System.Collections;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fase02 : MonoBehaviour
{
    [Header("Efeito Primeira Vida (Via Código)")]
    [Tooltip("Arraste o objeto 'PrimeiraVida' (o pai) para cá")]
    [SerializeField] private GameObject objetoPrimeiraVida;

    [Tooltip("Arraste o objeto 'single_heart' para cá")]
    [SerializeField] private RectTransform singleHeart;

    [Tooltip("Duração total da animação (Pulsar + Subir sumindo)")]
    [SerializeField] private float tempoEfeitoTotal = 1.2f;

    [Tooltip("Distância em pixels que o coração vai subir ao desaparecer")]
    [SerializeField] private float distanciaSubida = 150f;

    private UnityEngine.UI.Image imagemCoracao;
    private CanvasGroup canvasGroupPrimeiraVida;
    private Vector2 posicaoOriginalCoracao;

    [Header("Configurações de UI Geral")]
    public CanvasGroup canvasGroup;
    [Tooltip("Tempo de espera antes de começar a surgir a fase")]
    public float delay = 1.5f;
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
    [SerializeField] private string nomeDaProximaFase = "Fase03";

    [Tooltip("Marque esta caixinha APENAS na Fase 2 (Fase da Senha). Deixe desmarcada na Fase 1 (Pilhas).")]
    [SerializeField] private bool fasePorSenha = false;

    [Header("Sistema de Vidas")]
    [Tooltip("Coloque os Sprites em ordem: Posição 0 = 0 vidas, Posição 1 = 1 vida, até o máximo")]
    [SerializeField] private Sprite[] spritesVidas;
    private int vidasAtuais;
    private int vidasMaximas; // Adicionado para calcular a primeira morte

    [Header("Configurações de Áudio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip errorSound;
    [SerializeField] private AudioClip successSound;
    [Tooltip("Áudio que tocará exclusivamente na primeira perda de vida")]
    [SerializeField] private AudioClip somPrimeiraMorte;
    private StatsManager statsManager;


    void Start()
    {
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.RegisterLevelStart(SceneManager.GetActiveScene().name);
        }

        if (painelDoVideo != null) painelDoVideo.SetActive(false);
        if (modalGameOver != null) modalGameOver.SetActive(false);
        if (modalSucesso != null) modalSucesso.SetActive(false);

        // Inicialização do efeito da primeira vida
        if (objetoPrimeiraVida != null)
        {
            canvasGroupPrimeiraVida = objetoPrimeiraVida.GetComponent<CanvasGroup>();
            if (canvasGroupPrimeiraVida != null)
            {
                canvasGroupPrimeiraVida.alpha = 0f;
            }
            objetoPrimeiraVida.SetActive(false);
        }

        if (singleHeart != null)
        {
            imagemCoracao = singleHeart.GetComponent<UnityEngine.UI.Image>();
            posicaoOriginalCoracao = singleHeart.anchoredPosition;
            singleHeart.localScale = Vector3.one;
        }

        if (spritesVidas != null && spritesVidas.Length > 0)
        {
            vidasAtuais = spritesVidas.Length - 1;
            vidasMaximas = vidasAtuais; // Guarda o valor máximo de vidas
        }

        if (!fasePorSenha)
        {
            if (batterySlots == null || batterySlots.Length == 0)
            {
                batterySlots = Object.FindObjectsByType<SlotItem>(FindObjectsSortMode.None);
            }
        }

        if (meuBotaoPower == null) meuBotaoPower = Object.FindAnyObjectByType<PowerButton>();
        if (scriptDasVidas == null) scriptDasVidas = Object.FindAnyObjectByType<HeartAnimation>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        // Configuração inicial do Fade
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            StartCoroutine(FadeRoutine());
        }
    }

    // Usado na FASE 1 (Pilhas)
    public void checkSlots()
    {
        if (fasePorSenha)
        {
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
            ErrouCombinacao(); // Deixamos o controle do som de erro dentro do método unificado
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
        ErrouCombinacao(); // Deixamos o controle do som de erro dentro do método unificado
    }

    private void ErrouCombinacao()
    {
        if (vidasAtuais > 0)
        {
            // Se for a primeira perda de vida
            if (vidasAtuais == vidasMaximas)
            {
                TocarSom(somPrimeiraMorte, "Primeira Morte");

                if (objetoPrimeiraVida != null)
                {
                    objetoPrimeiraVida.SetActive(true);
                    StartCoroutine(AnimarTelaECoracaoSurgindo());
                }
            }
            else
            {
                // Sons normais de erro para as próximas vidas
                if (fasePorSenha) TocarSom(errorSound, "Erro Senha");
                else TocarSom(errorSound, "Erro Pilhas");
            }

            vidasAtuais--;

            StatsManager stats = Object.FindAnyObjectByType<StatsManager>();
            if (StatsManager.Instance != null)
            {
                StatsManager.Instance.RegisterError(SceneManager.GetActiveScene().name);
                Debug.Log("✅ Erro registrado para: " + SceneManager.GetActiveScene().name);
            }
            else
            {
                Debug.LogError("❌ StatsManager não encontrado!");
            }

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

    // Corrotina de Animação do Coração Replicada da Fase 03
    private IEnumerator AnimarTelaECoracaoSurgindo()
    {
        float tempoDecorrido = 0f;

        if (canvasGroupPrimeiraVida != null) canvasGroupPrimeiraVida.alpha = 0f;
        if (singleHeart != null)
        {
            singleHeart.anchoredPosition = posicaoOriginalCoracao;
            singleHeart.localScale = Vector3.one;
        }
        if (imagemCoracao != null)
        {
            Color c = imagemCoracao.color;
            c.a = 1f;
            imagemCoracao.color = c;
        }

        // PARTE 1: EXECUÇÃO DAS ANIMAÇÕES VISUAIS
        while (tempoDecorrido < tempoEfeitoTotal)
        {
            tempoDecorrido += Time.deltaTime;
            float progressoGlobal = tempoDecorrido / tempoEfeitoTotal;

            if (canvasGroupPrimeiraVida != null)
            {
                float progressoFade = Mathf.Clamp01(progressoGlobal / 0.3f);
                canvasGroupPrimeiraVida.alpha = Mathf.SmoothStep(0f, 1f, progressoFade);
            }

            if (singleHeart != null)
            {
                if (progressoGlobal <= 0.4f)
                {
                    float progressoPulso = progressoGlobal / 0.4f;
                    float escalaPulso = 1f + Mathf.Sin(progressoPulso * Mathf.PI) * 0.3f;

                    singleHeart.localScale = new Vector3(escalaPulso, escalaPulso, 1f);
                    singleHeart.anchoredPosition = posicaoOriginalCoracao;
                }
                else
                {
                    float progressoSumiço = (progressoGlobal - 0.4f) / 0.6f;
                    float curvaSubida = Mathf.SmoothStep(0f, 1f, progressoSumiço);

                    float novoY = posicaoOriginalCoracao.y + (curvaSubida * distanciaSubida);
                    singleHeart.anchoredPosition = new Vector2(posicaoOriginalCoracao.x, novoY);

                    float factorDesaparecer = Mathf.Clamp01(1f - progressoSumiço);
                    singleHeart.localScale = new Vector3(factorDesaparecer, factorDesaparecer, 1f);

                    if (imagemCoracao != null)
                    {
                        Color c = imagemCoracao.color;
                        c.a = factorDesaparecer;
                        imagemCoracao.color = c;
                    }
                }
            }

            yield return null;
        }

        if (singleHeart != null) singleHeart.localScale = Vector3.zero;

        // PARTE 2: ESPERA E FADE OUT DO MODAL (1 SEGUNDO)
        float tempoEsperaFechamento = 1.0f;
        float tempoDecorridoFechamento = 0f;

        while (tempoDecorridoFechamento < tempoEsperaFechamento)
        {
            tempoDecorridoFechamento += Time.deltaTime;
            float progressoFechamento = tempoDecorridoFechamento / tempoEsperaFechamento;

            if (canvasGroupPrimeiraVida != null)
            {
                canvasGroupPrimeiraVida.alpha = Mathf.Clamp01(1f - progressoFechamento);
            }

            yield return null;
        }

        // PARTE 3: FINALIZAÇÃO E LIMPEZA
        if (objetoPrimeiraVida != null)
        {
            objetoPrimeiraVida.SetActive(false);
        }

        if (singleHeart != null)
        {
            singleHeart.anchoredPosition = posicaoOriginalCoracao;
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
        if (StatsManager.Instance != null)
        {
            StatsManager.Instance.RegisterWin(SceneManager.GetActiveScene().name, vidasAtuais);
        }
        else
        {
            Debug.LogError("StatsManager não encontrado ao terminar a fase!");
        }

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
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}