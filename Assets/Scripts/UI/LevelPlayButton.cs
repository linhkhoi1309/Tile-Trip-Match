using UnityEngine;

public class LevelPlayButton : MonoBehaviour
{
    [SerializeField] private LevelSelectorUI levelSelectorUI;

    public void OnClick()
    {
        UIAudio.PlayTap();

        if (levelSelectorUI != null)
            levelSelectorUI.OnPlayButton();
    }
}
