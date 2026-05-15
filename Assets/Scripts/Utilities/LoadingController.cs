using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour
{
    [SerializeField] private LoadingUI loadingUI;

    private async void Start()
    {
        await RunLoading();
    }

    private async Task RunLoading()
    {
        loadingUI.SetProgress(0f);

        // STEP 1: Load Addressables
        float assetProgress = await AssetsLoader.LoadAsync();
        loadingUI.SetProgress(0.5f);

        // STEP 2: Load Scene
        await LoadScene("HomeScene");
        loadingUI.SetProgress(1f);
    }

    private async Task LoadScene(string sceneName)
    {
        var op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            float normalized = op.progress / 0.9f; // IMPORTANT FIX
            loadingUI.SetProgress(0.5f + normalized * 0.5f);

            await Task.Yield();
        }

        loadingUI.SetProgress(1f);

        op.allowSceneActivation = true;

        while (!op.isDone)
            await Task.Yield();
    }
}