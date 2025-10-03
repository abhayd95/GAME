using UnityEngine;
using UnityEngine.UI;

namespace FreeFire.Gameplay
{
    public class ZoneSystem : MonoBehaviour
    {
        [Header("Zone Settings")]
        public float initialZoneRadius = 200f;
        public float finalZoneRadius = 10f;
        public float zoneShrinkTime = 300f; // 5 minutes total
        public float damagePerSecond = 10f;
        public float warningTime = 30f; // Warning before zone starts moving

        [Header("Visual Effects")]
        public GameObject zoneWallPrefab;
        public Material safeZoneMaterial;
        public Material dangerZoneMaterial;
        public ParticleSystem zoneParticles;
        public AudioSource zoneAudioSource;
        public AudioClip zoneWarningSound;
        public AudioClip zoneDamageSound;

        [Header("UI")]
        public Text zoneTimerText;
        public Text zoneDistanceText;
        public Image zoneWarningImage;
        public Slider zoneProgressBar;

        [Header("Zone Phases")]
        public ZonePhase[] zonePhases;

        [System.Serializable]
        public class ZonePhase
        {
            public float radius;
            public float waitTime;
            public float shrinkTime;
            public Color zoneColor;
        }

        // Private variables
        private float currentZoneRadius;
        private float targetZoneRadius;
        private Vector3 zoneCenter;
        private bool isZoneMoving = false;
        private bool isPlayerInSafeZone = true;
        private float currentPhaseTime = 0f;
        private int currentPhaseIndex = 0;
        private float totalGameTime = 0f;
        private Transform player;

        // Zone state
        public enum ZoneState
        {
            Waiting,
            Moving,
            Stable
        }

        private ZoneState currentState = ZoneState.Waiting;

        void Start()
        {
            InitializeZone();
            FindPlayer();
            SetupZonePhases();
        }

        void Update()
        {
            UpdateZoneSystem();
            CheckPlayerZoneStatus();
            UpdateUI();
        }

        void InitializeZone()
        {
            currentZoneRadius = initialZoneRadius;
            targetZoneRadius = initialZoneRadius;
            zoneCenter = Vector3.zero; // Center of the map

            // Create initial zone wall
            CreateZoneWall();
        }

        void FindPlayer()
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        void SetupZonePhases()
        {
            if (zonePhases == null || zonePhases.Length == 0)
            {
                // Create default phases if none are set
                CreateDefaultPhases();
            }
        }

        void CreateDefaultPhases()
        {
            zonePhases = new ZonePhase[]
            {
                new ZonePhase { radius = 150f, waitTime = 60f, shrinkTime = 30f, zoneColor = Color.blue },
                new ZonePhase { radius = 100f, waitTime = 45f, shrinkTime = 25f, zoneColor = Color.yellow },
                new ZonePhase { radius = 70f, waitTime = 30f, shrinkTime = 20f, zoneColor = Color.orange },
                new ZonePhase { radius = 50f, waitTime = 20f, shrinkTime = 15f, zoneColor = Color.red },
                new ZonePhase { radius = 30f, waitTime = 15f, shrinkTime = 10f, zoneColor = Color.red },
                new ZonePhase { radius = 15f, waitTime = 10f, shrinkTime = 5f, zoneColor = Color.red },
                new ZonePhase { radius = 8f, waitTime = 5f, shrinkTime = 3f, zoneColor = Color.red }
            };
        }

        void UpdateZoneSystem()
        {
            totalGameTime += Time.deltaTime;

            if (currentPhaseIndex >= zonePhases.Length)
            {
                // Game should end - final zone reached
                return;
            }

            ZonePhase currentPhase = zonePhases[currentPhaseIndex];
            currentPhaseTime += Time.deltaTime;

            switch (currentState)
            {
                case ZoneState.Waiting:
                    HandleWaitingPhase(currentPhase);
                    break;
                case ZoneState.Moving:
                    HandleMovingPhase(currentPhase);
                    break;
                case ZoneState.Stable:
                    HandleStablePhase(currentPhase);
                    break;
            }
        }

        void HandleWaitingPhase(ZonePhase phase)
        {
            if (currentPhaseTime >= phase.waitTime)
            {
                // Start moving to next zone
                currentState = ZoneState.Moving;
                currentPhaseTime = 0f;
                targetZoneRadius = phase.radius;
                isZoneMoving = true;

                // Play warning sound
                if (zoneAudioSource != null && zoneWarningSound != null)
                {
                    zoneAudioSource.PlayOneShot(zoneWarningSound);
                }

                // Show warning UI
                ShowZoneWarning(true);
            }
        }

        void HandleMovingPhase(ZonePhase phase)
        {
            float shrinkProgress = currentPhaseTime / phase.shrinkTime;
            currentZoneRadius = Mathf.Lerp(initialZoneRadius, targetZoneRadius, shrinkProgress);

            if (shrinkProgress >= 1f)
            {
                // Zone movement complete
                currentState = ZoneState.Stable;
                currentPhaseTime = 0f;
                isZoneMoving = false;
                currentZoneRadius = targetZoneRadius;
                initialZoneRadius = targetZoneRadius;

                // Hide warning UI
                ShowZoneWarning(false);

                // Move to next phase
                currentPhaseIndex++;
            }

            UpdateZoneWall();
        }

