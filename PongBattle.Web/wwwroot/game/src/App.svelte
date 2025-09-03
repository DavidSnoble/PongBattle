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

  onMount(async () => {
    // Connect to SignalR hub
    connection = new HubConnectionBuilder()
      .withUrl('/gameHub')
      .build();

    // Set up event handlers
    connection.on('GameState', (state) => {
      console.log('Received GameState:', state);
      gameState = state;
      updateGameObjects();
    });

    connection.on('PlayerJoined', (playerId) => {
      console.log('Player joined:', playerId);
      // Store our player ID for paddle control
      if (!playerId) {
        playerId = 'player1'; // Default to player1 if not specified
      }
    });

    connection.on('PaddleMoved', (playerId, yPosition) => {
      console.log('Paddle moved:', playerId, yPosition);
      updatePaddlePosition(playerId, yPosition);
    });

    connection.on('GameStarted', () => {
      console.log('Game started!');
    });

    connection.on('GameUpdate', (updatedGameState) => {
       gameState = updatedGameState;

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
      rightPaddle = this.add.rectangle(750, 300, 20, 100, 0xffffff);
      ball = this.add.circle(400, 300, 10, 0xff0000);

      // Mark Phaser as ready and process any pending updates
      phaserReady = true;

      // Process any updates that arrived before Phaser was ready
      while (pendingUpdates.length > 0) {
        const pendingUpdate = pendingUpdates.shift();
        gameState = pendingUpdate;
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

      // Set player ID to control left paddle
      playerId = 'player1';
    }

    function update() {
      // Handle continuous paddle movement
      if (connection && gameState && leftPaddle) {
        const paddleSpeed = 5;
        let deltaY = 0;

        if (keys['W']) {
          deltaY = -paddleSpeed;
        } else if (keys['S']) {
          deltaY = paddleSpeed;
        }

        if (deltaY !== 0) {
          const newY = Math.max(50, Math.min(550, leftPaddle.y + deltaY));
          if (newY !== leftPaddle.y) {
            leftPaddle.y = newY;
            connection.invoke('MovePaddle', newY);
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
          leftPaddle.y = paddle.Y;
        } else if (playerId === 'player2' && rightPaddle) {
          rightPaddle.y = paddle.Y;
        }
      });
    }
  }

  function updatePaddlePosition(playerId, yPosition) {
    if (playerId === 'player1' && leftPaddle) {
      leftPaddle.y = yPosition;
    } else if (playerId === 'player2' && rightPaddle) {
      rightPaddle.y = yPosition;
    }
  }
</script>

<main>
  <h2>Svelte + Phaser.js Pong Game</h2>
  <div bind:this={gameContainer} class="game-canvas"></div>
</main>

<style>
  .game-canvas {
    border: 1px solid #ccc;
    margin: 20px 0;
  }
</style>