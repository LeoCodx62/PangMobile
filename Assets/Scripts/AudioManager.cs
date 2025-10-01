using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance { get; private set; }

    [SerializeField]
    private AudioSource _AS_SoundEffects;


    void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Evita duplicati
            return;
        }

        Instance = this;

    }

    public void PlaySEClip(AudioClip clip)
    {
        _AS_SoundEffects.clip = clip;
        _AS_SoundEffects.Play();
    }
}
