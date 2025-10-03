using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;

namespace FreeFire.Networking
{
    public class GameManager : NetworkBehaviour
    {
        [Header("Game Settings")]
        public int maxPlayers = 50;
        public float gameStartDelay = 10f;
        public float gameEndDelay = 5f;
        public GameMode currentGameMode = GameMode.Solo;

        [Header("UI References")]
        public GameObject lobbyUI;
        public GameObject gameUI;
        public GameObject endGameUI;
        public Text playerCountText;
        public Text gameTimerText;
        public Text winnerText;
        public Button startGameButton;

        [Header("Spawn Settings")]
        public Transform[] spawnPoints;
        public GameObject playerPrefab;
        public float spawnRadius = 50f;

        // Game state
        public enum GameState
        {
            Lobby,
            Starting,
            Playing,
            Ending
        }

        public enum GameMode
        {
            Solo,
            Duo,
            Squad
        }

        [SyncVar] public GameState currentState = GameState.Lobby;
        [SyncVar] public float gameTime = 0f;
        [SyncVar] public int playersAlive = 0;
        [SyncVar] public int totalPlayers = 0;

        // Player management
        private List<GameObject> players = new List<GameObject>();
        private List<GameObject> alivePlayers = new List<GameObject>();
        private Dictionary<GameObject, int> playerKills = new Dictionary<GameObject, int>();

        // Events
        public System.Action<GameState> OnGameStateChanged;
        public System.Action<GameObject> OnPlayerJoined;
        public System.Action<GameObject> OnPlayerLeft;
        public System.Action<GameObject, GameObject> OnPlayerKilled;

        void Start()
        {
            if (isServer)
            {
                InitializeGame();
            }
            UpdateUI();
        }

        void Update()
        {
            if (isServer)
            {
                UpdateGameState();
            }
            UpdateUI();
        }

        [Server]
        void InitializeGame()
        {
            currentState = GameState.Lobby;
            gameTime = 0f;
            playersAlive = 0;
            totalPlayers = 0;
            
            // Generate spawn points if none exist
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                GenerateSpawnPoints();
            }
        }

        [Server]
        void UpdateGameState()
        {
            switch (currentState)
            {
                case GameState.Lobby:
                    HandleLobbyState();
                    break;
                case GameState.Starting:
                    HandleStartingState();
                    break;
                case GameState.Playing:
                    HandlePlayingState();
                    break;
                case GameState.Ending:
                    HandleEndingState();
                    break;
            }
        }

        [Server]
        void HandleLobbyState()
        {
            // Check if we have enough players to start
            if (totalPlayers >= GetMinimumPlayers())
            {
                // Auto-start after delay
                if (gameTime >= gameStartDelay)
                {
                    StartGame();
                }
            }
            else
            {
                gameTime = 0f; // Reset timer if not enough players
            }

            gameTime += Time.deltaTime;
        }

        [Server]
        void HandleStartingState()
        {
            gameTime += Time.deltaTime;
            
            // Start game after countdown
            if (gameTime >= 5f) // 5 second countdown
            {
                currentState = GameState.Playing;
                gameTime = 0f;
                RpcStartGame();
            }
        }

        [Server]
        void HandlePlayingState()
        {
            gameTime += Time.deltaTime;
            
            // Check win conditions
            CheckWinConditions();
        }

        [Server]
        void HandleEndingState()
        {
            gameTime += Time.deltaTime;
            
            // Restart game after delay
            if (gameTime >= gameEndDelay)
            {
                RestartGame();
            }
        }

        [Server]
        void CheckWinConditions()
        {
            int aliveCount = 0;
            GameObject lastPlayer = null;

            foreach (GameObject player in players)
            {
                if (player != null)
                {
                    var playerHealth = player.GetComponent<PlayerHealth>();
                    if (playerHealth != null && !playerHealth.IsDead)
                    {
                        aliveCount++;
                        lastPlayer = player;
                    }
                }
            }

            playersAlive = aliveCount;

            // Check win conditions based on game mode
            bool gameEnded = false;
            switch (currentGameMode)
            {
                case GameMode.Solo:
                    gameEnded = aliveCount <= 1;
                    break;
                case GameMode.Duo:
                    gameEnded = aliveCount <= 2;
                    break;
                case GameMode.Squad:
                    gameEnded = aliveCount <= 4;
                    break;
            }

            if (gameEnded && aliveCount > 0)
            {
                EndGame(lastPlayer);
            }
        }

        [Server]
        public void StartGame()
        {
            if (currentState != GameState.Lobby) return;

            currentState = GameState.Starting;
            gameTime = 0f;
            RpcGameStarting();
        }

        [Server]
        public void EndGame(GameObject winner)
        {
            if (currentState != GameState.Playing) return;

            currentState = GameState.Ending;
            gameTime = 0f;
            RpcEndGame(winner.name);
        }

        [Server]
        public void RestartGame()
        {
            // Reset all players
            foreach (GameObject player in players)
            {
                if (player != null)
                {
                    var playerHealth = player.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        playerHealth.Revive(100f);
                    }
                    
                    // Respawn player
                    RespawnPlayer(player);
                }
            }

            // Reset game state
            currentState = GameState.Lobby;
            gameTime = 0f;
            playersAlive = totalPlayers;
            playerKills.Clear();

