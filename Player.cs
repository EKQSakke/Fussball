using Godot;
using Nidot;

public partial class Player : RigidBody3D
{
    [Export]
    public long PlayerId;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        this.AddChildOfType<PlayerInput>();
    }

    public void SetId(long id)
    {
        PlayerId = id;
        SetMultiplayerAuthority((int)id); 
    }

    
}
