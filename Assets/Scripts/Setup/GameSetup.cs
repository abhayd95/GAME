using UnityEngine;
using UnityEngine.UI;
using FreeFire.Player;
using FreeFire.Weapons;
using FreeFire.Gameplay;
using FreeFire.UI;
using FreeFire.Networking;
using FreeFire.Performance;

namespace FreeFire.Setup
{
    public class GameSetup : MonoBehaviour
    {
        [Header("Setup Options")]
        public bool autoSetupOnStart = true;
        public bool createTestPlayer = true;
        public bool setupUI = true;
        public bool setupNetworking = true;

        [Header("Test Player")]
        public GameObject playerPrefab;
        public Vector3 playerSpawnPosition = new Vector3(0, 1, 0);

        [Header("UI Canvas")]
        public Canvas uiCanvas;

        void Start()
        {
            if (autoSetupOnStart)
            {
                SetupGame();
            }
        }

        [ContextMenu("Setup Game")]
        public void SetupGame()
        {
            Debug.Log("Setting up Free Fire game...");

            if (createTestPlayer)
            {
                CreateTestPlayer();
            }

            if (setupUI)
            {
                SetupUI();
            }

            if (setupNetworking)
            {
                SetupNetworking();
            }

            SetupGameplaySystems();
            Debug.Log("Game setup complete!");
        }

        void CreateTestPlayer()
        {
            GameObject player;
            
            if (playerPrefab != null)
            {
                player = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
            }
            else
            {
                // Create a basic player if no prefab is assigned
                player = CreateBasicPlayer();
            }

            // Add required components
            AddPlayerComponents(player);
            
            Debug.Log("Test player created at: " + playerSpawnPosition);
        }

        GameObject CreateBasicPlayer()
        {
            // Create basic player GameObject
            GameObject player = new GameObject("Player");
            player.transform.position = playerSpawnPosition;
            player.tag = "Player";

            // Add basic components
            player.AddComponent<CharacterController>();
            player.AddComponent<CapsuleCollider>();
            player.AddComponent<Rigidbody>();
            
            // Add visual representation
            GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.transform.SetParent(player.transform);
            visual.transform.localPosition = Vector3.zero;
            visual.name = "PlayerVisual";

            // Remove the collider from visual (we'll use the main one)
            Destroy(visual.GetComponent<Collider>());

            return player;
        }

        void AddPlayerComponents(GameObject player)
        {
            // Add player controller
            if (player.GetComponent<PlayerController>() == null)
            {
                player.AddComponent<PlayerController>();
            }

            // Add health system
            if (player.GetComponent<PlayerHealth>() == null)
            {
                player.AddComponent<PlayerHealth>();
            }

            // Add inventory
            if (player.GetComponent<PlayerInventory>() == null)
            {
                player.AddComponent<PlayerInventory>();
            }

            // Add weapon system
            if (player.GetComponent<WeaponSystem>() == null)
            {
                player.AddComponent<WeaponSystem>();
            }

            // Add camera target
            Transform cameraTarget = player.transform.Find("CameraTarget");
            if (cameraTarget == null)
            {
                GameObject camTarget = new GameObject("CameraTarget");
                camTarget.transform.SetParent(player.transform);
                camTarget.transform.localPosition = new Vector3(0, 1.5f, 0);
            }
        }

        void SetupUI()
        {
            if (uiCanvas == null)
            {
                // Find existing canvas or create one
                uiCanvas = FindObjectOfType<Canvas>();
                if (uiCanvas == null)
                {
                    CreateUICanvas();
                }
            }

            // Add mobile controls
            if (uiCanvas.GetComponent<MobileControls>() == null)
            {
                uiCanvas.gameObject.AddComponent<MobileControls>();
            }

            // Add graphics settings
            if (uiCanvas.GetComponent<GraphicsSettings>() == null)
            {
                uiCanvas.gameObject.AddComponent<GraphicsSettings>();
            }

            // Add developer credits
            if (uiCanvas.GetComponent<DeveloperCredits>() == null)
            {
                uiCanvas.gameObject.AddComponent<DeveloperCredits>();
            }

            Debug.Log("UI setup complete - Developed by abhay virus 🔥");
        }

