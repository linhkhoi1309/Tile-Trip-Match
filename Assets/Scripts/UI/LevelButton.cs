using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite selectedSprite;

    private int level;
    private Action<LevelButton,int> onSelected;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void Setup(int level, Action<LevelButton,int> onSelected)
    {
        this.level = level;
        this.onSelected = onSelected;
        label.text = level.ToString();
        SetSelected(false);
    }

    public void OnClick()
    {
        onSelected?.Invoke(this, level);
    }

    public void SetSelected(bool selected)
    {
        if (button == null || button.image == null) return;
        button.image.sprite = selected && selectedSprite != null ? selectedSprite : normalSprite;
    }
}