using UnityEngine;
using UnityEngine.UI;

namespace FreeFire.UI
{
    public class DeveloperCredits : MonoBehaviour
    {
        [Header("Developer Info")]
        public Text developerNameText;
        public Text gameTitleText;
        public Image developerLogo;
        public Button creditsButton;

        [Header("Credits Panel")]
        public GameObject creditsPanel;
        public Text fullCreditsText;
        public Button closeCreditsButton;

        [Header("Settings")]
        public bool showCreditsOnStart = true;
        public float creditsDisplayTime = 3f;

        void Start()
        {
            SetupCredits();
            
            if (showCreditsOnStart)
            {
                ShowDeveloperCredits();
            }
        }

        void SetupCredits()
        {
            // Set developer name
            if (developerNameText != null)
            {
                developerNameText.text = "Developed by abhay virus 🔥";
            }

            // Set game title
            if (gameTitleText != null)
            {
                gameTitleText.text = "Free Fire Clone";
            }

            // Setup credits button
            if (creditsButton != null)
            {
                creditsButton.onClick.AddListener(ShowFullCredits);
            }

            // Setup close button
            if (closeCreditsButton != null)
            {
                closeCreditsButton.onClick.AddListener(HideFullCredits);
            }

            // Hide credits panel initially
            if (creditsPanel != null)
            {
                creditsPanel.SetActive(false);
            }

            // Set full credits text
            if (fullCreditsText != null)
            {
                fullCreditsText.text = GetFullCreditsText();
            }
        }

        void ShowDeveloperCredits()
        {
            // Show developer name briefly
            if (developerNameText != null)
            {
                developerNameText.gameObject.SetActive(true);
                
                // Hide after display time
                Invoke(nameof(HideDeveloperCredits), creditsDisplayTime);
            }
        }

        void HideDeveloperCredits()
        {
            if (developerNameText != null)
            {
                developerNameText.gameObject.SetActive(false);
            }
        }

        void ShowFullCredits()
        {
            if (creditsPanel != null)
            {
                creditsPanel.SetActive(true);
            }
        }

        void HideFullCredits()
        {
            if (creditsPanel != null)
            {
                creditsPanel.SetActive(false);
            }
        }

        string GetFullCreditsText()
        {
            return @"🔥 FREE FIRE CLONE 🔥

Developed by: abhay virus
Version: 1.0.0
Engine: Unity 2021.3 LTS

🎮 GAME FEATURES:
• 3D Battle Royale Gameplay
• Mobile-Optimized Controls
• Multiple Weapon Types
• Health & Damage System
• Loot Spawning System
• Shrinking Zone Mechanics
• Multiplayer Foundation

🛠️ TECHNICAL STACK:
• Unity C# Scripting
• Mobile UI/UX Design
• Network Programming
• Performance Optimization
• Cross-Platform Support

🎯 GAME MODES:
• Solo (50 players)
• Duo (25 teams)
• Squad (12-13 teams)

📱 PLATFORMS:
• Android
• iOS
• Desktop (Windows/Mac)

Special thanks to:
• Unity Technologies
• Free Fire (Garena) for inspiration
• Open source community
• Beta testers

© 2024 abhay virus
All rights reserved.

This is a fan-made project inspired by Free Fire.
Not affiliated with Garena or Free Fire.";
        }

        // Public methods for external access
        public void ShowCredits()
        {
            ShowFullCredits();
        }

        public void HideCredits()
        {
            HideFullCredits();
        }

        public void ToggleCredits()
        {
            if (creditsPanel != null)
            {
                creditsPanel.SetActive(!creditsPanel.activeSelf);
            }
        }

        // Update method for keyboard shortcuts
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                ToggleCredits();
            }
        }
    }
}
