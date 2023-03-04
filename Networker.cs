using System.Collections.Generic;
using System.Linq;
using Godot;
using Nidot;

public partial class Networker : Node
{
	const int PORT = 9999;
	const string ADDRESS = "127.0.0.1";
	[Export] PackedScene player;
	Node networked;

	Dictionary<int, long> playerTeamIds = new();
	PlayerPositioner playerPositioner = new();


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		networked = this.GetNodeFromAll<MultiplayerSpawner>();
		playerPositioner.AddSpawnPoints(GetNode("SpawnPoints").GetNodesOfType<SpawnPoint>());
	}

	public void CreateServer()
	{
		var peer = new ENetMultiplayerPeer();
		peer.PeerConnected += OnPlayerConnected;
		peer.CreateServer(PORT);
		Multiplayer.MultiplayerPeer = peer;
		OnPlayerConnected(1);
	}

	public void JoinServer()
	{
		var peer = new ENetMultiplayerPeer();
        _ = peer.CreateClient(ADDRESS, PORT);
		Multiplayer.MultiplayerPeer = peer;
	}

	void OnPlayerConnected(long id)
    {
        GD.Print($"{id} connected");

		var teamId = playerTeamIds.Count;
		playerTeamIds.Add(playerTeamIds.Count, id);

		for (int i = 0; i < Globals.PlayerPerTeamCount; i++)
		{
			var newPlayerNode = player.Instantiate();
			var playerNode = newPlayerNode as Player ?? throw new System.Exception("playerNode must be a Player");
			playerNode.GlobalPosition = playerPositioner.GetNextSpawnPointForTeam(teamId);
			playerNode.PlayerId = id;
	        networked.AddChild(newPlayerNode, true);
		}
    }
}
