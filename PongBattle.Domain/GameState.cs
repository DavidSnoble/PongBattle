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
    public bool IsGameFinished { get; set; }
    public string Winner { get; set; }
    public const int WINNING_SCORE = 7;

    public void Update()
    {
        if (!IsGameActive)
            return;

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
                // Reverse X direction
                Ball.VelocityX = -Ball.VelocityX;

                // Calculate bounce angle based on where ball hits paddle
                float paddleCenter = paddle.Y + paddle.Height / 2f;
                float ballRelativeY = Ball.Y - paddleCenter;
                float normalizedPosition = ballRelativeY / (paddle.Height / 2f);

                // Adjust Y velocity based on hit position (max angle of 60 degrees)
                float maxAngle = (float)(Math.PI / 3); // 60 degrees
                float bounceAngle = normalizedPosition * maxAngle;

                // Set new Y velocity based on bounce angle and current speed
                float currentSpeed = (float)
                    Math.Sqrt(Ball.VelocityX * Ball.VelocityX + Ball.VelocityY * Ball.VelocityY);
                Ball.VelocityY = (float)(Math.Sin(bounceAngle) * currentSpeed);

                // Ensure minimum Y velocity to prevent straight horizontal movement
                if (Math.Abs(Ball.VelocityY) < 1.0f)
                {
                    Ball.VelocityY = Ball.VelocityY > 0 ? 1.0f : -1.0f;
                }
            }
        }

        // Score when ball goes off screen
        if (Ball.X < 0)
        {
            Player2Score++;
            ResetBall();
            CheckWinCondition();
        }
        else if (Ball.X > 800)
        {
            Player1Score++;
            ResetBall();
            CheckWinCondition();
        }
    }

    private bool BallCollidesWithPaddle(Paddle paddle)
    {
        // Account for ball radius in collision detection
        float ballRadius = Ball.Size / 2f;
        float paddleHalfWidth = paddle.Width / 2f;
        float paddleHalfHeight = paddle.Height / 2f;
        return Ball.X + ballRadius >= paddle.X - paddleHalfWidth
            && Ball.X - ballRadius <= paddle.X + paddleHalfWidth
            && Ball.Y + ballRadius >= paddle.Y - paddleHalfHeight
            && Ball.Y - ballRadius <= paddle.Y + paddleHalfHeight;
    }

    private void ResetBall()
    {
        Ball.X = 400;
        Ball.Y = 300;
        Ball.VelocityX = Ball.VelocityX > 0 ? -3.333f : 3.333f;
        Ball.VelocityY = (float)((new Random().NextDouble() - 0.5) * 6.666f);
    }

    private void CheckWinCondition()
    {
        if (Player1Score >= WINNING_SCORE)
        {
            IsGameFinished = true;
            Winner = "player1";
            IsGameActive = false;
        }
        else if (Player2Score >= WINNING_SCORE)
        {
            IsGameFinished = true;
            Winner = "player2";
            IsGameActive = false;
        }
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

