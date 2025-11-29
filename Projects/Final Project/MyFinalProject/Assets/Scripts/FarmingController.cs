using System.Security;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum FarmingTool
{
    None,
    Hoe,
    Water,
    Seed,
    Harvest
}
public class FarmingController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private CropManager cropManager;
    [SerializeField] private SeedPacket selectedSeedPacket;

    [SerializeField] private LayerMask farmingLayerMask;

    private FarmingTool currentTool = FarmingTool.None;
    private CropBlock selectedBlock;

    public System.Action<FarmingTool> OnToolChanged;
    public System.Action<CropBlock> OnBlockSelected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (cropManager == null)
            cropManager = CropManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        // Handle mouse click for farming actions
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            HandleFarmingAction();
        }
    }

    public void SetTool(FarmingTool tool)
    {
        currentTool = tool;
        OnToolChanged?.Invoke(tool);
        Debug.Log($"{tool} selected");
    }

    public void SetSelectedSeed(SeedPacket seedPacket)
    {
        selectedSeedPacket = seedPacket;
        if (seedPacket != null)
        {
            Debug.Log($"{seedPacket.cropName} selected");
        }
    }

    private void HandleFarmingAction()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        if (mouseWorldPos == Vector3.zero) return;

        selectedBlock = cropManager.GetCropBlockPosition(mouseWorldPos);

        if (selectedBlock == null) return;

        OnBlockSelected?.Invoke(selectedBlock);

        // execute action based on current tool
        switch (currentTool)
        {
            case FarmingTool.Hoe:
                if (selectedBlock.CanTill())
                {
                    selectedBlock.TillSoil();
                }
                break;

            case FarmingTool.Water:
                if (selectedBlock.CanWater())
                {
                    selectedBlock.WaterSoil();
                }
                break;

            case FarmingTool.Seed:
                if (selectedBlock.CanPlant() && selectedSeedPacket != null)
                {
                    // plany seed on block
                    if (selectedBlock.PlantSeed(selectedSeedPacket))
                    {
                        // if planted successfully, add to crop manager
                        cropManager.AddToPlantedCrops(selectedBlock);
                    }
                }
                break;

            case FarmingTool.Harvest:
                if (selectedBlock.CanHarvest())
                {
                    selectedBlock.HarvestPlants();
                    cropManager.RemoveFromPlantedCrops(selectedBlock);
                }
                break;    
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        if (playerCamera == null) return Vector3.zero;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = playerCamera.nearClipPlane + 1f;
        Vector3 worldPos = playerCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        return worldPos;
    }
    
    // UI buttons
    public void OnHoeButtonClicked()
    {
        SetTool(FarmingTool.Hoe);
    }

    public void OnWaterButtonClicked()
    {
        SetTool(FarmingTool.Water);
    }

    public void OnSeedButtonClicked()
    {
        SetTool(FarmingTool.Seed);
    }    

    public void OnHarvestButtonClicked()
    {
        SetTool(FarmingTool.Harvest);
    }
}