using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using Godot;
using Nidot;

public partial class Lobby : Node
{
	const int PORT = 9999;
	const string ADDRESS = "127.0.0.1";

    List<long> teamIds = new();

    [Export]
    NodePath hostClientContainer;

    [Export]
    Label lobbyLabel;

    [Export]
    PackedScene lobbyPlayer;

    MultiplayerSpawner lobbyPlayerSpawner;

    public override void _Ready()
    {
        lobbyPlayerSpawner = this.GetChildOfType<MultiplayerSpawner>();
    }

    public override void _Process(double delta)
    {
        lobbyLabel.Text = "Players: \n";
        foreach (var item in lobbyPlayerSpawner.GetChildren())
        {
            lobbyLabel.Text += ((LobbyPlayer)item).PlayerId + "\n";
        }
    }

    public void CreateServer()
	{
		var peer = new ENetMultiplayerPeer();
		peer.PeerConnected += OnPlayerConnected;
		peer.CreateServer(PORT);
		Multiplayer.MultiplayerPeer = peer;
        OnPlayerConnected(1);
        this.GetNode(hostClientContainer).QueueFree();
	}

    public void JoinServer()
	{
		var peer = new ENetMultiplayerPeer();
        _ = peer.CreateClient(ADDRESS, PORT);
		Multiplayer.MultiplayerPeer = peer;
        this.GetNode(hostClientContainer).QueueFree();
	}

    private void OnPlayerConnected(long id)
    {
        GD.Print($"Connected: {id}");
        var newPlayerNode = lobbyPlayer.Instantiate();
        var playerNode = newPlayerNode as LobbyPlayer ?? throw new System.Exception("playerNode must be a LobbyPlayer");
        playerNode.PlayerId = id;
        lobbyPlayerSpawner.AddChild(playerNode, true);
    }
}