using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

namespace FreeFire.Performance
{
    public class GraphicsSettings : MonoBehaviour
    {
        [Header("Quality Settings")]
        public Dropdown qualityDropdown;
        public Slider renderDistanceSlider;
        public Toggle shadowsToggle;
        public Toggle antiAliasingToggle;
        public Toggle particlesToggle;
        public Toggle postProcessingToggle;

        [Header("Performance Settings")]
        public Slider targetFramerateSlider;
        public Toggle vsyncToggle;
        public Toggle lowPowerModeToggle;

        [Header("Mobile Optimizations")]
        public Toggle mobileOptimizationsToggle;
        public Slider textureQualitySlider;
        public Slider lodBiasSlider;

        [Header("UI References")]
        public Text fpsText;
        public Text memoryText;
        public Button applyButton;
        public Button resetButton;

        // Quality levels
        private string[] qualityLevels = { "Low", "Medium", "High", "Ultra" };
    private int currentQualityLevel = 1; // Default to Medium

        // Performance monitoring
        private float fpsUpdateInterval = 0.5f;
        private float fpsAccumulator = 0f;
        private int fpsFrames = 0;
        private float fpsTimeLeft;
        private float currentFPS = 0f;

        void Start()
        {
            InitializeSettings();
            SetupUI();
            LoadSettings();
            StartPerformanceMonitoring();
        }

        void Update()
        {
            UpdatePerformanceMonitoring();
        }

        void InitializeSettings()
        {
            // Set default values
            fpsTimeLeft = fpsUpdateInterval;
            
            // Initialize quality settings
            if (QualitySettings.names.Length > 0)
            {
                currentQualityLevel = QualitySettings.GetQualityLevel();
            }
        }

        void SetupUI()
        {
            // Setup quality dropdown
            if (qualityDropdown != null)
            {
                qualityDropdown.ClearOptions();
                qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(qualityLevels));
                qualityDropdown.value = currentQualityLevel;
                qualityDropdown.onValueChanged.AddListener(OnQualityLevelChanged);
            }

            // Setup sliders
            if (renderDistanceSlider != null)
            {
                renderDistanceSlider.value = Camera.main.farClipPlane / 1000f; // Convert to 0-1 range
                renderDistanceSlider.onValueChanged.AddListener(OnRenderDistanceChanged);
            }

            if (targetFramerateSlider != null)
            {
                targetFramerateSlider.value = Application.targetFrameRate / 60f; // Convert to 0-1 range
                targetFramerateSlider.onValueChanged.AddListener(OnTargetFramerateChanged);
            }

            if (textureQualitySlider != null)
            {
                textureQualitySlider.value = QualitySettings.masterTextureLimit / 3f; // Convert to 0-1 range
                textureQualitySlider.onValueChanged.AddListener(OnTextureQualityChanged);
            }

            if (lodBiasSlider != null)
            {
                lodBiasSlider.value = QualitySettings.lodBias / 2f; // Convert to 0-1 range
                lodBiasSlider.onValueChanged.AddListener(OnLODBiasChanged);
            }

            // Setup toggles
            if (shadowsToggle != null)
            {
                shadowsToggle.isOn = QualitySettings.shadows != ShadowQuality.Disable;
                shadowsToggle.onValueChanged.AddListener(OnShadowsToggled);
            }

            if (antiAliasingToggle != null)
            {
                antiAliasingToggle.isOn = QualitySettings.antiAliasing > 0;
                antiAliasingToggle.onValueChanged.AddListener(OnAntiAliasingToggled);
            }

            if (particlesToggle != null)
            {
                particlesToggle.isOn = true; // Default enabled
                particlesToggle.onValueChanged.AddListener(OnParticlesToggled);
            }

            if (postProcessingToggle != null)
            {
                postProcessingToggle.isOn = true; // Default enabled
                postProcessingToggle.onValueChanged.AddListener(OnPostProcessingToggled);
            }

