using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;

public class InventoryUI : MonoBehaviour
{

    [SerializeField] private Transform inventoryPanel;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Button toggleButton;
    [SerializeField] private GameObject inventoryWindow;

    [SerializeField] private Inventory inventory;

    private List<GameObject> slotInstances = new List<GameObject>();
    private bool isOpen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // find inventory
        if (inventory == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                inventory = player.GetComponent<Inventory>();
            }

            if (inventory == null)
            {
                inventory = Object.FindFirstObjectByType<Inventory>();
            }
        }

        if (inventory != null)
        {
            // inventory changes
            inventory.OnInventoryChanged += RefreshInventory;
        }

        if (toggleButton != null)
        {
            // set toggle button
            toggleButton.onClick.AddListener(ToggleInventory);
        }

        if (inventoryWindow != null)
        {
            inventoryWindow.SetActive(isOpen);
        }

        RefreshInventory();
    }

   
    void OnDestroy()
    {
        if (inventory != null )
        {
            inventory.OnInventoryChanged -= RefreshInventory;
        }
    }

    private void RefreshInventory()
    {
        // refreshes inventory display
        if (inventory == null || inventoryPanel == null)
        {
            return;
        }

        // clear existing slots
        foreach (GameObject slot in slotInstances)
        {
            if (slot != null)
            {
                Destroy(slot);
            }
        }
        slotInstances.Clear();

        // get all items from inventory
        List<InventoryItem> items = inventory.GetAllItems();

        // CREATE SLOTS FOR ITEMS FIRST
        foreach (InventoryItem item in items)
        {
            CreateSlot(item);
        }

        // create empty slots for remaining space
        int emptySlots = inventory.MaxSlots - items.Count;
        for (int i = 0; i < emptySlots; i++)
        {
            CreateEmptySlot();
        }
    }

    private void CreateSlot(InventoryItem item)
    {
        if (slotPrefab == null)
        {
            // create slot if no prefab
            GameObject slot = new GameObject("InventorySlot");
            slot.transform.SetParent(inventoryPanel, false);

            // add image for icon
            Image iconImage = slot.AddComponent<Image>();
            if (item.itemData.icon != null)
            {
                iconImage.sprite = item.itemData.icon;
            }

                // add text for quantity
                GameObject textObj = new GameObject("QuantityText");
                textObj.transform.SetParent(slot.transform, false);
                TextMeshProUGUI quantityText = textObj.AddComponent<TextMeshProUGUI>();
                quantityText.text = item.quantity > 1 ? item.quantity.ToString() : "";
                quantityText.fontSize = 14;
                quantityText.alignment = TextAlignmentOptions.BottomRight;

                RectTransform textRect = textObj.GetComponent<RectTransform>();
                textRect.anchorMin = new Vector2(1, 0);
                textRect.anchorMax = new Vector2(1, 0);
                textRect.pivot = new Vector2(1, 0);
                textRect.anchoredPosition = new Vector2(-5, 5);

                slotInstances.Add(slot);
            }
            else
            {
                // use prefab
                GameObject slot = Instantiate(slotPrefab, inventoryPanel);

                // find icon image
                Image iconImage = slot.GetComponentInChildren<Image>();
                if (iconImage != null && item.itemData.icon != null)
                {
                    iconImage.sprite = item.itemData.icon;
                }

                // find quantity text
                TextMeshProUGUI quantityText = slot.GetComponentInChildren<TextMeshProUGUI>();
                if (quantityText != null)
                {
                    quantityText.text = item.quantity > 1 ? item.quantity.ToString() : "";
                }

                slotInstances.Add(slot);
            }
        }

    public void ToggleInventory()
    {
        // toggle inventory window open/close
        isOpen = !isOpen;
        if (inventoryWindow != null)
        {
            inventoryWindow.SetActive(isOpen);
        }

        if (isOpen)
        {
            RefreshInventory();
        }
    }

    public void OpenInventory()
    {
        // open inventory window
        isOpen = true;
        if (inventoryWindow != null)
        {
            inventoryWindow.SetActive(true);
        }
        RefreshInventory();
    }

    public void CloseInventory()
    {
        isOpen = false;
        if (inventoryWindow != null)
        {
            inventoryWindow.SetActive(false);
        }
    }

    private void CreateEmptySlot()
    {
        GameObject slot = new GameObject("EmptySlot");
        slot.transform.SetParent(inventoryPanel, false);

        Image bgImage = slot.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);

        slotInstances.Add(slot);
    }
}
