using System.Collections.Concurrent;
using System.Timers;
using Microsoft.AspNetCore.SignalR;
using PongBattle.Domain;
using PongBattle.Web.Hubs;

namespace PongBattle.Web.Services;

public class GameService
{
    private readonly ConcurrentDictionary<string, GameState> _games = new();
    private readonly System.Timers.Timer _gameTimer;
    private readonly IHubContext<GameHub> _hubContext;

    public GameService(IHubContext<GameHub> hubContext)
    {
        _hubContext = hubContext;
        _gameTimer = new System.Timers.Timer(1000 / 60); // 60 FPS
        _gameTimer.Elapsed += UpdateGames;
        _gameTimer.Start();
    }

    public GameState CreateGame(string gameId)
    {
        var gameState = new GameState
        {
            GameId = gameId,
            IsGameActive = true
        };

        _games[gameId] = gameState;
        return gameState;
    }

    public GameState GetGameState(string gameId)
    {
        return _games.TryGetValue(gameId, out var game) ? game : null;
    }

    public void UpdatePaddlePosition(string gameId, string playerId, float yPosition)
    {
        if (_games.TryGetValue(gameId, out var gameState))
        {
            if (gameState.Paddles.TryGetValue(playerId, out var paddle))
            {
                // Clamp paddle position to screen bounds
                paddle.Y = Math.Max(0, Math.Min(500, yPosition));
            }
            else
            {
                // Create new paddle for player
                var newPaddle = new Paddle
                {
                    PlayerId = playerId,
                    X = gameState.Paddles.Count == 0 ? 50 : 730, // Left or right paddle
                    Y = yPosition
                };
                gameState.Paddles[playerId] = newPaddle;
            }
        }
    }

    public void StartGame(string gameId)
    {
        if (_games.TryGetValue(gameId, out var gameState))
        {
            gameState.IsGameActive = true;
        }
    }

    public void EndGame(string gameId)
    {
        if (_games.TryGetValue(gameId, out var gameState))
        {
            gameState.IsGameActive = false;
        }
    }

    public void RemovePlayer(string gameId, string playerId)
    {
        if (_games.TryGetValue(gameId, out var gameState))
        {
            gameState.Paddles.TryRemove(playerId, out _);

            // End game if no players left
            if (gameState.Paddles.IsEmpty)
            {
                gameState.IsGameActive = false;
            }
        }
    }

    private int _updateCounter = 0;

    private void UpdateGames(object sender, ElapsedEventArgs e)
    {
        _updateCounter++;
        foreach (var game in _games.Values)
        {
            var oldX = game.Ball.X;
            var oldY = game.Ball.Y;
            game.Update();
            var newX = game.Ball.X;
            var newY = game.Ball.Y;

        // Broadcast the updated game state to all clients
        var updateData = new {
            Ball = new { X = newX, Y = newY },
            Paddles = game.Paddles.ToDictionary(p => p.Key, p => new { p.Value.X, p.Value.Y })
        };
        _hubContext.Clients.Group(game.GameId).SendAsync("GameUpdate", updateData);
        }
    }
}