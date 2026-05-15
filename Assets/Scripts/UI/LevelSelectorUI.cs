using UnityEngine;

public class LevelSelectorUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private LevelButton buttonPrefab;
    [SerializeField] private LevelManager levelManager;
    private int totalLevels = 10;

    private void Start()
    {
        for (int i = 1; i <= totalLevels; i++)
        {
            var btn = Instantiate(buttonPrefab, container);
            btn.Setup(i, levelManager);
        }
    }
}