            RpcRestartGame();
        }

        [Server]
        public void OnPlayerJoined(GameObject player)
        {
            if (!players.Contains(player))
            {
                players.Add(player);
                totalPlayers++;
                playersAlive++;
                playerKills[player] = 0;

                // Spawn player
                RespawnPlayer(player);

                OnPlayerJoined?.Invoke(player);
                RpcUpdatePlayerCount(totalPlayers, playersAlive);
            }
        }

        [Server]
        public void OnPlayerLeft(GameObject player)
        {
            if (players.Contains(player))
            {
                players.Remove(player);
                totalPlayers--;
                
                if (alivePlayers.Contains(player))
                {
                    alivePlayers.Remove(player);
                    playersAlive--;
                }

                playerKills.Remove(player);

                OnPlayerLeft?.Invoke(player);
                RpcUpdatePlayerCount(totalPlayers, playersAlive);
            }
        }

        [Server]
        public void OnPlayerDeath(GameObject player, GameObject killer)
        {
            if (alivePlayers.Contains(player))
            {
                alivePlayers.Remove(player);
                playersAlive--;

                // Award kill to killer
                if (killer != null && killer != player && playerKills.ContainsKey(killer))
                {
                    playerKills[killer]++;
                }

                OnPlayerKilled?.Invoke(player, killer);
                RpcUpdatePlayerCount(totalPlayers, playersAlive);
                RpcPlayerKilled(player.name, killer != null ? killer.name : "Zone");
            }
        }

        [Server]
        void RespawnPlayer(GameObject player)
        {
            if (player == null) return;

            // Get random spawn point
            Vector3 spawnPosition = GetRandomSpawnPosition();
            
            // Move player to spawn position
            player.transform.position = spawnPosition;
            player.transform.rotation = Quaternion.identity;

            // Reset player state
            var playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Revive(100f);
            }

            var playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = true;
            }

            var weaponSystem = player.GetComponent<WeaponSystem>();
            if (weaponSystem != null)
            {
                weaponSystem.enabled = true;
            }
        }

        Vector3 GetRandomSpawnPosition()
        {
            if (spawnPoints != null && spawnPoints.Length > 0)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
                randomOffset.y = 0; // Keep on ground level
                return spawnPoint.position + randomOffset;
            }
            else
            {
                // Generate random position
                float mapSize = 400f;
                return new Vector3(
                    Random.Range(-mapSize, mapSize),
                    0f,
                    Random.Range(-mapSize, mapSize)
                );
            }
        }

        void GenerateSpawnPoints()
        {
            List<Transform> points = new List<Transform>();
            float mapSize = 400f;
            int pointCount = 20;

            for (int i = 0; i < pointCount; i++)
            {
                GameObject spawnPoint = new GameObject($"SpawnPoint_{i}");
                spawnPoint.transform.position = new Vector3(
                    Random.Range(-mapSize, mapSize),
                    0f,
                    Random.Range(-mapSize, mapSize)
                );
                points.Add(spawnPoint.transform);
            }

            spawnPoints = points.ToArray();
        }

        int GetMinimumPlayers()
        {
            switch (currentGameMode)
            {
                case GameMode.Solo:
                    return 2;
                case GameMode.Duo:
                    return 4;
                case GameMode.Squad:
                    return 8;
                default:
                    return 2;
            }
        }

        void UpdateUI()
        {
            if (playerCountText != null)
            {
                playerCountText.text = $"Players: {totalPlayers} ({playersAlive} alive)";
            }

            if (gameTimerText != null)
            {
                int minutes = Mathf.FloorToInt(gameTime / 60f);
                int seconds = Mathf.FloorToInt(gameTime % 60f);
                gameTimerText.text = $"{minutes:00}:{seconds:00}";
            }

            // Update UI panels based on game state
            if (lobbyUI != null)
            {
                lobbyUI.SetActive(currentState == GameState.Lobby);
            }

            if (gameUI != null)
            {
                gameUI.SetActive(currentState == GameState.Playing || currentState == GameState.Starting);
            }

            if (endGameUI != null)
            {
                endGameUI.SetActive(currentState == GameState.Ending);
            }

            if (startGameButton != null)
            {
                startGameButton.gameObject.SetActive(isServer && currentState == GameState.Lobby);
            }
        }

        // RPC Methods
        [ClientRpc]
        void RpcGameStarting()
        {
            // Show countdown UI
            Debug.Log("Game starting in 5 seconds...");
        }

        [ClientRpc]
        void RpcStartGame()
        {
            Debug.Log("Game started!");
            OnGameStateChanged?.Invoke(GameState.Playing);
        }

        [ClientRpc]
        void RpcEndGame(string winnerName)
        {
            if (winnerText != null)
            {
                winnerText.text = $"Winner: {winnerName}";
            }
            Debug.Log($"Game ended! Winner: {winnerName}");
            OnGameStateChanged?.Invoke(GameState.Ending);
        }

        [ClientRpc]
        void RpcRestartGame()
        {
            Debug.Log("Game restarted!");
            OnGameStateChanged?.Invoke(GameState.Lobby);
        }

        [ClientRpc]
        void RpcUpdatePlayerCount(int total, int alive)
        {
            totalPlayers = total;
            playersAlive = alive;
        }

        [ClientRpc]
        void RpcPlayerKilled(string playerName, string killerName)
        {
            Debug.Log($"{playerName} was killed by {killerName}");
        }

        // Public methods for UI buttons
        public void OnStartGameButtonClicked()
        {
            if (isServer)
            {
                StartGame();
            }
        }

        public void OnLeaveGameButtonClicked()
        {
            // Disconnect from server
            if (NetworkManager.singleton != null)
            {
                NetworkManager.singleton.StopHost();
            }
        }

        // Public getters
        public GameState GetCurrentState() => currentState;
        public GameMode GetCurrentGameMode() => currentGameMode;
        public int GetPlayerCount() => totalPlayers;
        public int GetAlivePlayerCount() => playersAlive;
        public float GetGameTime() => gameTime;
        public Dictionary<GameObject, int> GetPlayerKills() => playerKills;
    }
}