            if (vsyncToggle != null)
            {
                vsyncToggle.isOn = QualitySettings.vSyncCount > 0;
                vsyncToggle.onValueChanged.AddListener(OnVSyncToggled);
            }

            if (lowPowerModeToggle != null)
            {
                lowPowerModeToggle.isOn = false;
                lowPowerModeToggle.onValueChanged.AddListener(OnLowPowerModeToggled);
            }

            if (mobileOptimizationsToggle != null)
            {
                mobileOptimizationsToggle.isOn = Application.isMobilePlatform;
                mobileOptimizationsToggle.onValueChanged.AddListener(OnMobileOptimizationsToggled);
            }

            // Setup buttons
            if (applyButton != null)
            {
                applyButton.onClick.AddListener(ApplySettings);
            }

            if (resetButton != null)
            {
                resetButton.onClick.AddListener(ResetToDefaults);
            }
        }

        void StartPerformanceMonitoring()
        {
            // Start FPS monitoring
            if (fpsText != null)
            {
                InvokeRepeating(nameof(UpdateFPS), 0f, fpsUpdateInterval);
            }

            // Start memory monitoring
            if (memoryText != null)
            {
                InvokeRepeating(nameof(UpdateMemory), 0f, 1f);
            }
        }

        void UpdatePerformanceMonitoring()
        {
            fpsTimeLeft -= Time.deltaTime;
            fpsAccumulator += Time.timeScale / Time.deltaTime;
            fpsFrames++;

            if (fpsTimeLeft <= 0f)
            {
                currentFPS = fpsAccumulator / fpsFrames;
                fpsTimeLeft = fpsUpdateInterval;
                fpsAccumulator = 0f;
                fpsFrames = 0;
            }
        }

        void UpdateFPS()
        {
            if (fpsText != null)
            {
                fpsText.text = $"FPS: {currentFPS:F1}";
            }
        }

        void UpdateMemory()
        {
            if (memoryText != null)
            {
                long memoryUsage = System.GC.GetTotalMemory(false) / (1024 * 1024); // MB
                memoryText.text = $"Memory: {memoryUsage} MB";
            }
        }

        // UI Event Handlers
        void OnQualityLevelChanged(int level)
        {
            currentQualityLevel = level;
            QualitySettings.SetQualityLevel(level);
        }

        void OnRenderDistanceChanged(float value)
        {
            float renderDistance = value * 1000f; // Convert back to world units
            Camera.main.farClipPlane = renderDistance;
        }

        void OnTargetFramerateChanged(float value)
        {
            int targetFPS = Mathf.RoundToInt(value * 60f);
            Application.targetFrameRate = targetFPS;
        }

        void OnTextureQualityChanged(float value)
        {
            int textureLimit = Mathf.RoundToInt(value * 3f);
            QualitySettings.masterTextureLimit = textureLimit;
        }

        void OnLODBiasChanged(float value)
        {
            float lodBias = value * 2f;
            QualitySettings.lodBias = lodBias;
        }

        void OnShadowsToggled(bool enabled)
        {
            QualitySettings.shadows = enabled ? ShadowQuality.All : ShadowQuality.Disable;
        }

        void OnAntiAliasingToggled(bool enabled)
        {
            QualitySettings.antiAliasing = enabled ? 4 : 0;
        }

        void OnParticlesToggled(bool enabled)
        {
            // This would control particle systems in the scene
            var particleSystems = FindObjectsOfType<ParticleSystem>();
            foreach (var ps in particleSystems)
            {
                ps.gameObject.SetActive(enabled);
            }
        }

        void OnPostProcessingToggled(bool enabled)
        {
            // This would control post-processing effects
            var postProcessVolumes = FindObjectsOfType<UnityEngine.Rendering.Volume>();
            foreach (var volume in postProcessVolumes)
            {
                volume.enabled = enabled;
            }
        }

        void OnVSyncToggled(bool enabled)
        {
            QualitySettings.vSyncCount = enabled ? 1 : 0;
        }

