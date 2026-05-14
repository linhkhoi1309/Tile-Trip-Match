using UnityEngine;

public class TileAudio : MonoBehaviour
{
    [SerializeField]
    private AudioClip tapClip;

    [SerializeField]
    [Range(0f, 2f)]
    private float tapVolume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayTap()
    {
        if (tapClip == null || audioSource == null)
            return;

        audioSource.PlayOneShot(tapClip, tapVolume);
    }
}
