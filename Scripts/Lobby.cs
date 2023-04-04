using System;
using System.Linq;
using Godot;
using Nidot;

public partial class Lobby : Node
{
    const int PORT = 9999;
    const string ADDRESS = "127.0.0.1";
    [Export] NodePath hostClientContainer;
    [Export] NodePath gameSettingsSelection;
    [Export] Label lobbyLabel;
    [Export] PackedScene lobbyPlayer;
    [Export] PackedScene gameLevel;
    [Export] NodePath startGameButton;
    [Export] NodePath lobbyMenu;

    MultiplayerSpawner multiplayerSpawner;
    Control gameMenu;

    internal GameSettings GameSettings { get; set; }

    public override void _Ready()
    {
        GD.Print($"{nameof(Lobby)} ready");
        multiplayerSpawner = this.GetChildOfType<MultiplayerSpawner>();
        GameSettings = new GameSettings();
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

    // Called from UI -- should be called when starting headless server, how to?
    public void CreateServer()
    {
        var peer = new ENetMultiplayerPeer();
        peer.PeerConnected += OnPlayerConnected;
        peer.CreateServer(PORT);
        Multiplayer.MultiplayerPeer = peer;
        GetNode(hostClientContainer).QueueFree();

        var gameSettingsOptions = GetNode<ItemList>(gameSettingsSelection);
        gameSettingsOptions.Show();
        gameSettingsOptions.Select(0);
        GetNode<Button>(startGameButton).Show();
    }

    // Called from UI
    public void JoinServer()
    {
        var peer = new ENetMultiplayerPeer();
        _ = peer.CreateClient(ADDRESS, PORT);
        Multiplayer.MultiplayerPeer = peer;
        GetNode(hostClientContainer).QueueFree();
    }

    public void SelectGameSettings(int id)
    {
        GameSettings = new GameSettings((Game)id);
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
        var playerNode = newPlayerNode as LobbyPlayer ?? throw new Exception("playerNode must be a LobbyPlayer");
        playerNode.PlayerId = id;
        var isFirstTeam = Multiplayer.GetPeers().Length == 1;
        GD.Print($"{nameof(isFirstTeam)}: {isFirstTeam}");
        playerNode.TeamColor = isFirstTeam ? new Color(0, 1, 0) : new Color(1, 0, 0);
        multiplayerSpawner.AddChild(playerNode, true);
    }

    public Color GetTeamColor(long id) => multiplayerSpawner.GetNodesOfType<LobbyPlayer>().FirstOrDefault(x => x.PlayerId == id)?.TeamColor ?? throw new Exception($"No team with id {id} found");
}
