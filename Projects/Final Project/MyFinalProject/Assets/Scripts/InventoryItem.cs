using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public ItemData itemData;
    public int quantity;

    public InventoryItem(ItemData data, int qty = 1)
    {
        itemData = data;
        quantity = qty;
    }

    public void AddQuantity(int amount)
    {
        // add quantity to item
        if (itemData.maxStackSize > 0)
        {
            quantity = Mathf.Min(quantity + amount, itemData.maxStackSize);
        }
        else
        {
            quantity += amount;
        }
    }

    public bool RemoveQuantity(int amount)
    {
        // remove quantity from item
        if (quantity >= amount)
        {
            quantity -= amount;
            return true;
        }
        return false;
    }

    public bool CanStackMore(int amount)
    {
        // check if item can stack more
        if (itemData.maxStackSize <= 0) return true;
        return (quantity + amount) <= itemData.maxStackSize;
    }
}
