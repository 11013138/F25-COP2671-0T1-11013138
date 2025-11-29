# Step-by-Step Unity Setup Instructions
## Final Project - 2D Farm Simulation

After reviewing your scripts, here are the step-by-step instructions to complete your Unity project setup and fix critical issues.

---

## 🔴 CRITICAL CODE FIXES (Must Fix First)

### Fix 1: FarmingController - Add Mouse Click Input
**Problem:** `HandleFarmingAction()` is never called, so farming tools don't work.

**Location:** `Assets/Scripts/FarmingController.cs`

**Fix:** In the `Update()` method, add mouse click detection:
```csharp
void Update()
{
    if (Input.GetMouseButtonDown(0)) // Left mouse button
    {
        HandleFarmingAction();
    }
}
```

### Fix 2: FarmingController - Complete Seed Planting Logic
**Problem:** When using the Seed tool, `PlantSeed()` is never called.

**Location:** `Assets/Scripts/FarmingController.cs` line 87-91

**Fix:** Replace the Seed case in `HandleFarmingAction()`:
```csharp
case FarmingTool.Seed:
    if (selectedBlock.CanPlant() && selectedSeedPacket != null)
    {
        if (selectedBlock.PlantSeed(selectedSeedPacket))
        {
            cropManager.AddToPlantedCrops(selectedBlock);
        }
    }
    break;
```

### Fix 3: InventoryUI - Display Items in Inventory
**Problem:** `RefreshInventory()` creates empty slots but never calls `CreateSlot()` for actual items.

**Location:** `Assets/Scripts/InventoryUI.cs` line 67-94

**Fix:** In `RefreshInventory()`, add item slot creation:
```csharp
private void RefreshInventory()
{
    // ... existing clear code ...
    
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
```

### Fix 4: CropBlock - Fix MonoBehaviour Issue
**Problem:** `CropBlock` inherits from `MonoBehaviour` but is instantiated as a regular class.

**Location:** `Assets/Scripts/CropBlock.cs`

**Fix:** Remove `: MonoBehaviour` from the class declaration:
```csharp
public class CropBlock  // Remove : MonoBehaviour
{
    // ... rest of class ...
}
```

### Fix 5: InventoryItem - Fix MonoBehaviour Issue
**Problem:** `InventoryItem` inherits from `MonoBehaviour` but is used as a data class.

**Location:** `Assets/Scripts/InventoryItem.cs`

**Fix:** Remove `: MonoBehaviour` and make it a `[System.Serializable]` class:
```csharp
[System.Serializable]
public class InventoryItem  // Remove : MonoBehaviour
{
    public ItemData itemData;
    public int quantity;
    
    // Constructor stays the same
    public InventoryItem(ItemData data, int qty = 1)
    {
        itemData = data;
        quantity = qty;
    }
    // ... rest of methods ...
}
```

### Fix 6: ShopUi - Fix Syntax Error
**Problem:** Line 234 has `if (buyButton = null)` which should be `== null`.

**Location:** `Assets/Scripts/ShopUi.cs` line 234

**Fix:** Change to:
```csharp
if (buyButton == null)  // Use == not =
```

### Fix 7: CropBlock - Add Visual Feedback for Tilled Soil
**Problem:** `TillSoil()` doesn't update the tilemap to show tilled soil.

**Location:** `Assets/Scripts/CropBlock.cs` line 37-44

