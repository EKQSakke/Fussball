using System.Collections.Generic;
using Godot;
public class PlayerTeam
{
    public int TeamId { get; set; } // Index of team (0, 1)
    public bool IsReady { get; set; }

    public List<Vector3> SpawnPoints { get; set; }
}