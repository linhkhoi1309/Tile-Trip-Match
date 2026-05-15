public class GameContext
{
    public LevelSession LevelSession { get; private set; }

    public LevelDataSO CurrentLevelData { get; set; }

    public GameContext()
    {
        LevelSession = new LevelSession();
    }
}