#!/bin/bash

# Free Fire Game - Localhost Server
# Developed by abhay virus 🔥

echo "🔥 Free Fire Game - Localhost Server 🔥"
echo "Developed by: abhay virus"
echo "========================================"

# Check if Unity is installed
if ! command -v unity &> /dev/null; then
    echo "❌ Unity not found in PATH"
    echo "Please install Unity Hub and Unity 2021.3 LTS"
    echo "Download from: https://unity.com"
    exit 1
fi

# Check if project exists
if [ ! -d "Assets" ]; then
    echo "❌ Assets folder not found"
    echo "Make sure you're in the correct project directory"
    exit 1
fi

echo "✅ Project structure found"
echo "✅ Unity detected"

# Start Unity with localhost settings
echo "🚀 Starting Unity with localhost configuration..."
echo ""

# Set localhost environment variables
export UNITY_LOCALHOST=true
export UNITY_SERVER_PORT=7777
export UNITY_CLIENT_PORT=7778

# Open Unity with localhost project
echo "Opening Unity with localhost settings..."
unity -projectPath "$(pwd)" -batchmode -quit -executeMethod FreeFire.Setup.GameSetup.SetupLocalhost

echo ""
echo "📋 Localhost Configuration:"
echo "Server Port: 7777"
echo "Client Port: 7778"
echo "Local IP: 127.0.0.1"
echo ""
echo "🎮 To connect to localhost:"
echo "1. Open Unity project"
echo "2. Add GameSetup script to empty GameObject"
echo "3. Click Play ▶️"
echo "4. Game will run on localhost:7777"
echo ""
echo "🌐 Access your game at:"
echo "http://localhost:7777"
echo ""
echo "Happy Gaming! 🔥"
