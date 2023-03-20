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
    NodePath gameSettingsSelection;

    [Export]
    Label lobbyLabel;

    [Export]
    PackedScene lobbyPlayer;

    [Export] PackedScene gameLevel;

    [Export] NodePath startGameButton;

    [Export] NodePath lobbyMenu;

    MultiplayerSpawner multiplayerSpawner;

    GameSettings gameSettings;

    Control gameMenu;

    public override void _Ready()
    {
        multiplayerSpawner = this.GetChildOfType<MultiplayerSpawner>();
        gameSettings = new GameSettings();
        gameMenu = GetNode<Control>(lobbyMenu);
    }

    public override void _Process(double delta)
    {
        if (gameMenu.Visible)
        {
            lobbyLabel.Text = "Players: \n";
            foreach (var item in multiplayerSpawner.GetChildren())
            {
                if (item is LobbyPlayer lobbyItem)
                {
                    lobbyLabel.Text += lobbyItem.PlayerId + "\n";
                }
            }
        }
    }

    public void CreateServer()
    {
        var peer = new ENetMultiplayerPeer();
        peer.PeerConnected += OnPlayerConnected;
        peer.CreateServer(PORT);
        Multiplayer.MultiplayerPeer = peer;
        OnPlayerConnected(1);
        GetNode(hostClientContainer).QueueFree();

        var gameSettingsOptions = GetNode<ItemList>(gameSettingsSelection);
        gameSettingsOptions.Show();
        gameSettingsOptions.Select(0);
        GetNode<Button>(startGameButton).Show();
    }

    public void JoinServer()
    {
        var peer = new ENetMultiplayerPeer();
        _ = peer.CreateClient(ADDRESS, PORT);
        Multiplayer.MultiplayerPeer = peer;
        GetNode(hostClientContainer).QueueFree();
    }

    public void SelectGameSettings(int id)
    {
        gameSettings = new GameSettings((Game)id);
    }

    public void StartGame()
    {
        GetNode<Control>(lobbyMenu).Hide();
        var level = gameLevel.Instantiate();
        multiplayerSpawner.AddChild(level, true);
    }

    public void SetGameMenuVisible(bool value)
    {
        if (value)
        {
            gameMenu.Show();
        }
        else
        {
            gameMenu.Hide();
        }
    }

    private void OnPlayerConnected(long id)
    {
        GD.Print($"Connected: {id}");
        var newPlayerNode = lobbyPlayer.Instantiate();
        var playerNode = newPlayerNode as LobbyPlayer ?? throw new System.Exception("playerNode must be a LobbyPlayer");
        playerNode.PlayerId = id;
        multiplayerSpawner.AddChild(playerNode, true);
    }
}
