using System.Collections.Generic;
using Godot;
using Nidot;

public partial class GameLevel : Node
{

    [Export] PackedScene player;
    Node networked;

    Dictionary<int, long> playerTeamIds = new();
    PlayerPositioner playerPositioner = new();


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var lobby = this.GetNodeFromAll<Lobby>();
        networked = this.GetNodeFromChildren<MultiplayerSpawner>();
        playerPositioner.AddSpawnPoints(GetNode("SpawnPoints").GetNodesOfType<SpawnPoint>());

        if (Multiplayer.IsServer())
        {
            // Server team
            SpawnTeam(1);
            foreach (var item in Multiplayer.GetPeers())
            {
                SpawnTeam(item);
            }
        }
        else
        {
            lobby.SetGameMenuVisible(false);
        }
    }


    public void SpawnTeam(long id)
    {
        GD.Print($"SpawnTeam {id}");

        var teamId = playerTeamIds.Count;
        playerTeamIds.Add(playerTeamIds.Count, id);

        for (int i = 0; i < Globals.PlayerPerTeamCount; i++)
        {
            var newPlayerNode = player.Instantiate();
            var playerNode = newPlayerNode as Player ?? throw new System.Exception("playerNode must be a Player");
            playerNode.PlayerId = id;
            networked.AddChild(newPlayerNode, true);
            playerNode.GlobalPosition = playerPositioner.GetNextSpawnPointForTeam(teamId);
        }
    }
}
