using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AssetsLoader
{
    public static TileSpriteMappingSO TileMapping { get; private set; }

    public static AudioMappingSO AudioMapping { get; private set; }

    public static AudioClip BackgroundMusic { get; private set; }

    private static AsyncOperationHandle<TileSpriteMappingSO> handle;

    private static AsyncOperationHandle<AudioMappingSO> audioHandle;

    public static async Task<float> LoadAsync()
    {
        handle = Addressables.LoadAssetAsync<TileSpriteMappingSO>("TileSpriteMapping");

        while (!handle.IsDone)
        {
            await Task.Yield();
        }

        TileMapping = handle.Result;

        audioHandle = Addressables.LoadAssetAsync<AudioMappingSO>("AudioMapping");

        while (!audioHandle.IsDone)
        {
            await Task.Yield();
        }

        AudioMapping = audioHandle.Result;
        BackgroundMusic = AudioMapping != null ? AudioMapping.BackgroundMusic : null;
        return 1f;
    }

    public static void Release()
    {
        if (handle.IsValid())
            Addressables.Release(handle);

        if (audioHandle.IsValid())
            Addressables.Release(audioHandle);

        TileMapping = null;
        AudioMapping = null;
        BackgroundMusic = null;
    }
}