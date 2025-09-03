<script>
  import { onMount } from 'svelte';
  import Phaser from 'phaser';
  import { HubConnectionBuilder } from '@microsoft/signalr';

  let gameContainer;
  let connection;
  let gameState = null;
  let leftPaddle;
  let rightPaddle;
  let ball;
  let phaserReady = false;
  let pendingUpdates = [];
  let keys = {};
  let playerId = null;
  let player1Score = 0;
  let player2Score = 0;

  onMount(async () => {
    connection = new HubConnectionBuilder()
      .withUrl('/gameHub')
      .build();

    connection.on('GameState', (state) => {
      gameState = state;
      player1Score = state.Player1Score || 0;
      player2Score = state.Player2Score || 0;
      updateGameObjects();
    });

    connection.on('PlayerJoined', (playerId) => {
      // Store our player ID for paddle control
      if (!playerId) {
        playerId = 'player1'; // Default to player1 if not specified
      }
    });

    connection.on('PaddleMoved', (playerId, yPosition) => {
      updatePaddlePosition(playerId, yPosition);
    });

    connection.on('GameStarted', () => {
      // Game has started
    });

    connection.on('GameUpdate', (updatedGameState) => {
       gameState = updatedGameState;

       // Try different property name variations for compatibility
       player1Score = updatedGameState.Player1Score || updatedGameState.player1Score || 0;
       player2Score = updatedGameState.Player2Score || updatedGameState.player2Score || 0;

       if (ball) {
         updateGameObjects();
       } else {
         pendingUpdates.push(updatedGameState);
       }
     });

    // Start connection and join game
    await connection.start();
    await connection.invoke('JoinGame', 'default-game');
    await connection.invoke('StartGame', 'default-game');

    // Set up Phaser game
    const config = {
      type: Phaser.AUTO,
      width: 800,
      height: 600,
      parent: gameContainer,
      scene: {
        preload: preload,
        create: create,
        update: update
      }
    };

    const game = new Phaser.Game(config);

    function preload() {
      // Load assets here
    }

    function create() {
      // Set black background
      this.cameras.main.setBackgroundColor('#000000');

      // Create game objects
      leftPaddle = this.add.rectangle(50, 300, 20, 100, 0xffffff);
      rightPaddle = this.add.rectangle(730, 300, 20, 100, 0xffffff); // Fixed: matches backend collision detection
      ball = this.add.circle(400, 300, 10, 0xff0000);

      // Mark Phaser as ready and process any pending updates
      phaserReady = true;

      // Process any updates that arrived before Phaser was ready
      while (pendingUpdates.length > 0) {
        const pendingUpdate = pendingUpdates.shift();
        gameState = pendingUpdate;
        player1Score = pendingUpdate.Player1Score || pendingUpdate.player1Score || 0;
        player2Score = pendingUpdate.Player2Score || pendingUpdate.player2Score || 0;
        updateGameObjects();
      }



      // Set up keyboard input
      this.input.keyboard.on('keydown-W', () => {
        keys['W'] = true;
      });
      this.input.keyboard.on('keyup-W', () => {
        keys['W'] = false;
      });
      this.input.keyboard.on('keydown-S', () => {
        keys['S'] = true;
      });
      this.input.keyboard.on('keyup-S', () => {
        keys['S'] = false;
      });

      this.input.keyboard.on('keydown-UP', () => {
        keys['UP'] = true;
      });
      this.input.keyboard.on('keyup-UP', () => {
        keys['UP'] = false;
      });
      this.input.keyboard.on('keydown-DOWN', () => {
        keys['DOWN'] = true;
      });
      this.input.keyboard.on('keyup-DOWN', () => {
        keys['DOWN'] = false;
      });

      // Set player ID to control left paddle
      playerId = 'player1';
    }

    function update() {
      // Handle continuous paddle movement
      if (connection && gameState && leftPaddle && rightPaddle) {
        const paddleSpeed = 5;

        // Handle left paddle (W/S keys)
        let leftDeltaY = 0;
        if (keys['W']) {
          leftDeltaY = -paddleSpeed;
        } else if (keys['S']) {
          leftDeltaY = paddleSpeed;
        }

        if (leftDeltaY !== 0) {
          const newLeftY = Math.max(50, Math.min(550, leftPaddle.y + leftDeltaY));
          if (newLeftY !== leftPaddle.y) {
            leftPaddle.y = newLeftY;
            connection.invoke('MovePaddle', 'player1', newLeftY);
          }
        }

        // Handle right paddle (UP/DOWN keys)
        let rightDeltaY = 0;
        if (keys['UP']) {
          rightDeltaY = -paddleSpeed;
        } else if (keys['DOWN']) {
          rightDeltaY = paddleSpeed;
        }

        if (rightDeltaY !== 0) {
          const newRightY = Math.max(50, Math.min(550, rightPaddle.y + rightDeltaY));
          if (newRightY !== rightPaddle.y) {
            rightPaddle.y = newRightY;
            connection.invoke('MovePaddle', 'player2', newRightY);
          }
        }
      }
    }
  });



  function updateGameObjects() {
    if (!gameState) {
      return;
    }

    if (!phaserReady) {
      return;
    }

    // Update ball position
    if (ball) {
      const newX = parseFloat(gameState.ball.x);
      const newY = parseFloat(gameState.ball.y);

      ball.setPosition(newX, newY);
    }

    // Update paddles
    if (gameState.Paddles) {
      Object.entries(gameState.Paddles).forEach(([playerId, paddle]) => {
        if (playerId === 'player1' && leftPaddle) {
          leftPaddle.setPosition(paddle.X, paddle.Y); // Sync both X and Y from server
        } else if (playerId === 'player2' && rightPaddle) {
          rightPaddle.setPosition(paddle.X, paddle.Y); // Sync both X and Y from server
        }
      });
    }
  }

  function updatePaddlePosition(playerId, yPosition) {
    if (playerId === 'player1' && leftPaddle) {
      leftPaddle.y = yPosition; // Update Y position from real-time movement
    } else if (playerId === 'player2' && rightPaddle) {
      rightPaddle.y = yPosition; // Update Y position from real-time movement
    }
  }
</script>

<main>
  <h2>Svelte + Phaser.js Pong Game</h2>
  <div class="score-display">
    <span class="player1-score">Player 1: {player1Score}</span>
    <span class="player2-score">Player 2: {player2Score}</span>
  </div>
  <div bind:this={gameContainer} class="game-canvas"></div>
</main>

<style>
  .game-canvas {
    border: 1px solid #ccc;
    margin: 20px 0;
  }

  .score-display {
    display: flex;
    justify-content: space-between;
    padding: 10px;
    background-color: #f0f0f0;
    border: 1px solid #ccc;
    margin-bottom: 10px;
  }

  .player1-score, .player2-score {
    font-size: 18px;
    font-weight: bold;
  }
</style>
