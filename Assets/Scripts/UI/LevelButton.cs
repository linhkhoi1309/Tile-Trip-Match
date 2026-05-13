using TMPro;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TMP_Text label;

    private int level;
    private LevelManager levelManager;

    public void Setup(int level, LevelManager manager)
    {
        this.level = level;
        this.levelManager = manager;
        label.text = level.ToString();
    }

    public void OnClick()
    {
        levelManager.StartLevel(level);
    }
}