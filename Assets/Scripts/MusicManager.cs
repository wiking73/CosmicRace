using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    public AudioSource musicSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource = GetComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.Play();
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public bool IsMuted()
    {
        return musicSource.mute;
    }
}
