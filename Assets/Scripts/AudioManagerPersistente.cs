using UnityEngine;

public class AudioManagerPersistente : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
