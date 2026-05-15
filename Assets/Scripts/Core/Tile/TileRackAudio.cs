using UnityEngine;

public class TileRackAudio : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 2f)]
    private float matchClipVolume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayMatchSound()
    {
        AudioClip clip = AssetsLoader.AudioMapping != null ? AssetsLoader.AudioMapping.TileMatch : null;

        if (clip == null || audioSource == null)
            return;

        audioSource.PlayOneShot(clip, matchClipVolume);
    }
}
