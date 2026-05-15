using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameResultOverlay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private GameObject nextLevelButton;

    private void Awake()
    {
        Hide();
    }

    public void ShowWin()
    {
        this.gameObject.SetActive(true);

        if (messageText != null)
            messageText.text = "You Win!";

        if (nextLevelButton != null)
            nextLevelButton.SetActive(LevelManager.Instance != null && LevelManager.Instance.HasNextLevel());
    }

    public void ShowLose()
    {
        this.gameObject.SetActive(true);

        if (messageText != null)
            messageText.text = "You Lose!";

        if (nextLevelButton != null)
            nextLevelButton.SetActive(false);
    }

    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
}
