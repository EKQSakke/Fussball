using Godot;
using Nidot;

public partial class PlayerInput : Node
{
	Player player;
	Node2D mousePosition;
	MeshInstance3D rayMesh;
	Camera3D cam;
	MultiplayerSynchronizer sync;
	 
	bool isTargeting;

	bool hasAuthority;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetParent() as Player ?? throw new System.Exception("PlayerInput must be a child of Player");
		mousePosition = this.AddChildOfType<Node2D>();
		cam = this.GetNodeFromAll<Camera3D>();
		rayMesh = this.AddChildOfType<MeshInstance3D>();
		sync = player.GetChildOfType<MultiplayerSynchronizer>();
		hasAuthority = player.PlayerId == Multiplayer.GetUniqueId();
	}

	public override void _UnhandledInput(InputEvent @event)
	{

		if (@event.IsActionPressed("Fire"))
		{
			
			GD.Print($"{player.PlayerId} {Multiplayer.GetUniqueId()} {GetMultiplayerAuthority()} hasAuthority: {hasAuthority}");
			if (!hasAuthority) return;
			// Check if initial click is on top of the player
			isTargeting = ClickOnPlayer();
		}
		if (@event.IsActionReleased("Fire") && isTargeting)
		{
			isTargeting = false;
			var floorPos = GetFloorPosition();
			if (floorPos != null)
			{
				player.ApplyCentralImpulse((player.GlobalPosition - (Vector3)GetFloorPosition()) * 10);
			}
			rayMesh.Mesh = null;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        Target();
	}

    private void Target()
    {
		if (!isTargeting) return;


		DrawTools.DrawRay(rayMesh, player.GlobalPosition, (Vector3)GetFloorPosition());
    }

    Vector3? GetFloorPosition()
	{
		var screenPos = mousePosition.GetGlobalMousePosition();
        var worldPos = cam.ProjectPosition(screenPos, 1000);
        var spaceState = player.GetWorld3D().DirectSpaceState;
        var parameters = new PhysicsRayQueryParameters3D()
        {
            From = cam.GlobalPosition,
            To = worldPos,
        };
        var result = spaceState.IntersectRay(parameters);
        if (!result.TryGetValue("position", out var position))
            return null;

        return (Vector3) position + new Vector3(0, .1f, 0);
	}

	bool ClickOnPlayer()
	{
		var screenPos = mousePosition.GetGlobalMousePosition();
        var worldPos = cam.ProjectPosition(screenPos, 1000);
        var spaceState = player.GetWorld3D().DirectSpaceState;
        var parameters = new PhysicsRayQueryParameters3D()
        {
            From = cam.GlobalPosition,
            To = worldPos,
        };

        var result = spaceState.IntersectRay(parameters);
        if (!result.TryGetValue("rid", out var rid))
			return false;
			
		return (Rid)rid == player.GetRid();
	}
}