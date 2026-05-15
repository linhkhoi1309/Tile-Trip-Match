using UnityEngine;

public class CustomCursorManager : MonoBehaviour
{
    private static CustomCursorManager instance;

    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Vector2 hotspot = Vector2.zero;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;
    [SerializeField] private bool hideSystemCursor = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        ApplyCursor();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
            ApplyCursor();
    }

    public void ApplyCursor()
    {
        if (cursorTexture != null)
            Cursor.SetCursor(cursorTexture, hotspot, cursorMode);

        Cursor.visible = !hideSystemCursor;
    }
}
