using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelperManager : MonoBehaviour
{
    // [SerializeField] private int levelGame;
    [SerializeField] private GameObject level;
    [SerializeField] private GameObject optionsModal;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioClip clickSound;

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
    }
    public void CloseModal()
    {
        level.SetActive(true);
        optionsModal.SetActive(false);
    }

}
