# 🌐 Free Fire Game - Localhost Server Guide

**Developed by abhay virus** 🔥

---

## 🚀 **Quick Start - Run on Localhost**

### **Method 1: Using the Localhost Script**
```bash
./start_localhost.sh
```

### **Method 2: Manual Setup**
1. **Install Unity** from [unity.com](https://unity.com)
2. **Open Unity Hub** → **"Projects"** → **"Add"**
3. **Select your folder**: `/Users/abhaytiwari/Desktop/free fire`
4. **Open the project**
5. **Add GameSetup script** to an empty GameObject
6. **Click Play** ▶️

---

## 🌐 **Localhost Configuration**

### **Default Settings:**
- **Server Port**: 7777
- **Client Port**: 7778
- **Local IP**: 127.0.0.1
- **Max Connections**: 50

### **Access Your Game:**
- **URL**: http://localhost:7777
- **Local IP**: http://127.0.0.1:7777

---

## 🎮 **How to Run on Localhost**

### **Step 1: Start the Server**
1. **Open Unity** with your project
2. **Add LocalhostServer script** to an empty GameObject
3. **Click "Start Server"** button
4. **Server will start** on port 7777

### **Step 2: Connect Clients**
1. **Open multiple Unity instances** (or build executables)
2. **Connect to localhost:7777**
3. **Start playing** with multiple players

### **Step 3: Test Multiplayer**
- **Host**: Runs the server
- **Clients**: Connect to localhost:7777
- **Up to 50 players** can connect

---

## 🛠️ **Localhost Features**

### **Server Management:**
- ✅ **Start/Stop Server** buttons
- ✅ **Port Configuration** (customizable)
- ✅ **Connection Monitoring** (client count)
- ✅ **Server Status** display
- ✅ **Keyboard Shortcuts** (F2 to toggle)

### **Network Features:**
- ✅ **Real-time Multiplayer**
- ✅ **Player Synchronization**
- ✅ **Game State Management**
- ✅ **Chat System** (ready for implementation)
- ✅ **Lobby System**

---

## 🎯 **Controls for Localhost**

### **Server Controls:**
- **F2** - Start/Stop server
- **Start Server Button** - Start localhost server
- **Stop Server Button** - Stop localhost server
- **Port Input** - Change server port

### **Game Controls:**
- **WASD** - Move player
- **Mouse** - Look around
- **Left Click** - Shoot
- **Space** - Jump
- **R** - Reload

---

## 📱 **Testing on Different Devices**

### **Same Computer:**
1. **Start server** in Unity
2. **Build executable** (File → Build Settings)
3. **Run executable** to connect as client
4. **Test multiplayer** locally

### **Local Network:**
1. **Find your local IP** (192.168.x.x)
2. **Start server** on main computer
3. **Connect from other devices** using your local IP
4. **Test cross-device multiplayer**

---

## 🔧 **Troubleshooting**

### **Server Won't Start:**
- Check if port 7777 is available
- Try different port number
- Restart Unity
- Check firewall settings

### **Can't Connect:**
- Verify server is running
- Check IP address and port
- Ensure firewall allows connections
- Try localhost:7777

### **Performance Issues:**
- Reduce max connections
- Lower graphics quality
- Close unnecessary programs
- Check system resources

---

## 🎮 **Localhost Game Features**

### **Multiplayer Ready:**
- ✅ **50 Player Support**
- ✅ **Real-time Synchronization**
- ✅ **Battle Royale Mechanics**
- ✅ **Zone System** (shrinking safe areas)
- ✅ **Loot System** (weapons, health, armor)
- ✅ **Weapon System** (shooting, damage)
- ✅ **Health System** (damage, healing)

### **Mobile Support:**
- ✅ **Touch Controls** (joysticks, buttons)
- ✅ **Cross-Platform** (Android, iOS, Desktop)
- ✅ **Responsive UI** (adapts to screen size)
- ✅ **Performance Optimization**

---

## 🚀 **Advanced Localhost Setup**

### **Custom Port Configuration:**
```csharp
// In LocalhostServer.cs
public int serverPort = 7777;  // Change this
public int clientPort = 7778;  // Change this
```

### **Environment Variables:**
```bash
export UNITY_SERVER_PORT=7777
export UNITY_CLIENT_PORT=7778
```

### **Command Line Launch:**
```bash
unity -projectPath "/path/to/project" -executeMethod FreeFire.Setup.GameSetup.SetupLocalhost
```

---

## 📊 **Server Monitoring**

### **Real-time Stats:**
- **Connected Clients**: Live count
- **Server Status**: Running/Stopped
- **Port Information**: Current port
- **Performance**: FPS, memory usage

### **Logs:**
- **Server Events**: Start/stop, connections
- **Client Events**: Join/leave, actions
- **Error Logs**: Connection issues, errors

---

## 🔥 **Developer Information**

**Developer**: abhay virus  
**Game**: Free Fire Clone  
**Version**: 1.0.0  
**Copyright**: © 2024 abhay virus. All rights reserved.  

---

## 🎉 **Ready to Play!**

Your Free Fire game is now ready to run on localhost! 

**Start the server and invite friends to play on your local network!** 🎮🔥

---

**© 2024 abhay virus. All rights reserved.**
