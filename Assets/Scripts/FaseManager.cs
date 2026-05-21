using UnityEngine;
using System.Collections;

public class FaseManager : MonoBehaviour
{
    [Header("Configurações de UI")]
    public CanvasGroup canvasGroup;
    public float duration = 0.5f;
    public float delay = 0.5f;

    void Start()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            StartCoroutine(FadeRoutine());
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