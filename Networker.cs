using Godot;

public partial class Networker : Node
{
	const int PORT = 9999;
	const string ADDRESS = "127.0.0.1";
	[Export] PackedScene player;
	Node networked;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		networked = GetNode("MultiplayerSpawner");
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
		peer.CreateClient(ADDRESS, PORT);
		Multiplayer.MultiplayerPeer = peer;
	}

	void OnPlayerConnected(long id) {
		GD.Print($"{id} connected");
		var newPlayerNode = player.Instantiate();
		networked.AddChild(newPlayerNode, true);
	}
}
