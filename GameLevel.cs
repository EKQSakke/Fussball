using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Nidot;

public partial class GameLevel : Node
{
    [Export] PackedScene player;
    Node networked;

    Dictionary<int, PlayerTeam> playerTeamIds = new();
    PlayerPositioner playerPositioner = new();
    Lobby lobby;

    List<Player> players = new();

    [Export] double currentTimer;

    [Export] public GameState CurrentGameState;

    GameState lastState;

    Label gameStateLabel;

    bool localReady;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        gameStateLabel = this.GetChildOfType<Label>();
        CurrentGameState = GameState.Command;
        currentTimer = Rules.CommandTime;
        lobby = this.GetNodeFromAll<Lobby>();
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

    public override void _Process(double delta)
    {
        if (Multiplayer.IsServer())
        {
            RunGameTimer(delta);
        }
        else if (lastState != CurrentGameState)
        {
            lastState = CurrentGameState;
            localReady = false;
        }

        gameStateLabel.Text = $"{currentTimer.ToString("F2")}\n{CurrentGameState.ToString()}\nTeam:{IsTeamReady()}\n";
        if (Multiplayer.IsServer())
        {
            gameStateLabel.Text += $"All:{AllPlayersAreReady()}";
        }
    }

    void RunGameTimer(double delta)
    {
        currentTimer -= delta;

        switch (CurrentGameState)
        {
            case GameState.Act:
                if (currentTimer < 0)
                {
                    currentTimer = Rules.CommandTime;
                    CurrentGameState = GameState.Command;
                }

                break;

            case GameState.Command:
                if (currentTimer < 0 || AllPlayersAreReady())
                {
                    ActCommands();
                    currentTimer = Rules.ActTime;
                    CurrentGameState = GameState.Act;
                }

                break;
        }
    }

    bool AllPlayersAreReady() => playerTeamIds.Values.All(e => e.IsReady);

    public void EndLevel()
    {
        if (Multiplayer.IsServer())
        {
            lobby.SetGameMenuVisible(true);
            Rpc("ShowMenu");
            this.QueueFree();
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void ShowMenu() => lobby.SetGameMenuVisible(true);

    void ActCommands()
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }

        foreach (var item in players)
        {
            item.ApplyCommand();
        }

        foreach (var item in playerTeamIds.Values)
        {
            item.IsReady = false;
        }
    }


    public void SetTeamReady()
    {
        if (Multiplayer.IsServer())
        {
            playerTeamIds[Multiplayer.GetUniqueId()].IsReady = true;
        }
        else
        {
            localReady = true;
            Rpc("SetTeamReady", Multiplayer.GetUniqueId());
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void SetTeamReady(int id)
    {
        if (Multiplayer.IsServer())
        {
            playerTeamIds[id].IsReady = true;
        }
    }

    public bool IsTeamReady()
    {
        if (Multiplayer.IsServer())
        {
            return playerTeamIds[Multiplayer.GetUniqueId()].IsReady;
        }
        else
        {
            return localReady;
        }
    }

    void SpawnTeam(long id)
    {
        GD.Print($"SpawnTeam {id}");

        var teamId = playerTeamIds.Count;
        playerTeamIds.Add((int)id, new PlayerTeam());

        for (int i = 0; i < Globals.PlayerPerTeamCount; i++)
        {
            var newPlayerNode = player.Instantiate();
            var playerNode = newPlayerNode as Player ?? throw new Exception("playerNode must be a Player");
            playerNode.PlayerId = id;
            networked.AddChild(newPlayerNode, true);
            playerNode.GlobalPosition = playerPositioner.GetNextSpawnPointForTeam(teamId);
            players.Add(playerNode);
        }
    }
}