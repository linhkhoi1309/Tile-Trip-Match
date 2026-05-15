using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayAgainButton : MonoBehaviour
{
    private GameResultOverlay overlay;
    private void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnClick);

        overlay = FindObjectOfType<GameResultOverlay>();
    }

    private void OnClick()
    {
        UIAudio.PlayTap();
        if (overlay != null)
            overlay.Hide();

        if (LevelManager.Instance != null)
            LevelManager.Instance.RestartCurrentLevel();
    }
}
