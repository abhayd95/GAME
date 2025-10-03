using UnityEngine;
using System.Collections.Generic;

namespace FreeFire.Gameplay
{
    public enum LootType
    {
        Weapon,
        Ammo,
        Health,
        Armor,
        Utility
    }

    [System.Serializable]
    public class LootItem
    {
        public string itemName;
        public LootType lootType;
        public GameObject itemPrefab;
        public int quantity = 1;
        public float spawnWeight = 1f;
        public Sprite itemIcon;
        public string description;
    }

    [System.Serializable]
    public class LootSpawnPoint
    {
        public Vector3 position;
        public float spawnRadius = 2f;
        public LootType[] allowedTypes;
        public int maxItems = 3;
        public bool isHighValue = false;
    }

    public class LootSystem : MonoBehaviour
    {
        [Header("Loot Configuration")]
        public LootItem[] availableLoot;
        public LootSpawnPoint[] spawnPoints;
        public int totalLootItems = 200;

        [Header("Spawn Settings")]
        public float spawnHeight = 0.5f;
        public LayerMask groundLayer = 1;
        public bool useRandomSpawnPoints = true;
        public int randomSpawnPointCount = 150;

        [Header("Loot Categories")]
        public LootItem[] weapons;
        public LootItem[] ammo;
        public LootItem[] healthItems;
        public LootItem[] armorItems;
        public LootItem[] utilityItems;

        [Header("High Value Loot")]
        public LootItem[] highValueLoot;
        public float highValueSpawnChance = 0.1f;

        // Private variables
        private List<GameObject> spawnedLoot = new List<GameObject>();
        private Dictionary<LootType, List<LootItem>> lootByType = new Dictionary<LootType, List<LootItem>>();

        void Start()
        {
            InitializeLootSystem();
            SpawnInitialLoot();
        }

        void InitializeLootSystem()
        {
            // Organize loot by type
            lootByType.Clear();
            foreach (LootType type in System.Enum.GetValues(typeof(LootType)))
            {
                lootByType[type] = new List<LootItem>();
            }

            // Categorize available loot
            foreach (LootItem item in availableLoot)
            {
                if (lootByType.ContainsKey(item.lootType))
                {
                    lootByType[item.lootType].Add(item);
                }
            }

            // Setup default loot if none provided
            if (availableLoot == null || availableLoot.Length == 0)
            {
                CreateDefaultLoot();
            }

            // Generate spawn points if none provided
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                GenerateSpawnPoints();
            }
        }

        void CreateDefaultLoot()
        {
            List<LootItem> defaultLoot = new List<LootItem>();

            // Weapons
            defaultLoot.Add(new LootItem
            {
                itemName = "AK-47",
                lootType = LootType.Weapon,
                spawnWeight = 0.8f,
                description = "High damage assault rifle"
            });

            defaultLoot.Add(new LootItem
            {
                itemName = "M4A1",
                lootType = LootType.Weapon,
                spawnWeight = 0.9f,
                description = "Balanced assault rifle"
            });

            defaultLoot.Add(new LootItem
            {
                itemName = "AWM Sniper",
                lootType = LootType.Weapon,
                spawnWeight = 0.3f,
                description = "High damage sniper rifle"
            });

            defaultLoot.Add(new LootItem
            {
                itemName = "UMP9 SMG",
                lootType = LootType.Weapon,
                spawnWeight = 1.0f,
                description = "Fast firing SMG"
            });

            // Ammo
            defaultLoot.Add(new LootItem
            {
                itemName = "7.62 Ammo",
                lootType = LootType.Ammo,
                quantity = 30,
                spawnWeight = 1.5f,
                description = "Rifle ammunition"
            });

            defaultLoot.Add(new LootItem
            {
                itemName = "5.56 Ammo",
                lootType = LootType.Ammo,
                quantity = 30,
                spawnWeight = 1.5f,
                description = "Assault rifle ammunition"
            });

            defaultLoot.Add(new LootItem
            {
                itemName = "9mm Ammo",
                lootType = LootType.Ammo,
                quantity = 40,
                spawnWeight = 1.2f,
                description = "SMG ammunition"
            });

            // Health Items
            defaultLoot.Add(new LootItem
            {
                itemName = "Medkit",
                lootType = LootType.Health,
                quantity = 1,
                spawnWeight = 0.6f,
                description = "Restores 100 health"
            });

            defaultLoot.Add(new LootItem
            {
                itemName = "Bandage",
                lootType = LootType.Health,
                quantity = 1,
                spawnWeight = 1.0f,
                description = "Restores 20 health"
            });

            defaultLoot.Add(new LootItem
            {
                itemName = "First Aid Kit",
                lootType = LootType.Health,
                quantity = 1,
                spawnWeight = 0.4f,
                description = "Restores 75 health"
            });

            // Armor
            defaultLoot.Add(new LootItem
            {
                itemName = "Level 1 Helmet",
                lootType = LootType.Armor,
                quantity = 1,
                spawnWeight = 0.8f,
                description = "Basic head protection"
            });

            defaultLoot.Add(new LootItem
            {
                itemName = "Level 2 Vest",
                lootType = LootType.Armor,
                quantity = 1,
                spawnWeight = 0.6f,
                description = "Medium body protection"
            });

            defaultLoot.Add(new LootItem
            {
                itemName = "Level 3 Helmet",
                lootType = LootType.Armor,
                quantity = 1,
                spawnWeight = 0.2f,
                description = "Maximum head protection"
            });

            availableLoot = defaultLoot.ToArray();
        }

