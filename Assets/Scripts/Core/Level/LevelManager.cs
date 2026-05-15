using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
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