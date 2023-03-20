internal class GameSettings
{
    public int RoundLimit { get; set; }

    public int GoalLimit { get; set; }

    // Default
    public GameSettings()
    {
        SetDefaults();
    }

    public GameSettings(Game game)
    {
        switch (game)
        {
            case Game.Default:
                SetDefaults();
                break;
            case Game.Short:
                RoundLimit = 10;
                GoalLimit = 1;
                break;
            case Game.Long:
                RoundLimit = 100;
                GoalLimit = 7;
                break;
        }
    }

    public void SetDefaults()
    {
        RoundLimit = 50;
        GoalLimit = 3;
    }
}
