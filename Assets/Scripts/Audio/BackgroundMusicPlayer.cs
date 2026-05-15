using System.Threading.Tasks;
using UnityEngine;

public class BackgroundMusicPlayer : MonoBehaviour
{
    [SerializeField] private bool playOnStart = true;
    [SerializeField, Range(0f, 1f)] private float volume = 1f;

    private AudioSource audioSource;
    private static BackgroundMusicPlayer instance;

    private async void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.volume = volume;

        // Wait until AssetsLoader loads the background music
        while (AssetsLoader.BackgroundMusic == null)
        {
            await Task.Yield();
        }

        audioSource.clip = AssetsLoader.BackgroundMusic;
        if (playOnStart) audioSource.Play();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Play()
    {
        if (audioSource == null) return;
        if (audioSource.clip != null && !audioSource.isPlaying) audioSource.Play();
    }

    public void Stop()
    {
        audioSource?.Stop();
    }

    public void SetVolume(float v)
    {
        if (audioSource == null) return;
        audioSource.volume = Mathf.Clamp01(v);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            audioSource?.Stop();
            instance = null;
        }
    }
}
