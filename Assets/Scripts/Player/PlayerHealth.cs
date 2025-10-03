using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace FreeFire.Player
{
    public class PlayerHealth : NetworkBehaviour
    {
        [Header("Health Settings")]
        [SyncVar] public float maxHealth = 100f;
        [SyncVar] public float currentHealth;
        [SyncVar] public float maxArmor = 100f;
        [SyncVar] public float currentArmor;

        [Header("Regeneration")]
        public bool canRegenerate = false;
        public float regenRate = 5f;
        public float regenDelay = 5f;
        private float lastDamageTime;

        [Header("UI References")]
        public Slider healthBar;
        public Slider armorBar;
        public Text healthText;
        public Image damageOverlay;
        public GameObject deathScreen;

        [Header("Audio")]
        public AudioSource audioSource;
        public AudioClip[] damageSounds;
        public AudioClip[] deathSounds;
        public AudioClip healSound;

        [Header("Effects")]
        public ParticleSystem bloodEffect;
        public GameObject deathEffect;

        // Events
        public System.Action<float, float> OnHealthChanged;
        public System.Action<float, float> OnArmorChanged;
        public System.Action OnPlayerDeath;
        public System.Action OnPlayerRevive;

        // Private variables
        private bool isDead = false;
        private float damageOverlayAlpha = 0f;
        private Color damageOverlayColor;

        void Start()
        {
            if (isServer)
            {
                currentHealth = maxHealth;
                currentArmor = 0f;
            }

            if (damageOverlay != null)
            {
                damageOverlayColor = damageOverlay.color;
                damageOverlayColor.a = 0f;
                damageOverlay.color = damageOverlayColor;
            }

            UpdateUI();
        }

        void Update()
        {
            if (isDead) return;

            // Handle regeneration
            if (canRegenerate && currentHealth < maxHealth && Time.time - lastDamageTime > regenDelay)
            {
                RegenerateHealth();
            }

            // Update damage overlay
            UpdateDamageOverlay();
        }

        [Server]
        public void TakeDamage(float damage, GameObject attacker = null)
        {
            if (isDead) return;

            lastDamageTime = Time.time;

            // Apply damage to armor first
            float remainingDamage = damage;
            if (currentArmor > 0)
            {
                float armorDamage = Mathf.Min(currentArmor, damage * 0.5f); // Armor absorbs 50% of damage
                currentArmor -= armorDamage;
                remainingDamage -= armorDamage;
                OnArmorChanged?.Invoke(currentArmor, maxArmor);
            }

            // Apply remaining damage to health
            currentHealth -= remainingDamage;
            currentHealth = Mathf.Max(0, currentHealth);

            // Notify clients of health change
            RpcUpdateHealth(currentHealth, currentArmor);

            // Play damage effects
            RpcPlayDamageEffects(damage);

            // Check for death
            if (currentHealth <= 0)
            {
                Die(attacker);
            }

            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        [Server]
        public void Heal(float amount)
        {
            if (isDead) return;

            currentHealth += amount;
            currentHealth = Mathf.Min(maxHealth, currentHealth);

            RpcUpdateHealth(currentHealth, currentArmor);
            RpcPlayHealEffect();

            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        [Server]
        public void AddArmor(float amount)
        {
            if (isDead) return;

            currentArmor += amount;
            currentArmor = Mathf.Min(maxArmor, currentArmor);

            RpcUpdateHealth(currentHealth, currentArmor);
            OnArmorChanged?.Invoke(currentArmor, maxArmor);
        }

        [Server]
        void Die(GameObject killer)
        {
            if (isDead) return;

            isDead = true;
            currentHealth = 0;

            // Disable player controller
            var playerController = GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = false;
            }

            // Disable weapon system
            var weaponSystem = GetComponent<WeaponSystem>();
            if (weaponSystem != null)
            {
                weaponSystem.enabled = false;
            }

            // Play death effects
            RpcPlayDeathEffects();

            // Notify game manager
            var gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.OnPlayerDeath(gameObject, killer);
            }

            OnPlayerDeath?.Invoke();
        }

        [Server]
        public void Revive(float healthAmount = 50f)
        {
            if (!isDead) return;

            isDead = false;
            currentHealth = healthAmount;

            // Re-enable player controller
            var playerController = GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.enabled = true;
            }

            // Re-enable weapon system
            var weaponSystem = GetComponent<WeaponSystem>();
            if (weaponSystem != null)
            {
                weaponSystem.enabled = true;
            }

            RpcUpdateHealth(currentHealth, currentArmor);
            RpcPlayReviveEffect();

            OnPlayerRevive?.Invoke();
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        void RegenerateHealth()
        {
            if (isServer)
            {
                Heal(regenRate * Time.deltaTime);
            }
        }

        void UpdateDamageOverlay()
        {
            if (damageOverlay == null) return;

            // Fade out damage overlay
            if (damageOverlayAlpha > 0)
            {
                damageOverlayAlpha -= Time.deltaTime * 2f;
                damageOverlayColor.a = damageOverlayAlpha;
                damageOverlay.color = damageOverlayColor;
            }
        }

        [ClientRpc]
        void RpcUpdateHealth(float health, float armor)
        {
            currentHealth = health;
            currentArmor = armor;
            UpdateUI();
        }

        [ClientRpc]
        void RpcPlayDamageEffects(float damage)
        {
            // Show damage overlay
            if (damageOverlay != null)
            {
                damageOverlayAlpha = Mathf.Clamp01(damage / 50f); // Scale based on damage
                damageOverlayColor.a = damageOverlayAlpha;
                damageOverlay.color = damageOverlayColor;
            }

            // Play damage sound
            if (audioSource != null && damageSounds.Length > 0)
            {
                AudioClip randomSound = damageSounds[Random.Range(0, damageSounds.Length)];
                audioSource.PlayOneShot(randomSound);
            }

            // Play blood effect
            if (bloodEffect != null)
            {
                bloodEffect.Play();
            }

            // Screen shake
            StartCoroutine(ScreenShake(0.2f, 0.1f));
        }

        [ClientRpc]
        void RpcPlayHealEffect()
        {
            if (audioSource != null && healSound != null)
            {
                audioSource.PlayOneShot(healSound);
            }
        }

        [ClientRpc]
        void RpcPlayDeathEffects()
        {
            // Show death screen
            if (deathScreen != null)
            {
                deathScreen.SetActive(true);
            }

            // Play death sound
            if (audioSource != null && deathSounds.Length > 0)
            {
                AudioClip randomSound = deathSounds[Random.Range(0, deathSounds.Length)];
                audioSource.PlayOneShot(randomSound);
            }

            // Play death effect
            if (deathEffect != null)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
            }
        }

        [ClientRpc]
        void RpcPlayReviveEffect()
        {
            // Hide death screen
            if (deathScreen != null)
            {
                deathScreen.SetActive(false);
            }

            // Play revive sound
            if (audioSource != null && healSound != null)
            {
                audioSource.PlayOneShot(healSound);
            }
        }

        System.Collections.IEnumerator ScreenShake(float duration, float magnitude)
        {
            Vector3 originalPosition = Camera.main.transform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                Camera.main.transform.localPosition = new Vector3(x, y, originalPosition.z);

                elapsed += Time.deltaTime;
                yield return null;
            }

            Camera.main.transform.localPosition = originalPosition;
        }

        void UpdateUI()
        {
            if (healthBar != null)
            {
                healthBar.value = currentHealth / maxHealth;
            }

            if (armorBar != null)
            {
                armorBar.value = currentArmor / maxArmor;
            }

            if (healthText != null)
            {
                healthText.text = $"{Mathf.RoundToInt(currentHealth)}/{Mathf.RoundToInt(maxHealth)}";
            }
        }

        // Public getters
        public bool IsDead => isDead;
        public float HealthPercentage => currentHealth / maxHealth;
        public float ArmorPercentage => currentArmor / maxArmor;
        public bool IsFullHealth => currentHealth >= maxHealth;
        public bool IsFullArmor => currentArmor >= maxArmor;

        // Damage types for different sources
        public void TakeBulletDamage(float damage, GameObject shooter)
        {
            TakeDamage(damage, shooter);
        }

        public void TakeZoneDamage(float damage)
        {
            TakeDamage(damage);
        }

        public void TakeFallDamage(float damage)
        {
            TakeDamage(damage);
        }

        public void TakeExplosionDamage(float damage, GameObject source)
        {
            TakeDamage(damage, source);
        }
    }
}