        void GenerateSpawnPoints()
        {
            List<LootSpawnPoint> points = new List<LootSpawnPoint>();

            // Generate random spawn points across the map
            for (int i = 0; i < randomSpawnPointCount; i++)
            {
                Vector3 randomPos = GetRandomGroundPosition();
                LootSpawnPoint spawnPoint = new LootSpawnPoint
                {
                    position = randomPos,
                    spawnRadius = Random.Range(1f, 3f),
                    allowedTypes = GetAllLootTypes(),
                    maxItems = Random.Range(1, 4),
                    isHighValue = Random.value < highValueSpawnChance
                };
                points.Add(spawnPoint);
            }

            spawnPoints = points.ToArray();
        }

        Vector3 GetRandomGroundPosition()
        {
            // Generate random position within map bounds
            float mapSize = 400f; // Adjust based on your map size
            Vector3 randomPos = new Vector3(
                Random.Range(-mapSize, mapSize),
                100f, // Start high
                Random.Range(-mapSize, mapSize)
            );

            // Raycast down to find ground
            RaycastHit hit;
            if (Physics.Raycast(randomPos, Vector3.down, out hit, 200f, groundLayer))
            {
                randomPos.y = hit.point.y + spawnHeight;
            }

            return randomPos;
        }

        LootType[] GetAllLootTypes()
        {
            return System.Enum.GetValues(typeof(LootType)) as LootType[];
        }

        void SpawnInitialLoot()
        {
            int itemsSpawned = 0;
            int maxAttempts = totalLootItems * 2;
            int attempts = 0;

            while (itemsSpawned < totalLootItems && attempts < maxAttempts)
            {
                attempts++;

                // Select random spawn point
                LootSpawnPoint spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                // Check if spawn point can accept more items
                int itemsAtPoint = CountItemsAtPosition(spawnPoint.position, spawnPoint.spawnRadius);
                if (itemsAtPoint >= spawnPoint.maxItems)
                    continue;

                // Select loot item
                LootItem selectedLoot = SelectLootItem(spawnPoint);
                if (selectedLoot == null)
                    continue;

                // Spawn the item
                Vector3 spawnPosition = GetSpawnPositionInRadius(spawnPoint.position, spawnPoint.spawnRadius);
                SpawnLootItem(selectedLoot, spawnPosition);

                itemsSpawned++;
            }

            Debug.Log($"Spawned {itemsSpawned} loot items across {spawnPoints.Length} spawn points");
        }

        LootItem SelectLootItem(LootSpawnPoint spawnPoint)
        {
            List<LootItem> availableItems = new List<LootItem>();

            // Filter items based on spawn point restrictions
            foreach (LootItem item in availableLoot)
            {
                bool isAllowed = false;
                foreach (LootType allowedType in spawnPoint.allowedTypes)
                {
                    if (item.lootType == allowedType)
                    {
                        isAllowed = true;
                        break;
                    }
                }

                if (isAllowed)
                {
                    // Adjust weight for high value spawn points
                    float weight = item.spawnWeight;
                    if (spawnPoint.isHighValue && IsHighValueItem(item))
                    {
                        weight *= 2f;
                    }

                    // Add item multiple times based on weight
                    for (int i = 0; i < Mathf.RoundToInt(weight * 10); i++)
                    {
                        availableItems.Add(item);
                    }
                }
            }

            if (availableItems.Count == 0)
                return null;

            return availableItems[Random.Range(0, availableItems.Count)];
        }

