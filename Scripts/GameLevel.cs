using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Nidot;

public partial class GameLevel : Node
{
    [Export] PackedScene player;
    [Export] double currentTimer;
    [Export] public GameState CurrentGameState;

    Node networked;
    readonly Dictionary<int, PlayerTeam> playerTeamIds = new();
    readonly PlayerPositioner playerPositioner = new();
    Lobby lobby;
    readonly List<Player> players = new();
    GameState lastState;
    Label gameStateLabel;
    Ball ball;
    GameScore score;
    GameSettings settings;
    int rounds;
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
        ball = this.GetNodeFromChildren<Ball>();
        settings = lobby.GameSettings;
        score = new GameScore(settings.GoalLimit);

        if (Multiplayer.IsServer())
        {
            // Server team
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

        gameStateLabel.Text = $"{currentTimer:F2}\n{CurrentGameState}\nTeam:{IsTeamReady()}\n";
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
                    if (rounds > settings.RoundLimit)
                    {
                        EndGameWithWinner();
                        return;
                    }
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

            case GameState.Goal:
                if (currentTimer < 0)
                {
                    if (!score.IsGameOver())
                    {
                        ResetLevel();
                        return;
                    }

                    EndGameWithWinner();
                }

                break;
        }
    }

    private void EndGameWithWinner()
    {
        var winner = score.GetWinner();
        GD.Print($"Team {winner} won!");
        EndLevel();
    }

    private void ResetLevel()
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }

        // reset ball
        ball.GlobalPosition = new Vector3(0, 0.5f, 0);
        ball.LinearVelocity = Vector3.Zero;

        // reset players
        var team0counter = 0;
        var team1Counter = 0;

        foreach (var item in players)
        {
            if (item.teamId == 0)
            {
                item.GlobalPosition = playerTeamIds[(int)item.PlayerId].SpawnPoints[team0counter];
                team0counter++;
            }
            else
            {
                item.GlobalPosition = playerTeamIds[(int)item.PlayerId].SpawnPoints[team1Counter];
                team1Counter++;
            }
            item.LinearVelocity = Vector3.Zero;
        }

        // reset timer
        currentTimer = Rules.CommandTime;
        CurrentGameState = GameState.Command;
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
            return;
        }
        
        localReady = true;
        Rpc("SetTeamReady", Multiplayer.GetUniqueId());
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void SetTeamReady(int id)
    {
        if (Multiplayer.IsServer())
        {
            playerTeamIds[id].IsReady = true;
        }
    }

    public bool IsTeamReady() => Multiplayer.IsServer() || localReady;

    public void GoalScored(int goalId)
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }
        GD.Print($"Goal scored to: {goalId}");
        CurrentGameState = GameState.Goal;

        score.GoalScored(goalId);
    }

    void SpawnTeam(long id)
    {
        GD.Print($"SpawnTeam {id}");

        var teamId = playerTeamIds.Count;
        var team = new PlayerTeam() { TeamId = teamId, SpawnPoints = new() };
        playerTeamIds.Add((int)id, team);

        for (int i = 0; i < Globals.PlayerPerTeamCount; i++)
        {
            var newPlayerNode = player.Instantiate();
            var playerNode = newPlayerNode as Player ?? throw new Exception("playerNode must be a Player");
            playerNode.PlayerId = id;
            networked.AddChild(newPlayerNode, true);
            playerNode.GlobalPosition = playerPositioner.GetNextSpawnPointForTeam(teamId);
            players.Add(playerNode);
            playerNode.teamId = teamId;
            GD.Print($"{Multiplayer.GetUniqueId()} - {id} - {lobby.GetTeamColor(id)}");
            playerNode.SetColorToMesh(lobby.GetTeamColor(id));
            team.SpawnPoints.Add(playerNode.GlobalPosition);
        }
    }
}