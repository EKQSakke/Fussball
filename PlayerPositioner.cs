using System.Threading.Tasks.Dataflow;
using System.Collections.Generic;
using Godot;

public class PlayerPositioner
{
    Dictionary<int, List<Vector3>> spawnPoints;

    public PlayerPositioner()
    {
        spawnPoints = new();
    }

    ///<summary>
    /// Gets first spawn point and puts it at the end of the list
    ///</summary>
    public Vector3 GetNextSpawnPointForTeam(int teamId)
    {
        var spawnPointsForTeam = spawnPoints[teamId];
        var spawnPoint = spawnPointsForTeam[0];
        spawnPointsForTeam.RemoveAt(0);
        spawnPointsForTeam.Add(spawnPoint);
        return spawnPoint + new Vector3(0, 1, 0);
    }

    public void AddSpawnPoints(List<SpawnPoint> spawnPoints)
    {
        GD.Print($"Added {spawnPoints.Count} spawn points");
        foreach (var spawnPoint in spawnPoints)
        {
            AddSpawnPoint(spawnPoint);
        }
    }

    void AddSpawnPoint(SpawnPoint spawnPoint) 
    {
        if (!spawnPoints.ContainsKey(spawnPoint.TeamId))
        {
            spawnPoints.Add(spawnPoint.TeamId, new());
        }

        spawnPoints[spawnPoint.TeamId].Add(spawnPoint.GlobalPosition);
    }
}

