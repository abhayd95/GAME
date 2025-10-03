/*
 * Copyright (c) 2024 abhay virus. All rights reserved.
 * 
 * This file is part of the Free Fire Clone game.
 * No part of this software may be reproduced, distributed, or transmitted
 * without the prior written permission of the copyright owner.
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace FreeFire.Networking
{
    public class LocalhostServer : MonoBehaviour
    {
        [Header("Localhost Settings")]
        public int serverPort = 7777;
        public int clientPort = 7778;
        public string localhostIP = "127.0.0.1";
        public int maxConnections = 50;

        [Header("UI References")]
        public Text serverStatusText;
        public Text connectionInfoText;
        public Button startServerButton;
        public Button stopServerButton;
        public InputField portInput;

        [Header("Server Info")]
        public bool isServerRunning = false;
        public int connectedClients = 0;

        // Events
        public System.Action OnServerStarted;
        public System.Action OnServerStopped;
        public System.Action<int> OnClientConnected;
        public System.Action<int> OnClientDisconnected;

        void Start()
        {
            SetupUI();
            LoadLocalhostSettings();
        }

        void SetupUI()
        {
            if (portInput != null)
            {
                portInput.text = serverPort.ToString();
                portInput.onValueChanged.AddListener(OnPortChanged);
            }

            if (startServerButton != null)
            {
                startServerButton.onClick.AddListener(StartLocalhostServer);
            }

            if (stopServerButton != null)
            {
                stopServerButton.onClick.AddListener(StopLocalhostServer);
            }

            UpdateUI();
        }

        void LoadLocalhostSettings()
        {
            // Load from environment variables if available
            if (System.Environment.GetEnvironmentVariable("UNITY_SERVER_PORT") != null)
            {
                int.TryParse(System.Environment.GetEnvironmentVariable("UNITY_SERVER_PORT"), out serverPort);
            }

            if (System.Environment.GetEnvironmentVariable("UNITY_CLIENT_PORT") != null)
            {
                int.TryParse(System.Environment.GetEnvironmentVariable("UNITY_CLIENT_PORT"), out clientPort);
            }

            // Load from PlayerPrefs
            serverPort = PlayerPrefs.GetInt("LocalhostServerPort", 7777);
            clientPort = PlayerPrefs.GetInt("LocalhostClientPort", 7778);

            UpdateUI();
        }

        void SaveLocalhostSettings()
        {
            PlayerPrefs.SetInt("LocalhostServerPort", serverPort);
            PlayerPrefs.SetInt("LocalhostClientPort", clientPort);
            PlayerPrefs.Save();
        }

        public void StartLocalhostServer()
        {
            if (isServerRunning) return;

            try
            {
                // Configure NetworkManager for localhost
                var networkManager = FindObjectOfType<NetworkManager>();
                if (networkManager != null)
                {
                    networkManager.networkPort = serverPort;
                    networkManager.maxConnections = maxConnections;
                }

                // Start server
                bool success = NetworkManager.singleton.StartHost();
                
                if (success)
                {
                    isServerRunning = true;
                    connectedClients = 1; // Host counts as one client
                    
                    SaveLocalhostSettings();
                    OnServerStarted?.Invoke();
                    
                    Debug.Log($"🔥 Localhost server started by abhay virus on port {serverPort}");
                    Debug.Log($"🌐 Server accessible at: http://{localhostIP}:{serverPort}");
                }
                else
                {
                    Debug.LogError("Failed to start localhost server");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error starting localhost server: {e.Message}");
            }

            UpdateUI();
        }

        public void StopLocalhostServer()
        {
            if (!isServerRunning) return;

            try
            {
                NetworkManager.singleton.StopHost();
                isServerRunning = false;
                connectedClients = 0;
                
                OnServerStopped?.Invoke();
                Debug.Log("Localhost server stopped");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error stopping localhost server: {e.Message}");
            }

            UpdateUI();
        }

        void OnPortChanged(string newPort)
        {
            if (int.TryParse(newPort, out int port))
            {
                serverPort = port;
                SaveLocalhostSettings();
            }
        }

        void UpdateUI()
        {
            if (serverStatusText != null)
            {
                if (isServerRunning)
                {
                    serverStatusText.text = $"🔥 Server Running - abhay virus";
                    serverStatusText.color = Color.green;
                }
                else
                {
                    serverStatusText.text = "Server Stopped";
                    serverStatusText.color = Color.red;
                }
            }

            if (connectionInfoText != null)
            {
                connectionInfoText.text = $"Port: {serverPort} | Clients: {connectedClients}/{maxConnections}";
            }

            if (startServerButton != null)
            {
                startServerButton.interactable = !isServerRunning;
            }

            if (stopServerButton != null)
            {
                stopServerButton.interactable = isServerRunning;
            }
        }

        // Network callbacks
        void OnClientConnect()
        {
            connectedClients++;
            OnClientConnected?.Invoke(connectedClients);
            UpdateUI();
            Debug.Log($"Client connected. Total clients: {connectedClients}");
        }

        void OnClientDisconnect()
        {
            connectedClients = Mathf.Max(0, connectedClients - 1);
            OnClientDisconnected?.Invoke(connectedClients);
            UpdateUI();
            Debug.Log($"Client disconnected. Total clients: {connectedClients}");
        }

        // Public getters
        public bool IsServerRunning => isServerRunning;
        public int ServerPort => serverPort;
        public int ClientPort => clientPort;
        public string LocalhostIP => localhostIP;
        public int ConnectedClients => connectedClients;
        public int MaxConnections => maxConnections;

        // Utility methods
        public string GetServerURL()
        {
            return $"http://{localhostIP}:{serverPort}";
        }

        public string GetConnectionInfo()
        {
            return $"Server: {localhostIP}:{serverPort} | Clients: {connectedClients}/{maxConnections}";
        }

        // Keyboard shortcuts
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F2))
            {
                if (isServerRunning)
                    StopLocalhostServer();
                else
                    StartLocalhostServer();
            }
        }
    }
}
