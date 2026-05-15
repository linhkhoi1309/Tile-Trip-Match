using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI percentText;

    public void SetProgress(float value)
    {
        progressBar.value = value;
        percentText.text = $"{Mathf.RoundToInt(value * 100)}%";
    }
}