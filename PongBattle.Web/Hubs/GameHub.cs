using Microsoft.AspNetCore.SignalR;
using PongBattle.Domain;
using PongBattle.Web.Services;

namespace PongBattle.Web.Hubs;

public class GameHub : Hub
{
    private readonly GameService _gameService;

    public GameHub(GameService gameService)
    {
        _gameService = gameService;
    }

    public async Task JoinGame(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);

        // Create game if it doesn't exist
        var gameState = _gameService.GetGameState(gameId);
        if (gameState == null)
        {
            gameState = _gameService.CreateGame(gameId);
        }

        await Clients.Group(gameId).SendAsync("PlayerJoined", Context.ConnectionId);
        await Clients.Caller.SendAsync("GameState", gameState);
    }

    public async Task MovePaddle(string playerId, float yPosition)
    {
        // Get game ID from connection (you might want to store this in a connection mapping)
        var gameId = GetGameIdForConnection(Context.ConnectionId);
        if (gameId != null)
        {
            _gameService.UpdatePaddlePosition(gameId, playerId, yPosition);
            await Clients.Group(gameId).SendAsync("PaddleMoved", Context.ConnectionId, yPosition);
        }
    }

    public async Task StartGame(string gameId)
    {
        _gameService.StartGame(gameId);
        await Clients.Group(gameId).SendAsync("GameStarted");
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var gameId = GetGameIdForConnection(Context.ConnectionId);
        if (gameId != null)
        {
            _gameService.RemovePlayer(gameId, Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
            await Clients.Group(gameId).SendAsync("PlayerLeft", Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Helper method - in a real implementation, you'd maintain a connection-to-game mapping
    private string GetGameIdForConnection(string connectionId)
    {
        // This is a simplified implementation
        // In production, you'd use a proper mapping service
        return "default-game";
    }
}

