public partial class GameLevel
{
    public enum GameState {
        Act, // Launch players at start, can't input commands
        Command, // Can give commands, game starts here
        Goal, // State after goal, timer to wait until level resets, or game ends
    }
}
