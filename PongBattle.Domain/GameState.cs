using System;
using System.Collections.Concurrent;

namespace PongBattle.Domain;

public class GameState
{
    public required string GameId { get; set; }
    public Ball Ball { get; set; } = new Ball();
    public ConcurrentDictionary<string, Paddle> Paddles { get; set; } = new();
    public int Player1Score { get; set; }
    public int Player2Score { get; set; }
    public bool IsGameActive { get; set; }

    public void Update()
    {
        if (!IsGameActive) return;

        // Update ball position
        Ball.X += Ball.VelocityX;
        Ball.Y += Ball.VelocityY;

        // Ball collision with top/bottom walls
        if (Ball.Y <= 0 || Ball.Y >= 600)
        {
            Ball.VelocityY = -Ball.VelocityY;
        }

        // Ball collision with paddles
        foreach (var paddle in Paddles.Values)
        {
            if (BallCollidesWithPaddle(paddle))
            {
                Ball.VelocityX = -Ball.VelocityX;
                // Add some randomness to the bounce
                Ball.VelocityY += (float)((new Random().NextDouble() - 0.5) * 2);
            }
        }

        // Score when ball goes off screen
        if (Ball.X < 0)
        {
            Player2Score++;
            ResetBall();
        }
        else if (Ball.X > 800)
        {
            Player1Score++;
            ResetBall();
        }
    }

    private bool BallCollidesWithPaddle(Paddle paddle)
    {
        return Ball.X >= paddle.X && Ball.X <= paddle.X + paddle.Width &&
               Ball.Y >= paddle.Y && Ball.Y <= paddle.Y + paddle.Height;
    }

    private void ResetBall()
    {
        Ball.X = 400;
        Ball.Y = 300;
        Ball.VelocityX = Ball.VelocityX > 0 ? -3.333f : 3.333f;
        Ball.VelocityY = (float)((new Random().NextDouble() - 0.5) * 6.666f);
    }
}

public class Ball
{
    public float X { get; set; } = 400;
    public float Y { get; set; } = 300;
    public float VelocityX { get; set; } = 3.333f;
    public float VelocityY { get; set; } = 0;
    public int Size { get; set; } = 10;
}

public class Paddle
{
    public required string PlayerId { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public int Width { get; set; } = 20;
    public int Height { get; set; } = 100;
    public float Speed { get; set; } = 300;
}