using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AssetsLoader
{
    public static TileSpriteMappingSO TileMapping { get; private set; }

    private static AsyncOperationHandle<TileSpriteMappingSO> handle;

    public static async Task<float> LoadAsync()
    {
        handle = Addressables.LoadAssetAsync<TileSpriteMappingSO>("TileSpriteMapping");

        while (!handle.IsDone)
        {
            await Task.Yield();
        }

        TileMapping = handle.Result;
        return 1f;
    }

    public static void Release()
    {
        if (handle.IsValid())
            Addressables.Release(handle);
    }
}