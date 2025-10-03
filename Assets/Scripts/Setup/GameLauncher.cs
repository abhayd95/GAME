using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FreeFire.Setup;

namespace FreeFire.Setup
{
    public class GameLauncher : MonoBehaviour
    {
        [Header("UI References")]
        public Button playButton;
        public Button settingsButton;
        public Button quitButton;
        public Button testModeButton;
        public Text statusText;

        [Header("Game Settings")]
        public bool enableTestMode = true;
        public string mainSceneName = "MainScene";

        void Start()
        {
            SetupUI();
            UpdateStatus("Game Ready - Click Play to Start");
        }

        void SetupUI()
        {
            if (playButton != null)
            {
                playButton.onClick.AddListener(StartGame);
            }

            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OpenSettings);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(QuitGame);
            }

            if (testModeButton != null)
            {
                testModeButton.onClick.AddListener(StartTestMode);
                testModeButton.gameObject.SetActive(enableTestMode);
            }
        }

        public void StartGame()
        {
            UpdateStatus("Starting Game...");
            
            try
            {
                // Load main scene
                SceneManager.LoadScene(mainSceneName);
            }
            catch (System.Exception e)
            {
                UpdateStatus("Error: " + e.Message);
                Debug.LogError("Failed to start game: " + e.Message);
            }
        }

        public void StartTestMode()
        {
            UpdateStatus("Starting Test Mode...");
            
            try
            {
                // Load main scene
                SceneManager.LoadScene(mainSceneName);
                
                // The GameSetup script will automatically set up the test environment
            }
            catch (System.Exception e)
            {
                UpdateStatus("Error: " + e.Message);
                Debug.LogError("Failed to start test mode: " + e.Message);
            }
        }

        public void OpenSettings()
        {
            UpdateStatus("Opening Settings...");
            
            // This would open a settings menu
            // For now, just log
            Debug.Log("Settings button clicked");
        }

        public void QuitGame()
        {
            UpdateStatus("Quitting Game...");
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        void UpdateStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
            Debug.Log("Status: " + message);
        }

        // Keyboard shortcuts
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                StartGame();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                QuitGame();
            }
            else if (Input.GetKeyDown(KeyCode.T) && enableTestMode)
            {
                StartTestMode();
            }
        }
    }
}
