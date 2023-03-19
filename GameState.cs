public partial class GameLevel
{
    public enum GameState {
        Act, // Launch players at start, can't input commands
        Command // Can give commands, game starts here
    }
}
