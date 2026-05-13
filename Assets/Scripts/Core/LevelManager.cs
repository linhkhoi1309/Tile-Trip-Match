using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public void StartLevel(int level)
    {
        GameManager.Instance.Context.LevelSession.SetLevel(level);
        SceneManager.LoadScene(SceneNames.Gameplay);
    }
}