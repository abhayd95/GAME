using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace FreeFire.UI
{
    public class MobileControls : MonoBehaviour
    {
        [Header("Movement Controls")]
        public Joystick movementJoystick;
        public Joystick lookJoystick;

        [Header("Action Buttons")]
        public Button fireButton;
        public Button jumpButton;
        public Button crouchButton;
        public Button reloadButton;
        public Button scopeButton;

        [Header("UI Panels")]
        public GameObject controlsPanel;
        public GameObject inventoryPanel;
        public GameObject settingsPanel;

        [Header("Crosshair")]
        public Image crosshair;
        public RectTransform crosshairRect;

        [Header("Health & Ammo")]
        public Slider healthBar;
        public Text ammoText;
        public Text healthText;

        [Header("Mini Map")]
        public RawImage miniMap;
        public Camera miniMapCamera;

        [Header("Settings")]
        public bool allowButtonResize = true;
        public bool allowButtonReposition = true;

        // Private variables
        private PlayerController playerController;
        private WeaponSystem weaponSystem;
        private bool isDragging = false;
        private Button draggedButton;
        private Vector2 originalButtonPosition;

        void Start()
        {
            SetupControls();
            SetupEventListeners();
        }

        void SetupControls()
        {
            // Find player controller and weapon system
            playerController = FindObjectOfType<PlayerController>();
            weaponSystem = FindObjectOfType<WeaponSystem>();

            // Setup joysticks
            if (movementJoystick != null)
            {
                movementJoystick.OnPointerDown += OnMovementJoystickDown;
                movementJoystick.OnPointerUp += OnMovementJoystickUp;
            }

            if (lookJoystick != null)
            {
                lookJoystick.OnPointerDown += OnLookJoystickDown;
                lookJoystick.OnPointerUp += OnLookJoystickUp;
            }

            // Setup buttons
            if (fireButton != null)
            {
                fireButton.onClick.AddListener(OnFireButtonClicked);
                SetupDraggableButton(fireButton);
            }

            if (jumpButton != null)
            {
                jumpButton.onClick.AddListener(OnJumpButtonClicked);
                SetupDraggableButton(jumpButton);
            }

            if (crouchButton != null)
            {
                crouchButton.onClick.AddListener(OnCrouchButtonClicked);
                SetupDraggableButton(crouchButton);
            }

            if (reloadButton != null)
            {
                reloadButton.onClick.AddListener(OnReloadButtonClicked);
                SetupDraggableButton(reloadButton);
            }

            if (scopeButton != null)
            {
                scopeButton.onClick.AddListener(OnScopeButtonClicked);
                SetupDraggableButton(scopeButton);
            }

            // Setup crosshair
            if (crosshair != null)
            {
                crosshairRect = crosshair.GetComponent<RectTransform>();
            }
        }

        void SetupEventListeners()
        {
            // Add event listeners for UI updates
            if (playerController != null)
            {
                // Health updates would be handled by player health system
            }

            if (weaponSystem != null)
            {
                // Ammo updates would be handled by weapon system
            }
        }

        void SetupDraggableButton(Button button)
        {
            if (!allowButtonReposition) return;

            EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = button.gameObject.AddComponent<EventTrigger>();
            }

            // Drag events
            EventTrigger.Entry dragEntry = new EventTrigger.Entry();
            dragEntry.eventID = EventTriggerType.BeginDrag;
            dragEntry.callback.AddListener((data) => { OnButtonDragStart(button); });
            trigger.triggers.Add(dragEntry);

            EventTrigger.Entry dragEntry2 = new EventTrigger.Entry();
            dragEntry2.eventID = EventTriggerType.Drag;
            dragEntry2.callback.AddListener((data) => { OnButtonDrag(button, (PointerEventData)data); });
            trigger.triggers.Add(dragEntry2);

            EventTrigger.Entry dragEntry3 = new EventTrigger.Entry();
            dragEntry3.eventID = EventTriggerType.EndDrag;
            dragEntry3.callback.AddListener((data) => { OnButtonDragEnd(button); });
            trigger.triggers.Add(dragEntry3);
        }

        void Update()
        {
            HandleLookInput();
            UpdateUI();
        }

        void HandleLookInput()
        {
            if (lookJoystick != null && playerController != null)
            {
                Vector2 lookInput = lookJoystick.Direction;
                playerController.SetLookInput(lookInput);
            }
        }

        void UpdateUI()
        {
            // Update crosshair position based on look input
            if (crosshairRect != null && lookJoystick != null)
            {
                Vector2 lookInput = lookJoystick.Direction;
                Vector2 crosshairOffset = lookInput * 20f; // Adjust sensitivity
                crosshairRect.anchoredPosition = crosshairOffset;
            }
        }

        // Joystick Events
        void OnMovementJoystickDown()
        {
            // Movement is handled by PlayerController
        }

        void OnMovementJoystickUp()
        {
            // Movement is handled by PlayerController
        }

        void OnLookJoystickDown()
        {
            // Look input is handled in Update
        }

        void OnLookJoystickUp()
        {
            // Look input is handled in Update
        }

        // Button Events
        void OnFireButtonClicked()
        {
            if (weaponSystem != null)
            {
                weaponSystem.OnFireButtonPressed();
            }
        }

        void OnJumpButtonClicked()
        {
            // Jump is handled by PlayerController
        }

        void OnCrouchButtonClicked()
        {
            // Crouch is handled by PlayerController
        }

        void OnReloadButtonClicked()
        {
            if (weaponSystem != null)
            {
                weaponSystem.OnReloadButtonPressed();
            }
        }

        void OnScopeButtonClicked()
        {
            // Scope functionality would be implemented here
            ToggleScope();
        }

        // Drag and Drop for Button Repositioning
        void OnButtonDragStart(Button button)
        {
            if (!allowButtonReposition) return;
            
            isDragging = true;
            draggedButton = button;
            originalButtonPosition = button.GetComponent<RectTransform>().anchoredPosition;
        }

        void OnButtonDrag(Button button, PointerEventData eventData)
        {
            if (!allowButtonReposition || !isDragging || draggedButton != button) return;

            RectTransform buttonRect = button.GetComponent<RectTransform>();
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                buttonRect.parent as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint
            );
            buttonRect.anchoredPosition = localPoint;
        }

        void OnButtonDragEnd(Button button)
        {
            if (!allowButtonReposition || !isDragging || draggedButton != button) return;

            isDragging = false;
            draggedButton = null;
            
            // Save button position to PlayerPrefs for persistence
            SaveButtonPosition(button);
        }

        void SaveButtonPosition(Button button)
        {
            string buttonName = button.name;
            Vector2 position = button.GetComponent<RectTransform>().anchoredPosition;
            PlayerPrefs.SetFloat(buttonName + "_X", position.x);
            PlayerPrefs.SetFloat(buttonName + "_Y", position.y);
            PlayerPrefs.Save();
        }

        void LoadButtonPosition(Button button)
        {
            string buttonName = button.name;
            if (PlayerPrefs.HasKey(buttonName + "_X"))
            {
                float x = PlayerPrefs.GetFloat(buttonName + "_X");
                float y = PlayerPrefs.GetFloat(buttonName + "_Y");
                button.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
            }
        }

        // Utility Methods
        void ToggleScope()
        {
            // Implement scope functionality
            if (weaponSystem != null)
            {
                // This would interface with weapon system for scope mechanics
            }
        }

        public void ShowInventory()
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            }
        }

        public void ShowSettings()
        {
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(!settingsPanel.activeSelf);
            }
        }

        public void SetCrosshairVisibility(bool visible)
        {
            if (crosshair != null)
            {
                crosshair.gameObject.SetActive(visible);
            }
        }

        public void UpdateHealthBar(float currentHealth, float maxHealth)
        {
            if (healthBar != null)
            {
                healthBar.value = currentHealth / maxHealth;
            }

            if (healthText != null)
            {
                healthText.text = $"{Mathf.RoundToInt(currentHealth)}/{Mathf.RoundToInt(maxHealth)}";
            }
        }

        public void UpdateAmmoDisplay(int currentAmmo, int maxAmmo)
        {
            if (ammoText != null)
            {
                ammoText.text = $"{currentAmmo}/{maxAmmo}";
            }
        }

        // Load saved button positions on start
        void OnEnable()
        {
            if (allowButtonReposition)
            {
                LoadAllButtonPositions();
            }
        }

        void LoadAllButtonPositions()
        {
            if (fireButton != null) LoadButtonPosition(fireButton);
            if (jumpButton != null) LoadButtonPosition(jumpButton);
            if (crouchButton != null) LoadButtonPosition(crouchButton);
            if (reloadButton != null) LoadButtonPosition(reloadButton);
            if (scopeButton != null) LoadButtonPosition(scopeButton);
        }
    }
}
