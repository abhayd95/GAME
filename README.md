# Free Fire Style Battle Royale Game

**Developed by abhay virus** 🔥

A complete Free Fire-style mobile battle royale shooter game built with Unity and C#. This project includes all the core features of a modern battle royale game optimized for mobile devices.

## 🎮 Features

### Core Gameplay
- **3D Third-Person Shooter**: Fast-paced combat with realistic weapon mechanics
- **Battle Royale Mode**: 50-player matches with shrinking safe zones
- **Multiple Game Modes**: Solo, Duo, and Squad gameplay
- **Dynamic Zone System**: Shrinking safe zones with damage over time
- **Loot System**: Weapons, armor, health items, and ammunition scattered across the map

### Mobile-Optimized Controls
- **Customizable Joystick**: On-screen movement and look controls
- **Touch Controls**: Fire, jump, crouch, reload, and scope buttons
- **Drag & Drop UI**: Repositionable buttons for personalized layouts
- **Haptic Feedback**: Enhanced mobile gaming experience

### Weapon System
- **Multiple Weapon Types**: SMG, Assault Rifle, Sniper Rifle, Shotgun, Pistol
- **Realistic Ballistics**: Bullet drop, recoil, and damage mechanics
- **Weapon Customization**: Attachments and modifications
- **Ammo Management**: Different ammunition types and reload mechanics

### UI/UX
- **Modern Crosshair**: Clean, esports-style targeting reticle
- **Health & Armor System**: Visual health bars and armor indicators
- **Mini-Map**: Real-time map with player positions and zone information
- **Kill Feed**: Live updates of player eliminations
- **Inventory Management**: Drag-and-drop item organization

### Multiplayer
- **Online Matchmaking**: Up to 50 players per match
- **Team Communication**: Voice chat integration
- **Server Architecture**: Dedicated server support
- **Anti-Cheat**: Basic cheat prevention systems

### Performance & Graphics
- **Mobile Optimization**: Scalable graphics settings (Low, Medium, High)
- **Performance Monitoring**: Real-time FPS and memory usage
- **Adaptive Quality**: Automatic quality adjustment based on device capabilities
- **Low-Power Mode**: Battery-saving optimizations

## 🚀 Getting Started

