using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField] private List<LevelDataSO> levels = new();

    public void StartLevel(int level)
    {
        LevelDataSO levelData = GetLevelData(level);

        if (levelData == null)
            return;

        GameManager.Instance.Context.LevelSession.SetLevel(level);
        GameManager.Instance.Context.CurrentLevelData = levelData;
        SceneManager.LoadScene(SceneNames.Gameplay);
    }

    public void RestartCurrentLevel()
    {
        StartLevel(CurrentLevel);
    }

    public void StartNextLevel()
    {
        StartLevel(CurrentLevel + 1);
    }

    public bool HasNextLevel()
    {
        return GetLevelData(CurrentLevel + 1) != null;
    }

    public int CurrentLevel =>
        GameManager.Instance != null &&
        GameManager.Instance.Context != null &&
        GameManager.Instance.Context.LevelSession != null
            ? GameManager.Instance.Context.LevelSession.SelectedLevel
            : 1;

    private LevelDataSO GetLevelData(int level)
    {
        int index = level - 1;

        if (index < 0 || index >= levels.Count)
        {
            Debug.LogError($"No LevelData assigned for level {level}");
            return null;
        }

        return levels[index];
    }
}