        void OnLowPowerModeToggled(bool enabled)
        {
            if (enabled)
            {
                // Reduce quality for low power mode
                QualitySettings.SetQualityLevel(0); // Low quality
                Application.targetFrameRate = 30;
                QualitySettings.vSyncCount = 0;
            }
        }

        void OnMobileOptimizationsToggled(bool enabled)
        {
            if (enabled)
            {
                ApplyMobileOptimizations();
            }
        }

        void ApplyMobileOptimizations()
        {
            // Mobile-specific optimizations
            QualitySettings.SetQualityLevel(1); // Medium quality
            QualitySettings.shadows = ShadowQuality.HardOnly;
            QualitySettings.antiAliasing = 0;
            QualitySettings.masterTextureLimit = 1;
            QualitySettings.lodBias = 0.5f;
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 1;

            // Disable expensive effects
            OnParticlesToggled(false);
            OnPostProcessingToggled(false);
        }

        public void ApplySettings()
        {
            // Save settings to PlayerPrefs
            SaveSettings();
            
            // Apply any additional custom settings
            Debug.Log("Graphics settings applied!");
        }

        public void ResetToDefaults()
        {
            // Reset to default values
            currentQualityLevel = 1; // Medium
            
            if (qualityDropdown != null)
                qualityDropdown.value = currentQualityLevel;
            
            if (renderDistanceSlider != null)
                renderDistanceSlider.value = 1f;
            
            if (targetFramerateSlider != null)
                targetFramerateSlider.value = 1f;
            
            if (textureQualitySlider != null)
                textureQualitySlider.value = 0.33f;
            
            if (lodBiasSlider != null)
                lodBiasSlider.value = 0.5f;
            
            if (shadowsToggle != null)
                shadowsToggle.isOn = true;
            
            if (antiAliasingToggle != null)
                antiAliasingToggle.isOn = true;
            
            if (particlesToggle != null)
                particlesToggle.isOn = true;
            
            if (postProcessingToggle != null)
                postProcessingToggle.isOn = true;
            
            if (vsyncToggle != null)
                vsyncToggle.isOn = true;
            
            if (lowPowerModeToggle != null)
                lowPowerModeToggle.isOn = false;
            
            if (mobileOptimizationsToggle != null)
                mobileOptimizationsToggle.isOn = Application.isMobilePlatform;

            // Apply the reset settings
            OnQualityLevelChanged(currentQualityLevel);
            OnRenderDistanceChanged(1f);
            OnTargetFramerateChanged(1f);
            OnTextureQualityChanged(0.33f);
            OnLODBiasChanged(0.5f);
            OnShadowsToggled(true);
            OnAntiAliasingToggled(true);
            OnParticlesToggled(true);
            OnPostProcessingToggled(true);
            OnVSyncToggled(true);
        }

