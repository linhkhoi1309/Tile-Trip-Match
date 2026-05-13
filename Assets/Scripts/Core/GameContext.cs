public class GameContext
{
    public LevelSession LevelSession { get; private set; }

    public GameContext()
    {
        LevelSession = new LevelSession();
    }
}