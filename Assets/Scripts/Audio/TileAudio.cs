using UnityEngine;

public class TileAudio : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 2f)]
    private float tapVolume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayTap()
    {
        AudioClip clip = AssetsLoader.AudioMapping != null ? AssetsLoader.AudioMapping.TileTap : null;

        if (clip == null || audioSource == null)
            return;

        audioSource.PlayOneShot(clip, tapVolume);
    }
}
