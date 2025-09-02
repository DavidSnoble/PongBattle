#!/bin/bash
set -e

echo "Building the game bundle..."
cd PongBattle.Web/wwwroot/game
npm install
npm run build
cd ../../..

echo "Building the development Docker image..."
docker build --network=host -t pongbattle-dev -f Dockerfile.dev .

echo "Starting the development container with hot-reloading..."
docker run --rm -it \
    --network=host \
    -v .:/src \
    -e PONG_BATTLE_DB_USER=SA \
    -e PONG_BATTLE_DB_PASSWORD=ReallyStrongPassword123 \
    pongbattle-dev
