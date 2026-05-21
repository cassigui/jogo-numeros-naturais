using UnityEngine;

public class PulsarItem : MonoBehaviour
{
    [SerializeField] private float velocidade = 2.0f;
    [SerializeField] private float amplitude = 0.1f;

    private Vector3 escalaOriginal;

    void Start()
    {
        escalaOriginal = transform.localScale;
    }

    void Update()
    {
        float pulso = Mathf.Sin(Time.time * velocidade) * amplitude;
        transform.localScale = escalaOriginal + new Vector3(pulso, pulso, pulso);
    }
}