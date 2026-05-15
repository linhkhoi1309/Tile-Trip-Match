using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReturnButton : MonoBehaviour
{
    private void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        UIAudio.PlayTap();
        SceneManager.LoadScene(SceneNames.Home);
    }
}
