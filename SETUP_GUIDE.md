# 🚀 Free Fire Game - Setup & Run Guide

## Quick Start (5 Minutes)

### 1. **Open in Unity**
```bash
# Navigate to your project
cd "/Users/abhaytiwari/Desktop/free fire"

# Open Unity Hub
# Click "Add" → Select "free fire" folder
# Open with Unity 2021.3 LTS or later
```

### 2. **Install Required Packages**
In Unity:
- **Window** → **Package Manager**
- Install these packages:
  - `Netcode for GameObjects` (Multiplayer)
  - `Input System` (Controls)
  - `Universal Render Pipeline` (Graphics)

### 3. **Run the Game**
1. Open `Assets/Scenes/MainScene.unity`
2. In the Hierarchy, right-click → **Create Empty**
3. Name it "GameSetup"
4. Add the `GameSetup` script to it
5. Click **Play** ▶️

## 🎮 How to Play

### **Desktop Controls**
- **WASD** - Move
- **Mouse** - Look around
- **Left Click** - Shoot
- **Space** - Jump
- **Ctrl** - Crouch
- **R** - Reload
- **Shift** - Run

### **Mobile Controls**
- **Left Joystick** - Move
- **Right Joystick** - Look
- **Fire Button** - Shoot
- **Jump Button** - Jump
- **Crouch Button** - Crouch

## 🛠️ Development Setup

### **Step 1: Create Player Prefab**
1. Create a new GameObject
2. Add these components:
   - `CharacterController`
   - `PlayerController` script
   - `PlayerHealth` script
   - `PlayerInventory` script
   - `WeaponSystem` script
3. Save as prefab in `Assets/Prefabs/Player.prefab`

### **Step 2: Setup UI**
1. Create a Canvas (UI → Canvas)
2. Add `MobileControls` script
3. Create UI elements:
   - Joysticks for movement
   - Buttons for actions
   - Health/Ammo displays

### **Step 3: Configure Networking**
1. Add `NetworkManager` to scene
2. Add `GameManager` to scene
3. Configure player prefab in NetworkManager
4. Set up spawn points

## 🎯 Game Modes

### **Solo Mode**
- 50 players, last one standing wins
- Individual gameplay

### **Duo Mode**
- 25 teams of 2 players
- Team-based strategy

### **Squad Mode**
- 12-13 teams of 4 players
- Advanced coordination

## 📱 Mobile Build

### **Android Build**
1. **File** → **Build Settings**
2. Select **Android**
3. **Player Settings**:
   - Company Name: Your Company
   - Product Name: Free Fire Clone
   - Package Name: com.yourcompany.freefire
4. **Build** → Choose output folder

### **iOS Build**
1. **File** → **Build Settings**
2. Select **iOS**
3. **Player Settings**:
   - Bundle Identifier: com.yourcompany.freefire
4. **Build** → Open in Xcode
5. Configure in Xcode and build

## 🔧 Troubleshooting

### **Game Won't Start**
- Check Unity version (2021.3 LTS+)
- Verify all scripts are in correct folders
- Check Console for errors

### **Controls Not Working**
- Ensure Input System package is installed
- Check button assignments in UI
- Verify PlayerController script is attached

### **Multiplayer Issues**
- Install Netcode for GameObjects
- Check NetworkManager configuration
- Verify server/client setup

### **Performance Issues**
- Lower graphics quality in settings
- Enable mobile optimizations
- Check device requirements

## 🎨 Customization

### **Add New Weapons**
1. Create weapon prefab
2. Add to `WeaponSystem.cs` weapon list
3. Configure damage, fire rate, etc.
4. Add to loot system

### **Create New Maps**
1. Design terrain in Unity
2. Add spawn points
3. Configure zone boundaries
4. Test gameplay balance

### **Modify UI**
1. Edit UI prefabs
2. Update `MobileControls.cs`
3. Adjust button positions
4. Test on different screen sizes

## 📊 Performance Tips

### **Mobile Optimization**
- Use Low/Medium quality settings
- Enable Low Power Mode
- Close background apps
- Ensure adequate storage

### **Desktop Optimization**
- Use High/Ultra settings
- Enable VSync for smooth gameplay
- Close unnecessary programs
- Update graphics drivers

## 🚀 Advanced Features

### **Voice Chat**
- Integrate with Unity Voice SDK
- Add voice chat UI
- Configure push-to-talk

### **Custom Skins**
- Create skin system
- Add character customization
- Implement skin marketplace

### **Tournament Mode**
- Add tournament brackets
- Implement ranking system
- Create spectator mode

## 📞 Support

### **Common Issues**
1. **Scripts not found**: Check namespace imports
2. **UI not showing**: Verify Canvas setup
3. **Network errors**: Check Netcode installation
4. **Build failures**: Verify platform settings

### **Getting Help**
- Check Unity Console for errors
- Review script documentation
- Test in Unity Editor first
- Use Unity's built-in profiler

## 🎉 Success!

Once everything is set up, you should have:
- ✅ Working player movement
- ✅ Weapon shooting system
- ✅ Mobile controls
- ✅ Health/damage system
- ✅ Loot spawning
- ✅ Zone shrinking
- ✅ Multiplayer foundation

**Happy Gaming!** 🎮🔥
