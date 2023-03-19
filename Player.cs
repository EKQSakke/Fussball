using Godot;
using Nidot;

public partial class Player : RigidBody3D
{
    [Export]
    public long PlayerId;

    Label idLabel;
    Camera3D cam;

    [Export]
    public Vector3 LaunchCommand;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.AddChildOfType<PlayerInput>();
        idLabel = this.AddChildOfType<Label>();
        idLabel.Text = PlayerId.ToString();
        cam = this.GetNodeFromAll<Camera3D>();
    }

    public void SetId(long id)
    {
        PlayerId = id;
    }

    public override void _Process(double delta)
    {
        idLabel.GlobalPosition = cam.UnprojectPosition(GlobalPosition);
    }

    public void ApplyCommand()
    {
        ApplyCentralImpulse(LaunchCommand);
        LaunchCommand = new Vector3();
    }

}
