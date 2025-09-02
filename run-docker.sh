#!/bin/bash
set -e

echo "Building the game bundle..."
cd PongBattle.Web/wwwroot/game
npm install
npm run build
cd ../../..

echo "Building the Docker image..."
docker build --network=host -t pongbattle .

echo "Running the application..."
docker run --network=host -e PONG_BATTLE_DB_USER=SA -e PONG_BATTLE_DB_PASSWORD=ReallyStrongPassword123 pongbattle
