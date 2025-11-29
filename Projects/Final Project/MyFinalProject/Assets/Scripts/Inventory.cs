using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Inventory : MonoBehaviour
{
    // variables
    [SerializeField] private int maxInventorySlots = 100;

    private List<InventoryItem> items = new List<InventoryItem> ();

    public System.Action<InventoryItem> OnItemAdded;
    public System.Action<InventoryItem> OnItemRemoved;
    public System.Action OnInventoryChanged;

    public int ItemCount => items.Count;
    public int MaxSlots => maxInventorySlots;

    void Start()
    {
        // initialize empty inventory
        items = new List<InventoryItem>();
    }

    public bool AddItem(ItemData itemData, int quantity = 1)
    {
        // add item to inventory
        if (itemData == null)
        {
            return false;
        }

        // check if exists and can stack
        InventoryItem existingItem = items.FirstOrDefault(i => i.itemData.itemName == itemData.itemName);

        if (existingItem != null && existingItem.CanStackMore(quantity))
        {
            // add to existing stack
            existingItem.AddQuantity(quantity);
            OnItemAdded?.Invoke(existingItem);
            OnInventoryChanged?.Invoke();
            return true;
        }
        else if (existingItem != null && !existingItem.CanStackMore(quantity))
        {
            // partial stack 
            int canAdd = existingItem.itemData.maxStackSize - existingItem.quantity;
            if (canAdd > 0)
            {
                existingItem.AddQuantity(canAdd);
                OnItemAdded?.Invoke(existingItem);
                OnInventoryChanged?.Invoke();

                // add remainder as new stack
                if (items.Count < maxInventorySlots)
                {
                    int remainder = quantity - canAdd;
                    InventoryItem newItem = new InventoryItem(itemData, remainder);
                    items.Add(newItem);
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
            return false;
        }
        else
        {
            // check space for new item
            if (items.Count >= maxInventorySlots)
            {
                Debug.LogWarning("Inventory full!");
                return false;
            }

            // create new inventory item
            InventoryItem newItem  = new InventoryItem(itemData, quantity);
            items.Add(newItem);
            OnItemAdded?.Invoke(newItem);
            OnInventoryChanged?.Invoke();
            Debug.Log($"Added {quantity} x {itemData.itemName} to inventory.");
            return true;
        }
    }

    public bool RemoveItem(string itemName, int quantity = 1)
    {
        // remove item from inventory
        InventoryItem item = items.FirstOrDefault(items => items.itemData.itemName == itemName);

        if (item == null)
        {
            return false;
        }

        if (item.RemoveQuantity(quantity))
        {
            // if quantity is 0, remove from list
            if (item.quantity <= 0)
            {
                items.Remove(item);
            }

            OnItemRemoved?.Invoke(item);
            OnInventoryChanged?.Invoke();
            return true;
        }
        return false;
    }

    public int GetItemCount(string itemName)
    {
        // get quantity of item in inventory
        InventoryItem item = items.FirstOrDefault(items => items.itemData.itemName == itemName);
        return item != null ? item.quantity : 0;
    }

    public bool HasItem(string itemName, int quantity = 1)
    {
        // check if inventory has item
        return GetItemCount(itemName) >= quantity;
    }

    public List<InventoryItem> GetAllItems()
    {
        // get all items in inventory
        return new List<InventoryItem>(items);
    }
    public InventoryItem GetItemAt(int index)
    {
        // get items by index
        if (index >= 0 && index < items.Count)
        {
            return items[index];
        }
        return null;
    }

    public void ClearInventory()
    {
        items.Clear();
        OnInventoryChanged?.Invoke();
    }
}
