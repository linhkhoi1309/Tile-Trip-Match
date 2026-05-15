using UnityEngine;

public class LevelSelectorUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private LevelButton buttonPrefab;
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private int totalLevels = 10;

    private LevelButton selectedButton;
    private int selectedLevel = -1;

    private void Start()
    {
        for (int i = 1; i <= totalLevels; i++)
        {
            var btn = Instantiate(buttonPrefab, container);
            btn.Setup(i, (b, lvl) => OnButtonSelected(b, lvl));
        }
    }

    private void OnButtonSelected(LevelButton button, int level)
    {
        if (selectedButton != null)
            selectedButton.SetSelected(false);

        selectedButton = button;
        selectedLevel = level;
        selectedButton.SetSelected(true);
    }

    // Wire this to the Play button's onClick in the inspector
    public void OnPlayButton()
    {
        if (selectedLevel <= 0) return;
        levelManager.StartLevel(selectedLevel);
    }
}