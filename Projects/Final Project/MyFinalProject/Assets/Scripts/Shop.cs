using UnityEngine;
using System.Collections.Generic;

public class Shop : MonoBehaviour
{
    // manage shop buy/sell
    [Header("Shop Items")]
    [SerializeField] private List<ShopItem> shopItems = new List<ShopItem>();

    [Header("Currency")]
    [SerializeField] private ItemData currencyItem;

    [SerializeField] private Inventory playerInventory;

    public System.Action<ShopItem> OnItemPurchased;
    public System.Action OnShopChanged;

    void Start()
    {
        // find player inventory
        if (playerInventory == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerInventory = player.GetComponent<Inventory>();
            }
        }
    }

    public List<ShopItem> GetShopItems()
    {
        // get all shop items
        return new List<ShopItem>(shopItems);
    }

    public ItemData GetCurrencyItem()
    {
        return currencyItem;
    }

    public bool PurchaseItem(ShopItem shopItem, int quantity = 1)
    {
        // purchase item from shop
        if (shopItem == null || playerInventory == null)
            return false;

            // check stock
            if (shopItem.stock >= 0 && shopItem.stock < quantity)
                return false;

        // check player has enough currency
        int totalCost = shopItem.price * quantity;
        if (currencyItem != null)
        {
            if (!playerInventory.HasItem(currencyItem.itemName, totalCost))
            {
                return false;
            }
        }
        
        if (currencyItem != null)
        {
            // remove currency
            playerInventory.RemoveItem(currencyItem.itemName, totalCost);
        }

        // add item to inventory
        bool added = playerInventory.AddItem(shopItem.itemToSell, quantity);

            // update stock
        if (added)
        {
            if (shopItem.stock >= 0)
            {
                shopItem.stock -= quantity;
            }

            OnItemPurchased?.Invoke(shopItem);
            OnShopChanged?.Invoke();
            return true;
        }
        else
        {
            // refund currency if adding item failed
            if (currencyItem != null)
            {
                playerInventory.AddItem(currencyItem, totalCost);
            }
            return false;
        }
    }

    public bool SellItem(ItemData itemData, int quantity = 1)
    {
        // sell item to shop
        if (itemData == null || playerInventory == null)
        {
            return false;
        }

        // check if item is sellable
        if (!itemData.isSellable)
        {
            return false;
        }

        // check if player has item
        if (!playerInventory.HasItem(itemData.itemName, quantity))
        {
            return false;
        }

        // calculate sell price (50% of value)
        int sellPrice = (itemData.value * quantity) / 2;

        // remove item from inventory
        playerInventory.RemoveItem(itemData.itemName, quantity);

        // add currency
        if (currencyItem != null)
        {
            playerInventory.AddItem(currencyItem, sellPrice);
        }
        else
        {
            return false;
        }

        OnShopChanged?.Invoke();
        return true;
    }

    public void AddShopItem(ItemData item, int price, int stock = -1)
    {
        // add new item to shop
        ShopItem shopItem = new ShopItem
        {
            itemToSell = item,
            price = price,
            stock = stock
        };
        shopItems.Add(shopItem);
        OnShopChanged?.Invoke();
    }
}

[System.Serializable]
public class ShopItem
{
    // shop item info
    public ItemData itemToSell;
    public int price;
    public int stock = -1; // -1 = infinite stock
}