        void SaveSettings()
        {
            PlayerPrefs.SetInt("QualityLevel", currentQualityLevel);
            PlayerPrefs.SetFloat("RenderDistance", renderDistanceSlider != null ? renderDistanceSlider.value : 1f);
            PlayerPrefs.SetFloat("TargetFramerate", targetFramerateSlider != null ? targetFramerateSlider.value : 1f);
            PlayerPrefs.SetFloat("TextureQuality", textureQualitySlider != null ? textureQualitySlider.value : 0.33f);
            PlayerPrefs.SetFloat("LODBias", lodBiasSlider != null ? lodBiasSlider.value : 0.5f);
            PlayerPrefs.SetInt("Shadows", shadowsToggle != null && shadowsToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt("AntiAliasing", antiAliasingToggle != null && antiAliasingToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt("Particles", particlesToggle != null && particlesToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt("PostProcessing", postProcessingToggle != null && postProcessingToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt("VSync", vsyncToggle != null && vsyncToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt("LowPowerMode", lowPowerModeToggle != null && lowPowerModeToggle.isOn ? 1 : 0);
            PlayerPrefs.SetInt("MobileOptimizations", mobileOptimizationsToggle != null && mobileOptimizationsToggle.isOn ? 1 : 0);
            PlayerPrefs.Save();
        }

        void LoadSettings()
        {
            if (PlayerPrefs.HasKey("QualityLevel"))
            {
                currentQualityLevel = PlayerPrefs.GetInt("QualityLevel");
                if (qualityDropdown != null)
                    qualityDropdown.value = currentQualityLevel;
                OnQualityLevelChanged(currentQualityLevel);
            }

            if (PlayerPrefs.HasKey("RenderDistance") && renderDistanceSlider != null)
            {
                renderDistanceSlider.value = PlayerPrefs.GetFloat("RenderDistance");
                OnRenderDistanceChanged(renderDistanceSlider.value);
            }

            if (PlayerPrefs.HasKey("TargetFramerate") && targetFramerateSlider != null)
            {
                targetFramerateSlider.value = PlayerPrefs.GetFloat("TargetFramerate");
                OnTargetFramerateChanged(targetFramerateSlider.value);
            }

            if (PlayerPrefs.HasKey("TextureQuality") && textureQualitySlider != null)
            {
                textureQualitySlider.value = PlayerPrefs.GetFloat("TextureQuality");
                OnTextureQualityChanged(textureQualitySlider.value);
            }

            if (PlayerPrefs.HasKey("LODBias") && lodBiasSlider != null)
            {
                lodBiasSlider.value = PlayerPrefs.GetFloat("LODBias");
                OnLODBiasChanged(lodBiasSlider.value);
            }

            if (PlayerPrefs.HasKey("Shadows") && shadowsToggle != null)
            {
                shadowsToggle.isOn = PlayerPrefs.GetInt("Shadows") == 1;
                OnShadowsToggled(shadowsToggle.isOn);
            }

            if (PlayerPrefs.HasKey("AntiAliasing") && antiAliasingToggle != null)
            {
                antiAliasingToggle.isOn = PlayerPrefs.GetInt("AntiAliasing") == 1;
                OnAntiAliasingToggled(antiAliasingToggle.isOn);
            }

            if (PlayerPrefs.HasKey("Particles") && particlesToggle != null)
            {
                particlesToggle.isOn = PlayerPrefs.GetInt("Particles") == 1;
                OnParticlesToggled(particlesToggle.isOn);
            }

            if (PlayerPrefs.HasKey("PostProcessing") && postProcessingToggle != null)
            {
                postProcessingToggle.isOn = PlayerPrefs.GetInt("PostProcessing") == 1;
                OnPostProcessingToggled(postProcessingToggle.isOn);
            }

            if (PlayerPrefs.HasKey("VSync") && vsyncToggle != null)
            {
                vsyncToggle.isOn = PlayerPrefs.GetInt("VSync") == 1;
                OnVSyncToggled(vsyncToggle.isOn);
            }

            if (PlayerPrefs.HasKey("LowPowerMode") && lowPowerModeToggle != null)
            {
                lowPowerModeToggle.isOn = PlayerPrefs.GetInt("LowPowerMode") == 1;
                OnLowPowerModeToggled(lowPowerModeToggle.isOn);
            }

            if (PlayerPrefs.HasKey("MobileOptimizations") && mobileOptimizationsToggle != null)
            {
                mobileOptimizationsToggle.isOn = PlayerPrefs.GetInt("MobileOptimizations") == 1;
                OnMobileOptimizationsToggled(mobileOptimizationsToggle.isOn);
            }
        }

        // Public getters
        public float GetCurrentFPS() => currentFPS;
        public int GetCurrentQualityLevel() => currentQualityLevel;
        public bool IsLowPowerMode() => lowPowerModeToggle != null && lowPowerModeToggle.isOn;
        public bool IsMobileOptimized() => mobileOptimizationsToggle != null && mobileOptimizationsToggle.isOn;
    }
}
