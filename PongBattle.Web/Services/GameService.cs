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
        var gameState = new GameState { GameId = gameId, IsGameActive = false };

        // Initialize both paddles immediately so collision detection works from the start
        var leftPaddle = new Paddle
        {
            PlayerId = "player1",
            X = 50,  // Left paddle position
            Y = 300, // Center vertically
        };

        var rightPaddle = new Paddle
        {
            PlayerId = "player2",
            X = 730, // Right paddle position
            Y = 300, // Center vertically
        };

        gameState.Paddles["player1"] = leftPaddle;
        gameState.Paddles["player2"] = rightPaddle;

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
                float paddleX;
                if (playerId == "player1")
                {
                    paddleX = 50;
                }
                else
                {
                    paddleX = 730;
                }
                var newPaddle = new Paddle
                {
                    PlayerId = playerId,
                    X = paddleX,
                    Y = yPosition,
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

    public void CheckAndHandleGameEnd(string gameId)
    {
        if (_games.TryGetValue(gameId, out var gameState))
        {
            if (gameState.IsGameFinished)
            {
                // Notify all clients that game has ended
                _hubContext.Clients.Group(gameId).SendAsync("GameEnded", new
                {
                    Winner = gameState.Winner,
                    Player1Score = gameState.Player1Score,
                    Player2Score = gameState.Player2Score
                });
            }
        }
    }

    public void RestartGame(string gameId)
    {
        if (_games.TryGetValue(gameId, out var gameState))
        {
            // Reset game state
            gameState.IsGameActive = true;
            gameState.IsGameFinished = false;
            gameState.Winner = null;
            gameState.Player1Score = 0;
            gameState.Player2Score = 0;

            // Reset ball to center
            gameState.Ball.X = 400;
            gameState.Ball.Y = 300;
            gameState.Ball.VelocityX = 3.333f;
            gameState.Ball.VelocityY = 0;

            // Reset paddles to center
            if (gameState.Paddles.TryGetValue("player1", out var leftPaddle))
            {
                leftPaddle.Y = 300;
            }
            if (gameState.Paddles.TryGetValue("player2", out var rightPaddle))
            {
                rightPaddle.Y = 300;
            }

            // Notify clients
            _hubContext.Clients.Group(gameId).SendAsync("GameRestarted");
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
            if (game.IsGameActive)
            {
                var oldX = game.Ball.X;
                var oldY = game.Ball.Y;
                game.Update();
                var newX = game.Ball.X;
                var newY = game.Ball.Y;

                // Check if game should end
                CheckAndHandleGameEnd(game.GameId);
            }

            // Broadcast the updated game state to all clients
            var updateData = new
            {
                Ball = new { X = game.Ball.X, Y = game.Ball.Y },
                Paddles = game.Paddles.ToDictionary(p => p.Key, p => new { p.Value.X, p.Value.Y }),
                Player1Score = game.Player1Score,
                Player2Score = game.Player2Score
            };
            _hubContext.Clients.Group(game.GameId).SendAsync("GameUpdate", updateData);
        }
    }
}

