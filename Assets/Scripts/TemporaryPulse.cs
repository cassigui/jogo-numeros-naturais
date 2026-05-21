using UnityEngine;
using System.Collections;

public class DestaqueTemporario : MonoBehaviour
{
    [Header("Configurações do Pulso")]
    [SerializeField] private float velocidade = 5.0f;
    [SerializeField] private float amplitude = 0.15f;
    [SerializeField] private float duracao = 3.0f; // Tempo em segundos

    private Vector3 escalaOriginal;
    private bool estaPulsando = false;

    void Start()
    {
        escalaOriginal = transform.localScale;
        AtivarDestaque();
    }

    public void AtivarDestaque()
    {
        StartCoroutine(RotinaDePulso());
    }

    private IEnumerator RotinaDePulso()
    {
        estaPulsando = true;
        float tempoDecorrido = 0f;

        while (tempoDecorrido < duracao)
        {
            // Calcula o pulso
            float pulso = Mathf.Sin(Time.time * velocidade) * amplitude;
            transform.localScale = escalaOriginal + new Vector3(pulso, pulso, pulso);

            tempoDecorrido += Time.deltaTime;
            yield return null; // Espera o próximo frame
        }

        // Garante que o item volte ao tamanho exato original ao terminar
        estaPulsando = false;
        transform.localScale = escalaOriginal;
    }
}