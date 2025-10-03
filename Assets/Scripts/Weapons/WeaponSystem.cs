using UnityEngine;
using UnityEngine.UI;

namespace FreeFire.Weapons
{
    public enum WeaponType
    {
        SMG,
        AssaultRifle,
        SniperRifle,
        Shotgun,
        Pistol
    }

    [System.Serializable]
    public class WeaponData
    {
        public string weaponName;
        public WeaponType weaponType;
        public int damage;
        public int maxAmmo;
        public int currentAmmo;
        public float fireRate;
        public float range;
        public float reloadTime;
        public GameObject weaponPrefab;
        public AudioClip fireSound;
        public AudioClip reloadSound;
        public ParticleSystem muzzleFlash;
    }

    public class WeaponSystem : MonoBehaviour
    {
        [Header("Weapon Settings")]
        public WeaponData[] availableWeapons;
        public Transform weaponHolder;
        public Transform firePoint;
        public LayerMask enemyLayer;

        [Header("UI References")]
        public Text ammoText;
        public Image crosshair;
        public Slider reloadSlider;

        [Header("Audio")]
        public AudioSource audioSource;

        [Header("Effects")]
        public GameObject bulletHolePrefab;
        public GameObject bloodEffectPrefab;

        // Private variables
        private int currentWeaponIndex = 0;
        private WeaponData currentWeapon;
        private bool isReloading = false;
        private float nextFireTime = 0f;
        private Camera playerCamera;

        void Start()
        {
            playerCamera = Camera.main;
            if (availableWeapons.Length > 0)
            {
                EquipWeapon(0);
            }
            UpdateUI();
        }

        void Update()
        {
            HandleInput();
            UpdateCrosshair();
        }

        void HandleInput()
        {
            if (isReloading) return;

            // Fire input
            bool fireInput = false;
            if (Application.isMobilePlatform)
            {
                // Mobile fire input will be handled by UI button
            }
            else
            {
                fireInput = Input.GetButton("Fire1");
            }

            if (fireInput && Time.time >= nextFireTime)
            {
                Fire();
            }

            // Reload input
            if (Input.GetKeyDown(KeyCode.R) || (Application.isMobilePlatform && reloadInput))
            {
                StartReload();
            }

            // Weapon switching
            if (Input.GetKeyDown(KeyCode.Alpha1) && availableWeapons.Length > 0)
                EquipWeapon(0);
            if (Input.GetKeyDown(KeyCode.Alpha2) && availableWeapons.Length > 1)
                EquipWeapon(1);
            if (Input.GetKeyDown(KeyCode.Alpha3) && availableWeapons.Length > 2)
                EquipWeapon(2);
        }

        public void Fire()
        {
            if (currentWeapon == null || currentWeapon.currentAmmo <= 0 || isReloading)
                return;

            // Check fire rate
            if (Time.time < nextFireTime)
                return;

            nextFireTime = Time.time + (1f / currentWeapon.fireRate);

            // Consume ammo
            currentWeapon.currentAmmo--;

            // Play fire sound
            if (currentWeapon.fireSound != null)
            {
                audioSource.PlayOneShot(currentWeapon.fireSound);
            }

            // Muzzle flash
            if (currentWeapon.muzzleFlash != null)
            {
                currentWeapon.muzzleFlash.Play();
            }

            // Raycast for hit detection
            RaycastHit hit;
            Vector3 rayOrigin = playerCamera.transform.position;
            Vector3 rayDirection = playerCamera.transform.forward;

            if (Physics.Raycast(rayOrigin, rayDirection, out hit, currentWeapon.range))
            {
                // Hit something
                if (hit.collider.CompareTag("Enemy"))
                {
                    // Hit enemy
                    DealDamage(hit.collider.gameObject, currentWeapon.damage);
                    CreateBloodEffect(hit.point, hit.normal);
                }
                else
                {
                    // Hit environment
                    CreateBulletHole(hit.point, hit.normal);
                }
            }

            UpdateUI();

            // Auto reload if empty
            if (currentWeapon.currentAmmo <= 0)
            {
                StartReload();
            }
        }

        void DealDamage(GameObject target, int damage)
        {
            // This would interface with enemy health system
            var enemyHealth = target.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }
        }

        void CreateBulletHole(Vector3 position, Vector3 normal)
        {
            if (bulletHolePrefab != null)
            {
                GameObject bulletHole = Instantiate(bulletHolePrefab, position, Quaternion.LookRotation(normal));
                Destroy(bulletHole, 10f); // Remove after 10 seconds
            }
        }

        void CreateBloodEffect(Vector3 position, Vector3 normal)
        {
            if (bloodEffectPrefab != null)
            {
                GameObject bloodEffect = Instantiate(bloodEffectPrefab, position, Quaternion.LookRotation(normal));
                Destroy(bloodEffect, 2f);
            }
        }

        public void StartReload()
        {
            if (isReloading || currentWeapon == null || currentWeapon.currentAmmo >= currentWeapon.maxAmmo)
                return;

            isReloading = true;
            StartCoroutine(ReloadCoroutine());
        }

        System.Collections.IEnumerator ReloadCoroutine()
        {
            // Play reload sound
            if (currentWeapon.reloadSound != null)
            {
                audioSource.PlayOneShot(currentWeapon.reloadSound);
            }

            // Show reload progress
            if (reloadSlider != null)
            {
                reloadSlider.gameObject.SetActive(true);
                float reloadTime = currentWeapon.reloadTime;
                float elapsedTime = 0f;

                while (elapsedTime < reloadTime)
                {
                    elapsedTime += Time.deltaTime;
                    reloadSlider.value = elapsedTime / reloadTime;
                    yield return null;
                }

                reloadSlider.gameObject.SetActive(false);
            }
            else
            {
                yield return new WaitForSeconds(currentWeapon.reloadTime);
            }

            // Refill ammo
            currentWeapon.currentAmmo = currentWeapon.maxAmmo;
            isReloading = false;
            UpdateUI();
        }

        void EquipWeapon(int weaponIndex)
        {
            if (weaponIndex < 0 || weaponIndex >= availableWeapons.Length)
                return;

            // Remove current weapon
            if (weaponHolder.childCount > 0)
            {
                Destroy(weaponHolder.GetChild(0).gameObject);
            }

            currentWeaponIndex = weaponIndex;
            currentWeapon = availableWeapons[weaponIndex];

            // Instantiate new weapon
            if (currentWeapon.weaponPrefab != null)
            {
                GameObject weapon = Instantiate(currentWeapon.weaponPrefab, weaponHolder);
                weapon.transform.localPosition = Vector3.zero;
                weapon.transform.localRotation = Quaternion.identity;
            }

            UpdateUI();
        }

        void UpdateUI()
        {
            if (ammoText != null && currentWeapon != null)
            {
                ammoText.text = $"{currentWeapon.currentAmmo}/{currentWeapon.maxAmmo}";
            }
        }

        void UpdateCrosshair()
        {
            if (crosshair == null) return;

            // Simple crosshair - could be enhanced with spread mechanics
            crosshair.color = Color.white;
        }

        public WeaponData GetCurrentWeapon()
        {
            return currentWeapon;
        }

        public bool IsReloading => isReloading;

        // Mobile input methods
        private bool reloadInput = false;
        public void OnReloadButtonPressed()
        {
            reloadInput = true;
        }

        public void OnFireButtonPressed()
        {
            Fire();
        }
    }
}