        bool IsHighValueItem(LootItem item)
        {
            // Define what constitutes high value loot
            return item.lootType == LootType.Weapon || 
                   (item.lootType == LootType.Armor && item.itemName.Contains("Level 3")) ||
                   (item.lootType == LootType.Health && item.itemName.Contains("Medkit"));
        }

        Vector3 GetSpawnPositionInRadius(Vector3 center, float radius)
        {
            Vector2 randomCircle = Random.insideUnitCircle * radius;
            return center + new Vector3(randomCircle.x, 0, randomCircle.y);
        }

        void SpawnLootItem(LootItem lootItem, Vector3 position)
        {
            GameObject lootObject;

            if (lootItem.itemPrefab != null)
            {
                lootObject = Instantiate(lootItem.itemPrefab, position, Quaternion.identity);
            }
            else
            {
                // Create default loot object
                lootObject = CreateDefaultLootObject(lootItem, position);
            }

            // Add loot component
            LootItemComponent lootComponent = lootObject.GetComponent<LootItemComponent>();
            if (lootComponent == null)
            {
                lootComponent = lootObject.AddComponent<LootItemComponent>();
            }

            lootComponent.Initialize(lootItem);

            spawnedLoot.Add(lootObject);
        }

        GameObject CreateDefaultLootObject(LootItem lootItem, Vector3 position)
        {
            GameObject lootObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lootObject.name = lootItem.itemName;
            lootObject.transform.position = position;
            lootObject.transform.localScale = Vector3.one * 0.5f;

            // Add visual indicator based on loot type
            Renderer renderer = lootObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                switch (lootItem.lootType)
                {
                    case LootType.Weapon:
                        renderer.material.color = Color.red;
                        break;
                    case LootType.Ammo:
                        renderer.material.color = Color.yellow;
                        break;
                    case LootType.Health:
                        renderer.material.color = Color.green;
                        break;
                    case LootType.Armor:
                        renderer.material.color = Color.blue;
                        break;
                    case LootType.Utility:
                        renderer.material.color = Color.cyan;
                        break;
                }
            }

            // Add collider for interaction
            Collider collider = lootObject.GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }

            return lootObject;
        }

        int CountItemsAtPosition(Vector3 position, float radius)
        {
            int count = 0;
            foreach (GameObject loot in spawnedLoot)
            {
                if (loot != null && Vector3.Distance(loot.transform.position, position) <= radius)
                {
                    count++;
                }
            }
            return count;
        }

        public void RemoveLootItem(GameObject lootObject)
        {
            if (spawnedLoot.Contains(lootObject))
            {
                spawnedLoot.Remove(lootObject);
                Destroy(lootObject);
            }
        }

        public List<GameObject> GetLootInRadius(Vector3 position, float radius)
        {
            List<GameObject> nearbyLoot = new List<GameObject>();
            foreach (GameObject loot in spawnedLoot)
            {
                if (loot != null && Vector3.Distance(loot.transform.position, position) <= radius)
                {
                    nearbyLoot.Add(loot);
                }
            }
            return nearbyLoot;
        }

        public void RespawnLoot()
        {
            // Clear existing loot
            foreach (GameObject loot in spawnedLoot)
            {
                if (loot != null)
                    Destroy(loot);
            }
            spawnedLoot.Clear();

            // Respawn new loot
            SpawnInitialLoot();
        }
    }

    // Component for individual loot items
    public class LootItemComponent : MonoBehaviour
    {
        public LootItem lootData;
        public bool isPickedUp = false;

        void Start()
        {
            // Add interaction trigger
            if (GetComponent<Collider>() == null)
            {
                gameObject.AddComponent<BoxCollider>().isTrigger = true;
            }
        }

        public void Initialize(LootItem item)
        {
            lootData = item;
        }

        void OnTriggerEnter(Collider other)
        {
            if (isPickedUp) return;

            if (other.CompareTag("Player"))
            {
                // Handle loot pickup
                var playerInventory = other.GetComponent<PlayerInventory>();
                if (playerInventory != null)
                {
                    if (playerInventory.CanPickupItem(lootData))
                    {
                        playerInventory.PickupItem(lootData);
                        PickupItem();
                    }
                }
            }
        }

        void PickupItem()
        {
            isPickedUp = true;
            
            // Remove from loot system
            var lootSystem = FindObjectOfType<LootSystem>();
            if (lootSystem != null)
            {
                lootSystem.RemoveLootItem(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
