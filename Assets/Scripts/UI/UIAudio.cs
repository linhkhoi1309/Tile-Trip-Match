using UnityEngine;

public static class UIAudio
{
    private static GameObject audioHost;
    private static AudioSource audioSource;

    private static void EnsureAudioSource()
    {
        if (audioSource != null)
            return;

        audioHost = new GameObject("UIAudioHost");
        Object.DontDestroyOnLoad(audioHost);
        audioSource = audioHost.AddComponent<AudioSource>();
    }

    public static void PlayTap()
    {
        EnsureAudioSource();

        AudioClip clip = AssetsLoader.AudioMapping != null ? AssetsLoader.AudioMapping.TileTap : null;

        if (clip == null || audioSource == null)
            return;

        audioSource.PlayOneShot(clip);
    }
}
