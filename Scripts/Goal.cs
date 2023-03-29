using Godot;
using Nidot;

public partial class Goal : Area3D
{
    GameLevel level;
    [Export] int goalId;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        level = this.GetNodeFromAll<GameLevel>();
        BodyEntered += CheckForBall;
    }

    private void CheckForBall(Node3D body)
    {
        if (body is Ball ball)
        {
            level.GoalScored(goalId);
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

}
