using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HeartAnimation : MonoBehaviour
{
    [Header("Configurações de Componentes")]
    [SerializeField] private RectTransform fechaduraRect; 
    [SerializeField] private Image imagemDosCoracoes; 

    [Header("Configurações de Tempo")]
    [SerializeField] private float tempoEsperaParaAparecer = 2.0f; 
    [SerializeField] private float tempoDeslizarEntrada = 1.0f;  

    private Vector2 posicaoInicialCentro;
    private Vector2 posicaoEscondidaEsquerda;

    void OnEnable()
    {
        if (fechaduraRect != null)
        {
            posicaoInicialCentro = fechaduraRect.anchoredPosition;
            float larguraImagem = fechaduraRect.rect.width;
            posicaoEscondidaEsquerda = new Vector2(-Screen.width - larguraImagem, posicaoInicialCentro.y);
            fechaduraRect.anchoredPosition = posicaoEscondidaEsquerda;

            StartCoroutine(SequenciaAnimacao());
        }
    }

    private IEnumerator SequenciaAnimacao()
    {
        yield return new WaitForSeconds(tempoEsperaParaAparecer);

        float tempoPassado = 0f;
        while (tempoPassado < tempoDeslizarEntrada)
        {
            tempoPassado += Time.deltaTime;
            float progresso = tempoPassado / tempoDeslizarEntrada;
            float progressoSuave = Mathf.SmoothStep(0f, 1f, progresso); 

            fechaduraRect.anchoredPosition = Vector2.Lerp(posicaoEscondidaEsquerda, posicaoInicialCentro, progressoSuave);
            yield return null; 
        }
        fechaduraRect.anchoredPosition = posicaoInicialCentro; 
    }

    // NOVA FUNÇÃO: Altera a imagem dos corações dinamicamente
    public void AtualizarSpriteVidas(Sprite novoSprite)
    {
        if (imagemDosCoracoes != null && novoSprite != null)
        {
            imagemDosCoracoes.sprite = novoSprite;
        }
    }

    public void EsconderCoracoes()
    {
        StopAllCoroutines(); 
        gameObject.SetActive(false);
    }
}