        void HandleStablePhase(ZonePhase phase)
        {
            // Zone is stable, waiting for next phase
            if (currentPhaseIndex < zonePhases.Length - 1)
            {
                currentState = ZoneState.Waiting;
                currentPhaseTime = 0f;
            }
        }

        void CheckPlayerZoneStatus()
        {
            if (player == null) return;

            float distanceFromCenter = Vector3.Distance(player.position, zoneCenter);
            bool wasInSafeZone = isPlayerInSafeZone;
            isPlayerInSafeZone = distanceFromCenter <= currentZoneRadius;

            // Player entered danger zone
            if (wasInSafeZone && !isPlayerInSafeZone)
            {
                OnPlayerEnterDangerZone();
            }
            // Player left danger zone
            else if (!wasInSafeZone && isPlayerInSafeZone)
            {
                OnPlayerExitDangerZone();
            }

            // Apply damage if player is in danger zone
            if (!isPlayerInSafeZone)
            {
                ApplyZoneDamage();
            }
        }

        void OnPlayerEnterDangerZone()
        {
            // Play damage sound
            if (zoneAudioSource != null && zoneDamageSound != null)
            {
                zoneAudioSource.PlayOneShot(zoneDamageSound);
            }

            // Show danger UI effects
            ShowDangerEffects(true);
        }

        void OnPlayerExitDangerZone()
        {
            // Hide danger UI effects
            ShowDangerEffects(false);
        }

        void ApplyZoneDamage()
        {
            if (player == null) return;

            var playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damagePerSecond * Time.deltaTime);
            }
        }

        void CreateZoneWall()
        {
            if (zoneWallPrefab == null) return;

            // Create zone wall at current radius
            GameObject zoneWall = Instantiate(zoneWallPrefab, zoneCenter, Quaternion.identity);
            zoneWall.transform.localScale = Vector3.one * currentZoneRadius * 2f;
        }

        void UpdateZoneWall()
        {
            // Update existing zone wall or create new one
            GameObject existingWall = GameObject.FindGameObjectWithTag("ZoneWall");
            if (existingWall != null)
            {
                existingWall.transform.localScale = Vector3.one * currentZoneRadius * 2f;
            }
            else
            {
                CreateZoneWall();
            }
        }

        void ShowZoneWarning(bool show)
        {
            if (zoneWarningImage != null)
            {
                zoneWarningImage.gameObject.SetActive(show);
            }
        }

        void ShowDangerEffects(bool show)
        {
            // Implement screen effects for danger zone
            if (show)
            {
                // Red tint, screen shake, etc.
            }
        }

        void UpdateUI()
        {
            // Update zone timer
            if (zoneTimerText != null)
            {
                float timeUntilNextPhase = 0f;
                if (currentPhaseIndex < zonePhases.Length)
                {
                    ZonePhase currentPhase = zonePhases[currentPhaseIndex];
                    switch (currentState)
                    {
                        case ZoneState.Waiting:
                            timeUntilNextPhase = currentPhase.waitTime - currentPhaseTime;
                            break;
                        case ZoneState.Moving:
                            timeUntilNextPhase = currentPhase.shrinkTime - currentPhaseTime;
                            break;
                        case ZoneState.Stable:
                            timeUntilNextPhase = 0f;
                            break;
                    }
                }
                zoneTimerText.text = $"Zone: {Mathf.CeilToInt(timeUntilNextPhase)}s";
            }

            // Update zone distance
            if (zoneDistanceText != null && player != null)
            {
                float distanceFromCenter = Vector3.Distance(player.position, zoneCenter);
                float distanceFromZone = distanceFromCenter - currentZoneRadius;
                zoneDistanceText.text = $"Distance: {Mathf.RoundToInt(distanceFromZone)}m";
            }

            // Update zone progress
            if (zoneProgressBar != null)
            {
                float progress = (float)currentPhaseIndex / zonePhases.Length;
                zoneProgressBar.value = progress;
            }
        }

        // Public methods for other systems
        public bool IsPlayerInSafeZone()
        {
            return isPlayerInSafeZone;
        }

        public float GetCurrentZoneRadius()
        {
            return currentZoneRadius;
        }

        public Vector3 GetZoneCenter()
        {
            return zoneCenter;
        }

        public float GetDistanceToZone(Vector3 position)
        {
            float distanceFromCenter = Vector3.Distance(position, zoneCenter);
            return distanceFromCenter - currentZoneRadius;
        }

        public ZoneState GetCurrentZoneState()
        {
            return currentState;
        }

        public int GetCurrentPhaseIndex()
        {
            return currentPhaseIndex;
        }

        public float GetTimeUntilNextPhase()
        {
            if (currentPhaseIndex >= zonePhases.Length) return 0f;

            ZonePhase currentPhase = zonePhases[currentPhaseIndex];
            switch (currentState)
            {
                case ZoneState.Waiting:
                    return currentPhase.waitTime - currentPhaseTime;
                case ZoneState.Moving:
                    return currentPhase.shrinkTime - currentPhaseTime;
                default:
                    return 0f;
            }
        }
    }
}
