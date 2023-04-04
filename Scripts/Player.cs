using Godot;
using Nidot;

public partial class Player : RigidBody3D
{
    [Export] public long PlayerId;
    [Export] public Vector3 LaunchCommand;
    
    public int teamId;

    Label idLabel;
    Camera3D cam;
    Lobby lobby;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        lobby = this.GetNodeFromAll<Lobby>();
        this.AddChildOfType<PlayerInput>();
        idLabel = this.AddChildOfType<Label>();
        idLabel.Text = PlayerId.ToString();
        cam = this.GetNodeFromAll<Camera3D>();
        GD.Print($"color: {PlayerId} {lobby.GetTeamColor(PlayerId)}");
        SetColorToMesh(lobby.GetTeamColor(PlayerId));
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

    public void SetColorToMesh(Color color)
    {
        var mesh = this.GetNode<MeshInstance3D>("CollisionShape3D/PlayerMesh");
        if (mesh != null)
        {
            var material = mesh.Mesh.SurfaceGetMaterial(0).Duplicate() as Material;
            material.Set("albedo_color", color);
            mesh.Mesh.SurfaceSetMaterial(0, material);
        }
    }

}
