using Godot;
using Nidot;

public partial class Player :  RigidBody3D
{
	Node2D mousePosition;

	MeshInstance3D rayMesh;
	Camera3D cam;

	bool isTargeting;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		mousePosition = this.AddChildOfType<Node2D>();
		cam = this.GetNodeFromAll<Camera3D>();
		rayMesh = this.AddChildOfType<MeshInstance3D>();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("Fire"))
		{
			isTargeting = true;
			DrawTools.DrawRay(rayMesh, GlobalPosition, (Vector3)GetFloorPosition());
		}
		if (@event.IsActionReleased("Fire"))
		{
			isTargeting = false;
			var floorPos = GetFloorPosition();
			if (floorPos != null)
			{
				GD.Print(GlobalPosition);
				ApplyCentralImpulse((GlobalPosition - (Vector3)GetFloorPosition()) * 10);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	Vector3? GetFloorPosition()
	{
		var screenPos = mousePosition.GetGlobalMousePosition();
        var worldPos = cam.ProjectPosition(screenPos, 1000);
        var spaceState = GetWorld3D().DirectSpaceState;
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
}
