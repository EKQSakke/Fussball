using Godot;

public partial class LobbyPlayer : Node
{
    [Export] public long PlayerId { get; set; }

    [Export] public Color TeamColor { get; set; }
}
