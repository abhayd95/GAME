using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using FreeFire.Gameplay;

namespace FreeFire.Player
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("Inventory Settings")]
        public int maxInventorySlots = 20;
        public int maxWeaponSlots = 3;
        public int maxArmorSlots = 2; // Helmet and Vest

        [Header("UI References")]
        public Transform inventoryPanel;
        public Transform weaponSlots;
        public Transform armorSlots;
        public GameObject inventorySlotPrefab;
        public Text inventoryWeightText;

        [Header("Quick Use")]
        public Button[] quickUseButtons = new Button[4];
        public Image[] quickUseIcons = new Image[4];

        // Inventory data
        private List<LootItem> inventory = new List<LootItem>();
        private LootItem[] equippedWeapons = new LootItem[3];
        private LootItem[] equippedArmor = new LootItem[2];
        private LootItem[] quickUseItems = new LootItem[4];

        // Weight system
        private float currentWeight = 0f;
        private float maxWeight = 100f;

        // Events
        public System.Action<LootItem> OnItemPickedUp;
        public System.Action<LootItem> OnItemDropped;
        public System.Action<LootItem> OnItemUsed;
        public System.Action<LootItem> OnWeaponEquipped;

        void Start()
        {
            InitializeInventory();
            SetupQuickUseButtons();
            UpdateInventoryUI();
        }

        void InitializeInventory()
        {
            inventory.Clear();
            for (int i = 0; i < maxWeaponSlots; i++)
            {
                equippedWeapons[i] = null;
            }
            for (int i = 0; i < maxArmorSlots; i++)
            {
                equippedArmor[i] = null;
            }
            for (int i = 0; i < 4; i++)
            {
                quickUseItems[i] = null;
            }
        }

        void SetupQuickUseButtons()
        {
            for (int i = 0; i < quickUseButtons.Length; i++)
            {
                int index = i; // Capture for closure
                if (quickUseButtons[i] != null)
                {
                    quickUseButtons[i].onClick.AddListener(() => UseQuickItem(index));
                }
            }
        }

        public bool CanPickupItem(LootItem item)
        {
            // Check if inventory has space
            if (inventory.Count >= maxInventorySlots)
            {
                // Check if we can stack with existing items
                if (!CanStackItem(item))
                {
                    return false;
                }
            }

            // Check weight limit
            if (currentWeight + GetItemWeight(item) > maxWeight)
            {
                return false;
            }

            return true;
        }

        bool CanStackItem(LootItem item)
        {
            // Check if item can be stacked with existing items
            foreach (LootItem existingItem in inventory)
            {
                if (existingItem.itemName == item.itemName && 
                    existingItem.lootType == item.lootType &&
                    existingItem.quantity < GetMaxStackSize(item))
                {
                    return true;
                }
            }
            return false;
        }

        int GetMaxStackSize(LootItem item)
        {
            switch (item.lootType)
            {
                case LootType.Ammo:
                    return 999;
                case LootType.Health:
                    return 5;
                case LootType.Utility:
                    return 10;
                default:
                    return 1;
            }
        }

        float GetItemWeight(LootItem item)
        {
            // Define weight for different item types
            switch (item.lootType)
            {
                case LootType.Weapon:
                    return 5f;
                case LootType.Armor:
                    return 3f;
                case LootType.Health:
                    return 1f;
                case LootType.Ammo:
                    return 0.1f;
                case LootType.Utility:
                    return 2f;
                default:
                    return 1f;
            }
        }

        public void PickupItem(LootItem item)
        {
            if (!CanPickupItem(item))
            {
                Debug.Log("Cannot pickup item: " + item.itemName);
                return;
            }

            // Try to stack with existing items first
            bool stacked = false;
            for (int i = 0; i < inventory.Count; i++)
            {
                LootItem existingItem = inventory[i];
                if (existingItem.itemName == item.itemName && 
                    existingItem.lootType == item.lootType &&
                    existingItem.quantity < GetMaxStackSize(item))
                {
                    existingItem.quantity += item.quantity;
                    stacked = true;
                    break;
                }
            }

            // If couldn't stack, add new item
            if (!stacked)
            {
                LootItem newItem = new LootItem
                {
                    itemName = item.itemName,
                    lootType = item.lootType,
                    quantity = item.quantity,
                    spawnWeight = item.spawnWeight,
                    description = item.description
                };
                inventory.Add(newItem);
            }

            currentWeight += GetItemWeight(item);
            OnItemPickedUp?.Invoke(item);
            UpdateInventoryUI();
        }

        public void DropItem(LootItem item, int quantity = -1)
        {
            if (quantity == -1)
            {
                quantity = item.quantity;
            }

            // Find item in inventory
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].itemName == item.itemName && inventory[i].lootType == item.lootType)
                {
                    if (inventory[i].quantity <= quantity)
                    {
                        // Remove entire stack
                        currentWeight -= GetItemWeight(inventory[i]);
                        inventory.RemoveAt(i);
                    }
                    else
                    {
                        // Reduce quantity
                        inventory[i].quantity -= quantity;
                        currentWeight -= GetItemWeight(item) * (quantity / (float)item.quantity);
                    }
                    break;
                }
            }

            // Create dropped item in world
            CreateDroppedItem(item, quantity);
            OnItemDropped?.Invoke(item);
            UpdateInventoryUI();
        }

        void CreateDroppedItem(LootItem item, int quantity)
        {
            // Create a simple dropped item object
            GameObject droppedItem = GameObject.CreatePrimitive(PrimitiveType.Cube);
            droppedItem.name = item.itemName + " (Dropped)";
            droppedItem.transform.position = transform.position + transform.forward * 2f;
            droppedItem.transform.localScale = Vector3.one * 0.5f;

            // Add loot component
            LootItemComponent lootComponent = droppedItem.AddComponent<LootItemComponent>();
            LootItem droppedLootData = new LootItem
            {
                itemName = item.itemName,
                lootType = item.lootType,
                quantity = quantity,
                spawnWeight = item.spawnWeight,
                description = item.description
            };
            lootComponent.Initialize(droppedLootData);

            // Add to loot system
            var lootSystem = FindObjectOfType<LootSystem>();
            if (lootSystem != null)
            {
                lootSystem.spawnedLoot.Add(droppedItem);
            }
        }

        public void UseItem(LootItem item)
        {
            switch (item.lootType)
            {
                case LootType.Health:
                    UseHealthItem(item);
                    break;
                case LootType.Weapon:
                    EquipWeapon(item);
                    break;
                case LootType.Armor:
                    EquipArmor(item);
                    break;
                case LootType.Ammo:
                    UseAmmoItem(item);
                    break;
                case LootType.Utility:
                    UseUtilityItem(item);
                    break;
            }

            OnItemUsed?.Invoke(item);
        }

        void UseHealthItem(LootItem item)
        {
            var playerHealth = GetComponent<PlayerHealth>();
            if (playerHealth == null) return;

            int healAmount = 0;
            switch (item.itemName)
            {
                case "Medkit":
                    healAmount = 100;
                    break;
                case "First Aid Kit":
                    healAmount = 75;
                    break;
                case "Bandage":
                    healAmount = 20;
                    break;
            }

            if (healAmount > 0)
            {
                playerHealth.Heal(healAmount);
                RemoveItemFromInventory(item, 1);
            }
        }

        void UseAmmoItem(LootItem item)
        {
            var weaponSystem = GetComponent<WeaponSystem>();
            if (weaponSystem == null) return;

            // This would interface with weapon system to add ammo
            // For now, just remove the item
            RemoveItemFromInventory(item, item.quantity);
        }

        void UseUtilityItem(LootItem item)
        {
            // Handle utility items like grenades, smoke, etc.
            RemoveItemFromInventory(item, 1);
        }

        void EquipWeapon(LootItem weapon)
        {
            var weaponSystem = GetComponent<WeaponSystem>();
            if (weaponSystem == null) return;

            // Find empty weapon slot
            for (int i = 0; i < maxWeaponSlots; i++)
            {
                if (equippedWeapons[i] == null)
                {
                    equippedWeapons[i] = weapon;
                    OnWeaponEquipped?.Invoke(weapon);
                    RemoveItemFromInventory(weapon, 1);
                    break;
                }
            }
        }

        void EquipArmor(LootItem armor)
        {
            var playerHealth = GetComponent<PlayerHealth>();
            if (playerHealth == null) return;

            int armorSlot = 0; // Default to helmet
            if (armor.itemName.Contains("Vest"))
            {
                armorSlot = 1; // Vest slot
            }

            if (equippedArmor[armorSlot] == null)
            {
                equippedArmor[armorSlot] = armor;
                
                // Apply armor stats
                int armorValue = 0;
                if (armor.itemName.Contains("Level 1"))
                    armorValue = 25;
                else if (armor.itemName.Contains("Level 2"))
                    armorValue = 50;
                else if (armor.itemName.Contains("Level 3"))
                    armorValue = 75;

                playerHealth.AddArmor(armorValue);
                RemoveItemFromInventory(armor, 1);
            }
        }

        void RemoveItemFromInventory(LootItem item, int quantity)
        {
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].itemName == item.itemName && inventory[i].lootType == item.lootType)
                {
                    if (inventory[i].quantity <= quantity)
                    {
                        currentWeight -= GetItemWeight(inventory[i]);
                        inventory.RemoveAt(i);
                    }
                    else
                    {
                        inventory[i].quantity -= quantity;
                        currentWeight -= GetItemWeight(item) * (quantity / (float)item.quantity);
                    }
                    break;
                }
            }
            UpdateInventoryUI();
        }

        void UseQuickItem(int slotIndex)
        {
            if (quickUseItems[slotIndex] != null)
            {
                UseItem(quickUseItems[slotIndex]);
            }
        }

        public void SetQuickUseItem(int slotIndex, LootItem item)
        {
            if (slotIndex >= 0 && slotIndex < quickUseItems.Length)
            {
                quickUseItems[slotIndex] = item;
                UpdateQuickUseUI();
            }
        }

        void UpdateInventoryUI()
        {
            // Update inventory slots
            if (inventoryPanel != null)
            {
                // Clear existing slots
                foreach (Transform child in inventoryPanel)
                {
                    Destroy(child.gameObject);
                }

                // Create new slots
                foreach (LootItem item in inventory)
                {
                    CreateInventorySlot(item);
                }
            }

            // Update weight display
            if (inventoryWeightText != null)
            {
                inventoryWeightText.text = $"{currentWeight:F1}/{maxWeight}";
            }

            UpdateQuickUseUI();
        }

        void CreateInventorySlot(LootItem item)
        {
            if (inventorySlotPrefab == null) return;

            GameObject slot = Instantiate(inventorySlotPrefab, inventoryPanel);
            
            // Setup slot UI
            Image icon = slot.GetComponentInChildren<Image>();
            Text nameText = slot.GetComponentInChildren<Text>();
            Text quantityText = slot.transform.Find("Quantity")?.GetComponent<Text>();

            if (icon != null && item.itemIcon != null)
            {
                icon.sprite = item.itemIcon;
            }

            if (nameText != null)
            {
                nameText.text = item.itemName;
            }

            if (quantityText != null)
            {
                quantityText.text = item.quantity > 1 ? item.quantity.ToString() : "";
            }

            // Add click handlers
            Button slotButton = slot.GetComponent<Button>();
            if (slotButton != null)
            {
                slotButton.onClick.AddListener(() => UseItem(item));
            }
        }

        void UpdateQuickUseUI()
        {
            for (int i = 0; i < quickUseIcons.Length; i++)
            {
                if (quickUseIcons[i] != null)
                {
                    if (quickUseItems[i] != null && quickUseItems[i].itemIcon != null)
                    {
                        quickUseIcons[i].sprite = quickUseItems[i].itemIcon;
                        quickUseIcons[i].color = Color.white;
                    }
                    else
                    {
                        quickUseIcons[i].sprite = null;
                        quickUseIcons[i].color = Color.clear;
                    }
                }
            }
        }

        // Public getters
        public List<LootItem> GetInventory() => inventory;
        public LootItem[] GetEquippedWeapons() => equippedWeapons;
        public LootItem[] GetEquippedArmor() => equippedArmor;
        public float GetCurrentWeight() => currentWeight;
        public float GetMaxWeight() => maxWeight;
        public bool IsInventoryFull() => inventory.Count >= maxInventorySlots;
        public bool IsOverweight() => currentWeight > maxWeight;

        // Search methods
        public LootItem FindItem(string itemName)
        {
            return inventory.Find(item => item.itemName == itemName);
        }

        public List<LootItem> FindItemsByType(LootType type)
        {
            return inventory.FindAll(item => item.lootType == type);
        }

        public int GetItemCount(string itemName)
        {
            int count = 0;
            foreach (LootItem item in inventory)
            {
                if (item.itemName == itemName)
                {
                    count += item.quantity;
                }
            }
            return count;
        }
    }
}