**Fix:** Add tilemap update (you'll need a tilled soil tile/sprite):
```csharp
public void TillSoil()
{
    if (currentState == CropState.Empty)
    {
        currentState = CropState.Tilled;
        // TODO: Set a tilled soil tile/sprite on the tilemap
        // Example: cropTilemap.SetTile(cellPosition, tilledSoilTile);
    }
}
```

---

## 🎮 UNITY SCENE SETUP

### Step 1: Scene Hierarchy Setup

1. **Open your main scene** (PersistentScene.unity)

2. **Create/Verify Game Managers:**
   - Create empty GameObject named "GameManager"
     - Add `TimeManager` component
     - Add `DayNightLighting` component
     - Add `DayNightEvents` component
   - Create empty GameObject named "CropManager"
     - Add `CropManager` component
     - Add `Grid` component (should auto-add due to RequireComponent)

3. **Setup Player:**
   - Find or create Player GameObject (tagged "Player")
   - Add `PlayerController` component
   - Add `Rigidbody2D` component (set to Dynamic, freeze rotation Z)
   - Add `Inventory` component
   - Add `FarmingController` component
   - Add `Collider2D` (CircleCollider2D or BoxCollider2D) for harvestable collection

4. **Setup Camera:**
   - Find Main Camera
   - Add `CameraController` component
   - Assign Player transform to CameraController's "Player" field

5. **Setup Tilemap:**
   - Create Tilemap GameObject (if not exists)
   - Create a child Tilemap named "FarmingTilemap"
   - Assign this Tilemap to CropManager's "Farming Tilemap" field

### Step 2: Lighting Setup

1. **Create Global Light:**
   - Create empty GameObject named "GlobalLight"
   - Add `Light2D` component (Universal Render Pipeline)
   - Set Light Type to "Global"
   - Assign this Light2D to DayNightLighting's "Main Light" field

2. **Configure AnimationCurve:**
   - Select GameManager (with DayNightLighting)
   - In Inspector, find "Light Intensity Curve"
   - Click the curve field
   - Set keyframes:
     - 0.0 (midnight): 0.1
     - 0.25 (6 AM): 0.1
     - 0.3 (7 AM): 0.8
     - 0.5 (noon): 1.0
     - 0.7 (5 PM): 0.8
     - 0.75 (6 PM): 0.3
     - 1.0 (midnight): 0.1

3. **Configure Gradient:**
   - Find "Light Color Gradient" field
   - Click gradient field
   - Set colors:
     - 0.0 (midnight): Dark blue (0.2, 0.2, 0.4)
     - 0.25 (6 AM): Orange/yellow (1.0, 0.7, 0.5)
     - 0.5 (noon): White (1.0, 1.0, 1.0)
     - 0.75 (6 PM): Orange (1.0, 0.6, 0.3)
     - 1.0 (midnight): Dark blue (0.2, 0.2, 0.4)

### Step 3: SeedPacket Assets Setup

1. **Create SeedPacket Assets:**
   - Right-click in Project: `Assets/SeedPackets/`
   - Create > Scriptable Objects > SeedPacket
   - Create at least 4 seed packets (e.g., Carrot, Wheat, Potato, Onion)

2. **Configure Each SeedPacket:**
   - **Crop Name:** Set descriptive name
   - **Growth Sprites:** Assign 4 sprites (stage 0, 1, 2, 3)
   - **Cover Image:** Assign UI sprite for shop/inventory
   - **Harvestable Prefab:** Assign prefab (create these next)
   - **Growth Time Per Stage:** Set (e.g., 10 seconds)
   - **Total Growth Time:** Set (e.g., 40 seconds)

### Step 4: Harvestable Prefabs Setup

1. **Create Harvestable Prefabs:**
   - For each crop type, create a prefab:
     - Create empty GameObject
     - Add `Harvestable` component
     - Add `SpriteRenderer` with crop sprite
     - Add `Collider2D` (CircleCollider2D, set as Trigger)
     - Add `Rigidbody2D` (set to Kinematic)

2. **Configure Harvestable Component:**
   - **Item Data:** Assign corresponding ItemData asset
   - **Item Name:** Set (should match ItemData)
   - **Value:** Set sell value
   - **Collect Effect:** (Optional) Assign particle effect prefab

3. **Assign to SeedPackets:**
   - Open each SeedPacket asset
   - Assign the corresponding harvestable prefab to "Harvestable Prefab" field

### Step 5: ItemData Assets Setup

1. **Create ItemData Assets:**
   - Right-click: `Assets/Items/`
   - Create > Scriptable Objects > ItemData
   - Create items for each crop (Carrot, Wheat, etc.)
   - Create a "Coin" ItemData for currency

2. **Configure Each ItemData:**
   - **Item Name:** Set name
   - **Icon:** Assign sprite for inventory display
   - **Value:** Set sell value
   - **Max Stack Size:** Set (e.g., 99)
   - **Is Sellable:** Check if item can be sold

### Step 6: Inventory UI Setup

1. **Create Canvas:**
   - Right-click Hierarchy > UI > Canvas
   - Set Canvas Scaler to "Scale With Screen Size"
   - Reference Resolution: 1920x1080

2. **Create Inventory Panel:**
   - Right-click Canvas > UI > Panel (name it "InventoryPanel")
   - Add `InventoryUI` component
   - Set initially inactive

3. **Create Inventory Window:**
   - Create child GameObject "InventoryWindow"
   - Add Image component (background)
   - Add `Grid Layout Group` component
   - Set Cell Size: 64x64
   - Set Spacing: 5x5
   - Assign to InventoryUI's "Inventory Panel" field

4. **Create Slot Prefab (Optional but Recommended):**
   - Create GameObject "InventorySlot"
   - Add Image component (background)
   - Create child "Icon" GameObject with Image component
   - Create child "QuantityText" GameObject with TextMeshProUGUI
   - Position quantity text in bottom-right corner
   - Make it a Prefab
   - Assign to InventoryUI's "Slot Prefab" field

5. **Create Toggle Button:**
   - Create Button as child of Canvas
   - Name it "InventoryToggleButton"
   - Assign to InventoryUI's "Toggle Button" field
   - Assign InventoryWindow to "Inventory Window" field

6. **Connect Inventory:**
   - Assign Player's Inventory component to InventoryUI's "Inventory" field

### Step 7: Toolbar UI Setup

1. **Create Toolbar Panel:**
   - Right-click Canvas > UI > Panel (name it "ToolbarPanel")
   - Add `ToolbarController` component
   - Set Horizontal Layout Group component
   - Set Child Alignment: Middle Center
   - Set Spacing: 10

2. **Create Tool Buttons:**
   - Create 4 Buttons as children of ToolbarPanel:
     - "HoeButton" (with icon/text "Hoe")
     - "WaterButton" (with icon/text "Water")
     - "SeedButton" (with icon/text "Plant")
     - "HarvestButton" (with icon/text "Harvest")

3. **Connect Buttons:**
   - Assign each button to ToolbarController's corresponding field
   - Assign FarmingController to ToolbarController's "Farming Controller" field

### Step 8: Shop UI Setup

1. **Create Shop GameObject:**
   - Create empty GameObject "Shop"
   - Add `Shop` component

2. **Configure Shop:**
   - Assign Currency Item (Coin ItemData)
   - Assign Player's Inventory to "Player Inventory" field
   - Add Shop Items in Inspector (ItemData, Price, Stock)

3. **Create Shop UI:**
   - Create Panel "ShopWindow" as child of Canvas
   - Add `ShopUi` component
   - Set initially inactive

4. **Create Shop Items Container:**
   - Create child "ShopItemsContainer"
   - Add Vertical Layout Group
   - Assign to ShopUi's "Shop Items Container" field

5. **Create Shop Item Prefab (Optional):**
   - Create GameObject with Image, Icon Image, Name Text, Price Text, Buy Button
   - Make it a Prefab
   - Assign to ShopUi's "Shop Item Prefab" field

6. **Create Close Button:**
   - Create Button as child of ShopWindow
   - Assign to ShopUi's "Close Button" field

7. **Create Currency Display:**
   - Create TextMeshProUGUI for currency
   - Assign to ShopUi's "Currency Text" field

8. **Connect References:**
   - Assign Shop component to ShopUi's "Shop" field
   - Assign Player Inventory to ShopUi's "Player Inventory" field

### Step 9: Time-Aware Interactions (Optional Enhancement)

**Add to CropBlock.cs:**
```csharp
public bool CanGrow(TimeManager timeManager)
{
    // Only grow during day (6 AM to 6 PM)
    return timeManager.currentHour >= 6f && timeManager.currentHour <= 18f;
}
```

**Update CropBlock.UpdateGrowth():**
```csharp
public void UpdateGrowth(float deltaTime, TimeManager timeManager)
{
    if (currentState == CropState.Planted || currentState == CropState.Growing)
    {
        if (seedPacket == null) return;
        
        // Only grow during day
        if (!CanGrow(timeManager)) return;
        
        // ... rest of growth logic ...
    }
}
```

**Update CropManager.UpdateCropGrowth():**
```csharp
private void UpdateCropGrowth()
{
    TimeManager timeManager = FindObjectOfType<TimeManager>();
    
    for (int i = plantedCrops.Count - 1; i >= 0; i--)
    {
        if (plantedCrops[i] != null)
        {
            plantedCrops[i].UpdateGrowth(growthUpdateInterval, timeManager);
            // ... rest of code ...
        }
    }
}
```

### Step 10: Seed Selection UI (For Planting)

1. **Create Seed Selection Panel:**
   - Create Panel "SeedSelectionPanel"
   - Add Horizontal Layout Group
   - Create buttons for each SeedPacket

2. **Add Seed Selection to FarmingController:**
   - Add method: `public void SelectSeedFromUI(SeedPacket seed)`
   - Connect seed buttons to this method

3. **Update Seed Button in Toolbar:**
   - When clicked, show seed selection panel
   - Player selects seed, then can plant

---

## ✅ TESTING CHECKLIST

After setup, test each system:

- [ ] Day/Night cycle transitions smoothly
- [ ] Lighting changes with time (AnimationCurve and Gradient work)
- [ ] Player can move with WASD/Arrow keys
- [ ] Camera follows player
- [ ] Clicking with Hoe tool tills soil (visual feedback)
- [ ] Clicking with Water tool waters tilled/planted soil
- [ ] Selecting seed and clicking plants the seed
- [ ] Crops grow over time (only during day if time-aware)
- [ ] Crops show growth stages visually
- [ ] Harvesting spawns harvestable items
- [ ] Walking into harvestables adds them to inventory
- [ ] Inventory UI shows collected items
- [ ] Inventory UI updates when items are added
- [ ] Shop UI displays items correctly
- [ ] Can purchase items from shop (if have currency)
- [ ] Can sell items to shop
- [ ] Currency updates in shop UI

---

## 🐛 COMMON ISSUES & SOLUTIONS

**Issue:** Crops don't grow
- Check CropManager is updating
- Check crops are added to plantedCrops list
- Check TimeManager is running
- Check isWatered flag is true

**Issue:** Can't interact with crops
- Check FarmingController has camera reference
- Check mouse click is calling HandleFarmingAction()
- Check CropManager has tilemap assigned

**Issue:** Inventory doesn't show items
- Check RefreshInventory() calls CreateSlot()
- Check InventoryUI has inventory reference
- Check slot prefab structure matches code

**Issue:** Shop doesn't work
- Check Shop has currency ItemData assigned
- Check Shop has player inventory reference
- Check ShopUi has Shop reference
- Fix syntax error on line 234 (== not =)

---

## 📝 NOTES

- Make sure all ScriptableObject assets are in the correct folders
- Ensure all prefabs are properly configured before assigning to scripts
- Test each system individually before integrating
- Use Debug.Log() statements to track issues
- Check Console for errors after each setup step

Good luck with your project! 🌾

