using UnityEngine;

namespace FreeFire.Weapons
{
    public class EnemyHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        public float maxHealth = 100f;
        public float currentHealth;
        public bool isDead = false;

        [Header("Effects")]
        public GameObject deathEffect;
        public AudioClip deathSound;
        public ParticleSystem bloodEffect;

        [Header("Loot")]
        public GameObject[] lootDrops;
        public float lootDropChance = 0.3f;

        // Events
        public System.Action<float, float> OnHealthChanged;
        public System.Action OnDeath;

        void Start()
        {
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public void TakeDamage(float damage)
        {
            if (isDead) return;

            currentHealth -= damage;
            currentHealth = Mathf.Max(0, currentHealth);

            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            // Play damage effects
            if (bloodEffect != null)
            {
                bloodEffect.Play();
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        void Die()
        {
            if (isDead) return;

            isDead = true;
            currentHealth = 0;

            // Play death effects
            if (deathEffect != null)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
            }

            if (deathSound != null)
            {
                AudioSource.PlayClipAtPoint(deathSound, transform.position);
            }

            // Drop loot
            DropLoot();

            // Disable components
            var collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }

            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }

            OnDeath?.Invoke();

            // Destroy after delay
            Destroy(gameObject, 2f);
        }

        void DropLoot()
        {
            if (lootDrops.Length == 0) return;

            if (Random.value < lootDropChance)
            {
                GameObject lootToDrop = lootDrops[Random.Range(0, lootDrops.Length)];
                if (lootToDrop != null)
                {
                    Instantiate(lootToDrop, transform.position + Vector3.up, Quaternion.identity);
                }
            }
        }

        public void Heal(float amount)
        {
            if (isDead) return;

            currentHealth += amount;
            currentHealth = Mathf.Min(maxHealth, currentHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        public float GetHealthPercentage()
        {
            return currentHealth / maxHealth;
        }

        public bool IsAlive()
        {
            return !isDead && currentHealth > 0;
        }
    }
}
