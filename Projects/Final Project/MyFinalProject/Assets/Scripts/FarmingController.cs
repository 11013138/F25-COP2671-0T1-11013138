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
    //variables

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
                if (selectedSeedPacket == null)
                {
                    Debug.LogWarning("No seed selected.");
                    break;
                }

                // check light before planting
                if (DayNightLighting.Instance != null && DayNightLighting.Instance.currentLightFactor < selectedSeedPacket.minLightRequirement)
                {
                    Debug.LogWarning($"Too dark to plant {selectedSeedPacket.cropName}. Wait for more light.");
                    break;
                }

                if (selectedBlock.CanPlant())
                {
                    if (selectedBlock.PlantSeed(selectedSeedPacket))
                    {
                        cropManager.AddToPlantedCrops(selectedBlock);
                    }
                }
                break;

            case FarmingTool.Harvest:
                // harvesting requires daytime light
                if (selectedBlock.CanHarvest())
                {
                    if (DayNightLighting.Instance != null && DayNightLighting.Instance.currentLightFactor < 0.15f)
                    {
                        Debug.LogWarning("Too dark to harvest. Come back during the day.");
                        break;
                    }

                    if (selectedBlock.HarvestPlants())
                    {
                        cropManager.RemoveFromPlantedCrops(selectedBlock);
                    }
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