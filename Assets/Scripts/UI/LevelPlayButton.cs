using UnityEngine;

public class LevelPlayButton : MonoBehaviour
{
    [SerializeField] private LevelSelectorUI levelSelectorUI;

    public void OnClick()
    {
        if (levelSelectorUI != null)
            levelSelectorUI.OnPlayButton();
    }
}
