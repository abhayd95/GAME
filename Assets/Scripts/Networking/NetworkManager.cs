using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace FreeFire.Networking
{
    public class NetworkManager : MonoBehaviour
    {
        [Header("Network Settings")]
        public int maxConnections = 50;
        public int port = 7777;
        public string serverIP = "localhost";

        [Header("UI References")]
        public InputField serverIPInput;
        public InputField portInput;
        public Button hostButton;
        public Button clientButton;
        public Button disconnectButton;
        public Text connectionStatusText;
        public GameObject lobbyUI;
        public GameObject gameUI;

        [Header("Prefabs")]
        public GameObject playerPrefab;

        // Network state
        private bool isHost = false;
        private bool isClient = false;
        private bool isConnected = false;

        void Start()
        {
            SetupUI();
            InitializeNetwork();
        }

        void SetupUI()
        {
            if (serverIPInput != null)
            {
                serverIPInput.text = serverIP;
                serverIPInput.onValueChanged.AddListener(OnServerIPChanged);
            }

            if (portInput != null)
            {
                portInput.text = port.ToString();
                portInput.onValueChanged.AddListener(OnPortChanged);
            }

            if (hostButton != null)
            {
                hostButton.onClick.AddListener(StartHost);
            }

            if (clientButton != null)
            {
                clientButton.onClick.AddListener(StartClient);
            }

            if (disconnectButton != null)
            {
                disconnectButton.onClick.AddListener(Disconnect);
            }

            UpdateUI();
        }

        void InitializeNetwork()
        {
            // Configure network settings
            NetworkManager.singleton.maxConnections = maxConnections;
            NetworkManager.singleton.networkPort = port;
            
            // Register player prefab
            if (playerPrefab != null)
            {
                ClientScene.RegisterPrefab(playerPrefab);
            }

            // Setup callbacks
            NetworkManager.singleton.client.RegisterHandler(MsgType.Connect, OnClientConnect);
            NetworkManager.singleton.client.RegisterHandler(MsgType.Disconnect, OnClientDisconnect);
            NetworkManager.singleton.client.RegisterHandler(MsgType.Error, OnClientError);
        }

        void OnServerIPChanged(string newIP)
        {
            serverIP = newIP;
        }

        void OnPortChanged(string newPort)
        {
            if (int.TryParse(newPort, out int portValue))
            {
                port = portValue;
            }
        }

        public void StartHost()
        {
            if (isConnected) return;

            try
            {
                NetworkManager.singleton.networkPort = port;
                bool success = NetworkManager.singleton.StartHost();
                
                if (success)
                {
                    isHost = true;
                    isConnected = true;
                    Debug.Log("Host started successfully");
                }
                else
                {
                    Debug.LogError("Failed to start host");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error starting host: " + e.Message);
            }

            UpdateUI();
        }

        public void StartClient()
        {
            if (isConnected) return;

            try
            {
                NetworkManager.singleton.networkAddress = serverIP;
                NetworkManager.singleton.networkPort = port;
                bool success = NetworkManager.singleton.StartClient();
                
                if (success)
                {
                    isClient = true;
                    Debug.Log("Client started successfully");
                }
                else
                {
                    Debug.LogError("Failed to start client");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error starting client: " + e.Message);
            }

            UpdateUI();
        }

        public void Disconnect()
        {
            if (!isConnected) return;

            if (isHost)
            {
                NetworkManager.singleton.StopHost();
            }
            else if (isClient)
            {
                NetworkManager.singleton.StopClient();
            }

            isHost = false;
            isClient = false;
            isConnected = false;

            UpdateUI();
        }

        void OnClientConnect(NetworkMessage netMsg)
        {
            isConnected = true;
            Debug.Log("Connected to server");
            UpdateUI();
        }

        void OnClientDisconnect(NetworkMessage netMsg)
        {
            isConnected = false;
            isClient = false;
            Debug.Log("Disconnected from server");
            UpdateUI();
        }

        void OnClientError(NetworkMessage netMsg)
        {
            Debug.LogError("Network error occurred");
            isConnected = false;
            isClient = false;
            UpdateUI();
        }

        void UpdateUI()
        {
            if (connectionStatusText != null)
            {
                if (isConnected)
                {
                    if (isHost)
                    {
                        connectionStatusText.text = "Host - Connected";
                        connectionStatusText.color = Color.green;
                    }
                    else if (isClient)
                    {
                        connectionStatusText.text = "Client - Connected";
                        connectionStatusText.color = Color.blue;
                    }
                }
                else
                {
                    connectionStatusText.text = "Not Connected";
                    connectionStatusText.color = Color.red;
                }
            }

            // Update button states
            if (hostButton != null)
            {
                hostButton.interactable = !isConnected;
            }

            if (clientButton != null)
            {
                clientButton.interactable = !isConnected;
            }

            if (disconnectButton != null)
            {
                disconnectButton.interactable = isConnected;
            }

            // Update UI panels
            if (lobbyUI != null)
            {
                lobbyUI.SetActive(!isConnected);
            }

            if (gameUI != null)
            {
                gameUI.SetActive(isConnected);
            }
        }

        // Public getters
        public bool IsHost => isHost;
        public bool IsClient => isClient;
        public bool IsConnected => isConnected;
        public string ServerIP => serverIP;
        public int Port => port;
    }
}
