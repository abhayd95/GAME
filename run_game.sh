#!/bin/bash

# Free Fire Game Launcher Script
# This script helps you quickly set up and run the game

echo "🔥 Free Fire Game Launcher 🔥"
echo "================================"

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

# Open Unity with the project
echo "🚀 Opening Unity with Free Fire project..."
echo ""

# Try to open with Unity Hub first
if command -v unityhub &> /dev/null; then
    echo "Opening with Unity Hub..."
    unityhub --project-path "$(pwd)"
else
    echo "Opening with Unity directly..."
    unity -projectPath "$(pwd)"
fi

echo ""
echo "📋 Next Steps:"
echo "1. Wait for Unity to load the project"
echo "2. Open Assets/Scenes/MainScene.unity"
echo "3. Add GameSetup script to an empty GameObject"
echo "4. Click Play ▶️"
echo ""
echo "🎮 Controls:"
echo "Desktop: WASD to move, Mouse to look, Left Click to shoot"
echo "Mobile: Use on-screen joysticks and buttons"
echo ""
echo "📖 For detailed instructions, see SETUP_GUIDE.md"
echo ""
echo "Happy Gaming! 🔥"
