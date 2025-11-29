using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ShopUi : MonoBehaviour
{
    // variables
    [SerializeField] private GameObject shopWindow;
    [SerializeField] private Transform shopItemsContainer;
    [SerializeField] private GameObject shopItemPrefab;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI currencyText;

    [SerializeField] private Shop shop;
    [SerializeField] Inventory playerInventory;

    private List<GameObject> shopItemInstances = new List<GameObject>();
    private bool isOpen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // find shop
        if (shop == null)
        {
            shop = Object.FindFirstObjectByType<Shop>();
        }

        // find inventory
        if (playerInventory == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerInventory = player.GetComponent<Inventory>();
            }
        }

        // update shop UI when shop changes
        if (shop != null)
        {
            shop.OnShopChanged += RefreshShop;
        }

        // update inventory changes
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged += UpdateCurrencyDisplay;
        }

        // close button listener
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseShop);
        }

        // initialize shop UI
        if (shopWindow != null)
        {
            shopWindow.SetActive(isOpen);
        }

        RefreshShop();
        UpdateCurrencyDisplay();
    }

    void OnDestroy()
    {
        if (shop != null)
        {
            shop.OnShopChanged -= RefreshShop;
        }

        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= UpdateCurrencyDisplay;
        }
    }

    public void OpenShop()
    {
        // open shop window
        isOpen = true;
        if (shopWindow != null)
        {
            shopWindow.SetActive(true);
        }

        RefreshShop();
        UpdateCurrencyDisplay();
    }

    public void CloseShop()
    {
        // close shop window
        isOpen = false;
        if (shopWindow != null)
        {
            shopWindow.SetActive(false);
        }
    }

    public void ToggleShop()
    {
        if (isOpen)
        {
            CloseShop();
        }
        else
        {
            OpenShop();
        }
    }

    private void RefreshShop()
    {
        // refresh shop display
        if (shop == null || shopItemsContainer == null)
        {
            return;
        }

        // clear existing items
        foreach (GameObject item in shopItemInstances)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }

        shopItemInstances.Clear();

        // get shop items
        List<ShopItem> items = shop.GetShopItems();

        // create UI for each item
        foreach (ShopItem shopItem in items)
        {
            CreateShopItemUI(shopItem);
        }
    }

    private void CreateShopItemUI(ShopItem shopItem)
    {
        GameObject itemUI;

        if (shopItemPrefab != null)
        {
            itemUI = Instantiate(shopItemPrefab, shopItemsContainer);
        }

        else
        {
            // create UI if no prefab
            itemUI = new GameObject("ShopItem");
            itemUI.transform.SetParent(shopItemsContainer, false);

            // add background
            Image bg = itemUI.AddComponent<Image>();
            bg.color = new Color(0.3f, 0.3f, 0.3f, 1f);

            RectTransform rect = itemUI.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 100);
        }

        // create icon
        Image iconImage = itemUI.GetComponentInChildren<Image>();
        if (iconImage == null)
        {
            GameObject iconObj = new GameObject("Icon");
            iconObj.transform.SetParent(itemUI.transform, false);
            iconImage = iconObj.AddComponent<Image>();
            RectTransform iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0, 0.5f);
            iconRect.anchorMax = new Vector2(0, 0.5f);
            iconRect.pivot = new Vector2(0, 0.5f);
            iconRect.sizeDelta = new Vector2(64, 64);
            iconRect.anchoredPosition = new Vector2(10, 0);
        }

        if (shopItem.itemToSell != null && shopItem.itemToSell.icon != null)
        {
            iconImage.sprite = shopItem.itemToSell.icon;
        }

        // create text
        TextMeshProUGUI nameText = itemUI.GetComponentInChildren<TextMeshProUGUI>();
        if (nameText == null)
        {
            GameObject nameObj = new GameObject("NameText");
            nameObj.transform.SetParent(itemUI.transform, false);
            nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.fontSize = 16;
            RectTransform nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0.3f, 0.6f);
            nameRect.anchorMax = new Vector2(1f, 1f);
            nameRect.offsetMin = new Vector2(10, 0);
            nameRect.offsetMax = new Vector2(-10, 0);
        }
        
        if (shopItem != null)
        {
            nameText.text = shopItem.itemToSell.itemName;
        }

        // create price text
        TextMeshProUGUI priceText = null;
        TextMeshProUGUI[] texts = itemUI.GetComponentsInChildren<TextMeshProUGUI>();
        if (texts.Length > 1)
        {
            priceText = texts[1];
        }

        if (priceText == null)
        {
            GameObject priceObj = new GameObject("PriceText");
            priceObj.transform.SetParent(itemUI.transform, false);
            priceText = priceObj.AddComponent<TextMeshProUGUI>();
            priceText.fontSize = 14;
            RectTransform priceRect = priceObj.GetComponent<RectTransform>();
            priceRect.anchorMin = new Vector2(0.3f, 0f);
            priceRect.anchorMax = new Vector2(1f, 0.4f);
            priceRect.offsetMin = new Vector2(10, 0);
            priceRect.offsetMax = new Vector2(-10, 0);
        }

        priceText.text = $"Price: {shopItem.price}";

        // add buy button
        Button buyButton = itemUI.GetComponentInChildren<Button>();
        if (buyButton == null)
        {
            GameObject buttonObj = new GameObject("BuyButton");
            buttonObj.transform.SetParent(itemUI.transform, false);
            buyButton = buttonObj.AddComponent<Button>();
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.8f, 0.2f, 1f);

            GameObject buttonTextObj = new GameObject("Text");
            buttonTextObj.transform.SetParent(buttonObj.transform, false);
            TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = "Buy";
            buttonText.fontSize = 14;
            buttonText.alignment = TextAlignmentOptions.Center;

            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.8f, 0.1f);
            buttonRect.anchorMax = new Vector2(1f, 0.9f);
            buttonRect.offsetMin = new Vector2(-10, 5);
            buttonRect.offsetMax = new Vector2(-5, -5);

            RectTransform textRect = buttonTextObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
        }

        // buy button
        ShopItem itemToBuy = shopItem;
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => OnBuyButtonClicked(itemToBuy));

        shopItemInstances.Add(itemUI);
    }

    private void OnBuyButtonClicked(ShopItem shopItem)
    {
        // handle buy button click
        if (shop != null)
        {
            shop.PurchaseItem(shopItem, 1);
            RefreshShop();
            UpdateCurrencyDisplay();
        }
    }

    private void UpdateCurrencyDisplay()
    {
        // update currency display
        if (currencyText == null || playerInventory == null || shop == null)
        {
            return;
        }

        // get currency from shop
        ItemData currency = shop.GetCurrencyItem();

        if (currency != null)
        {
            int currencyAmount = playerInventory.GetItemCount(currency.itemName);
            currencyText.text = $"{currency.itemName}: {currencyAmount}";
        }
        else
        {
            currencyText.text = "Currency: X";
        }
    }
}