### Prerequisites
- Unity 2021.3 LTS or later
- Visual Studio or Visual Studio Code
- Android SDK (for mobile builds)
- Xcode (for iOS builds)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/free-fire-game.git
   cd free-fire-game
   ```

2. **Open in Unity**
   - Launch Unity Hub
   - Click "Add" and select the project folder
   - Open the project with Unity 2021.3 LTS

3. **Configure Build Settings**
   - Go to File > Build Settings
   - Add your target platform (Android/iOS)
   - Configure player settings for your platform

4. **Set up Networking**
   - Install Unity Netcode for GameObjects (NGO)
   - Configure network settings in the NetworkManager

### Project Structure

```
Assets/
├── Scripts/
│   ├── Player/
│   │   ├── PlayerController.cs      # Player movement and controls
│   │   ├── PlayerHealth.cs          # Health and damage system
│   │   └── PlayerInventory.cs       # Inventory management
│   ├── Weapons/
│   │   ├── WeaponSystem.cs          # Weapon mechanics
│   │   └── EnemyHealth.cs           # Enemy health system
│   ├── UI/
│   │   ├── MobileControls.cs        # Mobile UI controls
│   │   └── Joystick.cs              # Virtual joystick
│   ├── Gameplay/
│   │   ├── ZoneSystem.cs            # Safe zone mechanics
│   │   └── LootSystem.cs            # Loot spawning
│   ├── Networking/
│   │   ├── GameManager.cs           # Game state management
│   │   └── NetworkManager.cs        # Network handling
│   └── Performance/
│       └── GraphicsSettings.cs      # Graphics optimization
├── Prefabs/                         # Game object prefabs
├── Materials/                       # Visual materials
├── Textures/                        # Game textures
└── Audio/                          # Sound effects and music
```

## 🎯 Game Modes

### Solo Mode
- 50 players, last player standing wins
- Individual gameplay with no team mechanics
- Focus on survival and combat skills

### Duo Mode
- 25 teams of 2 players each
- Team-based strategy and communication
- Shared inventory and revival mechanics

### Squad Mode
- 12-13 teams of 4 players each
- Advanced team coordination required
- Squad-based objectives and tactics

## 🎮 Controls

### Mobile Controls
- **Left Joystick**: Player movement
- **Right Joystick**: Camera/look control
- **Fire Button**: Shoot weapon
- **Jump Button**: Jump action
- **Crouch Button**: Crouch/prone
- **Reload Button**: Reload weapon
- **Scope Button**: Aim down sights

### Desktop Controls
- **WASD**: Player movement
- **Mouse**: Camera control
- **Left Click**: Fire weapon
- **Space**: Jump
- **Ctrl**: Crouch
- **R**: Reload
- **Right Click**: Aim down sights
- **Shift**: Run

## 🔧 Configuration

### Graphics Settings
- **Quality Levels**: Low, Medium, High, Ultra
- **Render Distance**: Adjustable view distance
- **Shadows**: Enable/disable shadow rendering
- **Anti-Aliasing**: Smooth edge rendering
- **Particle Effects**: Visual effect quality
- **Post-Processing**: Advanced visual effects

### Performance Settings
- **Target Framerate**: 30/60/120 FPS options
- **VSync**: Vertical synchronization
- **Low Power Mode**: Battery optimization
- **Mobile Optimizations**: Automatic mobile tuning

## 🌐 Multiplayer Setup

### Hosting a Server
1. Open the game
2. Click "Host Game"
3. Configure server settings
4. Wait for players to join

### Joining a Game
1. Enter server IP address
2. Click "Join Game"
3. Wait for connection
4. Enter lobby

### Server Requirements
- **Minimum**: 2 CPU cores, 4GB RAM
- **Recommended**: 4 CPU cores, 8GB RAM
- **Network**: Stable internet connection
- **Bandwidth**: 10 Mbps upload for 50 players

## 📱 Mobile Optimization

### Performance Tips
- Use Low/Medium quality settings on older devices
- Enable Low Power Mode for extended battery life
- Close background applications
- Ensure adequate storage space

### Device Requirements
- **Android**: Android 7.0+, 3GB RAM, OpenGL ES 3.0
- **iOS**: iOS 12.0+, 3GB RAM, Metal support
- **Storage**: 2GB free space minimum

## 🛠️ Development

### Adding New Weapons
1. Create weapon prefab in `Assets/Prefabs/Weapons/`
2. Add weapon data to `WeaponSystem.cs`
3. Configure weapon stats and behavior
4. Add to loot system for spawning

### Creating New Maps
1. Design terrain in Unity
2. Add spawn points for players and loot
3. Configure zone system boundaries
4. Test performance and gameplay balance

### Customizing UI
1. Modify UI prefabs in `Assets/Prefabs/UI/`
2. Update `MobileControls.cs` for new elements
3. Adjust button positions and sizes
4. Test on different screen sizes

## 🐛 Troubleshooting

### Common Issues

**Game won't start**
- Check Unity version compatibility
- Verify all required packages are installed
- Check console for error messages

**Network connection fails**
- Verify server IP and port settings
- Check firewall and network configuration
- Ensure server is running and accessible

**Performance issues**
- Lower graphics quality settings
- Enable mobile optimizations
- Close unnecessary background processes
- Check device temperature and battery level

**Controls not responding**
- Verify input system configuration
- Check button assignments in UI
- Test on different devices/platforms

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📞 Support

For support and questions:
- Create an issue on GitHub
- Join our Discord community
- Check the documentation wiki

## 🎉 Acknowledgments

- **Developer**: abhay virus 🔥
- Unity Technologies for the game engine
- Free Fire (Garena) for inspiration
- Open source community for various assets and tools
- Beta testers and contributors

---

**Developed by abhay virus** - A passionate game developer creating amazing battle royale experiences! 🎮🔥

**Note**: This is a fan-made project inspired by Free Fire. It is not affiliated with Garena or Free Fire in any way.