        void CreateUICanvas()
        {
            // Create UI Canvas
            GameObject canvasGO = new GameObject("UI Canvas");
            uiCanvas = canvasGO.AddComponent<Canvas>();
            uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            uiCanvas.sortingOrder = 100;

            // Add Canvas Scaler
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            // Add Graphic Raycaster
            canvasGO.AddComponent<GraphicRaycaster>();

            // Create Event System if it doesn't exist
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            Debug.Log("UI Canvas created");
        }

        void SetupNetworking()
        {
            // Add NetworkManager if it doesn't exist
            if (FindObjectOfType<NetworkManager>() == null)
            {
                GameObject networkManager = new GameObject("NetworkManager");
                networkManager.AddComponent<NetworkManager>();
            }

            // Add GameManager if it doesn't exist
            if (FindObjectOfType<GameManager>() == null)
            {
                GameObject gameManager = new GameObject("GameManager");
                gameManager.AddComponent<GameManager>();
            }

            // Add LocalhostServer if it doesn't exist
            if (FindObjectOfType<LocalhostServer>() == null)
            {
                GameObject localhostServer = new GameObject("LocalhostServer");
                localhostServer.AddComponent<LocalhostServer>();
            }

            Debug.Log("Networking setup complete - Localhost ready by abhay virus 🔥");
        }

        [ContextMenu("Setup Localhost")]
        public static void SetupLocalhost()
        {
            Debug.Log("🔥 Setting up localhost server by abhay virus 🔥");
            
            // This method can be called from Unity command line
            var gameSetup = FindObjectOfType<GameSetup>();
            if (gameSetup != null)
            {
                gameSetup.SetupGame();
            }
        }

        void SetupGameplaySystems()
        {
            // Add Zone System
            if (FindObjectOfType<ZoneSystem>() == null)
            {
                GameObject zoneSystem = new GameObject("ZoneSystem");
                zoneSystem.AddComponent<ZoneSystem>();
            }

            // Add Loot System
            if (FindObjectOfType<LootSystem>() == null)
            {
                GameObject lootSystem = new GameObject("LootSystem");
                lootSystem.AddComponent<LootSystem>();
            }

            // Add Audio Manager
            if (FindObjectOfType<AudioManager>() == null)
            {
                GameObject audioManager = new GameObject("AudioManager");
                audioManager.AddComponent<AudioManager>();
            }

            Debug.Log("Gameplay systems setup complete");
        }

        [ContextMenu("Create Test Environment")]
        public void CreateTestEnvironment()
        {
            // Create ground
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(10, 1, 10);

            // Create some obstacles
            for (int i = 0; i < 5; i++)
            {
                GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obstacle.name = "Obstacle_" + i;
                obstacle.transform.position = new Vector3(
                    Random.Range(-20, 20),
                    1,
                    Random.Range(-20, 20)
                );
                obstacle.transform.localScale = new Vector3(
                    Random.Range(1, 3),
                    Random.Range(1, 4),
                    Random.Range(1, 3)
                );
            }

            // Create spawn points
            GameObject spawnPoints = new GameObject("SpawnPoints");
            for (int i = 0; i < 10; i++)
            {
                GameObject spawnPoint = new GameObject("SpawnPoint_" + i);
                spawnPoint.transform.SetParent(spawnPoints.transform);
                spawnPoint.transform.position = new Vector3(
                    Random.Range(-30, 30),
                    0.5f,
                    Random.Range(-30, 30)
                );
                
                // Add visual indicator
                GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                indicator.transform.SetParent(spawnPoint.transform);
                indicator.transform.localPosition = Vector3.zero;
                indicator.transform.localScale = new Vector3(0.5f, 0.1f, 0.5f);
                indicator.GetComponent<Renderer>().material.color = Color.green;
            }

            Debug.Log("Test environment created");
        }

        [ContextMenu("Reset Game")]
        public void ResetGame()
        {
            // Find and destroy existing players
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                DestroyImmediate(player);
            }

            // Reset camera
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.transform.position = new Vector3(0, 1, -10);
                mainCam.transform.rotation = Quaternion.identity;
            }

            Debug.Log("Game reset complete");
        }
    }
}
