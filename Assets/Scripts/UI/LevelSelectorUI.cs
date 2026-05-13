using UnityEngine;

public class LevelSelectorUI : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private LevelButton buttonPrefab;
    [SerializeField] private LevelManager levelManager;

    private void Start()
    {
        for (int i = 1; i <= 10; i++)
        {
            var btn = Instantiate(buttonPrefab, container);
            btn.Setup(i, levelManager);
        }
    }
}