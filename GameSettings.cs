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
                return;
        }
    }

    public void SetDefaults()
    {
        RoundLimit = 50;
        GoalLimit = 3;
    }
}
