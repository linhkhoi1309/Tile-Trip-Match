public class LevelSession
{
    public int SelectedLevel { get; private set; } = 1;

    public void SetLevel(int level)
    {
        SelectedLevel = level;
    }

    public void Reset()
    {
        SelectedLevel = 1;
    }
}