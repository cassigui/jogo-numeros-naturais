using System.Collections;
using UnityEngine;

public class AnimarFechadura : MonoBehaviour
{
    [Header("Configurações de Componentes")]
    [SerializeField] private RectTransform fechaduraRect; 

    [Header("Configurações de Tempo")]
    [SerializeField] private float tempoDeslizarEntrada = 1.0f;  
    [SerializeField] private float tempoParadaNoCentro = 3.0f; 
    [SerializeField] private float tempoDeslizarSaida = 1.0f;   

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

            // Inicia a sequência da animação
            StartCoroutine(SequenciaAnimacao());
        }
    }

    private IEnumerator SequenciaAnimacao()
    {
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

        yield return new WaitForSeconds(tempoParadaNoCentro);

        tempoPassado = 0f;
        while (tempoPassado < tempoDeslizarSaida)
        {
            tempoPassado += Time.deltaTime;
            float progresso = tempoPassado / tempoDeslizarSaida;
            float progressoSuave = Mathf.SmoothStep(0f, 1f, progresso);

            fechaduraRect.anchoredPosition = Vector2.Lerp(posicaoInicialCentro, posicaoEscondidaEsquerda, progressoSuave);
            yield return null;
        }
        fechaduraRect.anchoredPosition = posicaoEscondidaEsquerda;

        gameObject.SetActive(false);
    }
}