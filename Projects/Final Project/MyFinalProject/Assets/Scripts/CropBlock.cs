using UnityEngine;
using UnityEngine.Tilemaps;

public enum CropState
{
    Empty,
    Tilled,
    Planted,
    Growing,
    Ready
}

public class CropBlock
{
    // variables
    public Vector2Int gridPosition;
    public Vector3Int cellPosition;
    public Vector3 worldPosition;
    public CropState currentState = CropState.Empty;
    public SeedPacket seedPacket;

    private int currentGrowthStage = 0;
    private float growthTimer = 0f;
    private bool isWatered = false;
    private Tilemap cropTilemap;

    public CropBlock(Vector2Int gridPos, Vector3Int cellPos, Vector3 worldPos, Tilemap tilemap)
    {
        gridPosition = gridPos;
        cellPosition = cellPos;
        worldPosition = worldPos;
        cropTilemap = tilemap;
        currentState = CropState.Empty;
    }

    public void TillSoil()
    {
        if (currentState == CropState.Empty)
        {
            currentState = CropState.Tilled;
            // TODO: set a tilled-soil tile if you have one
        }
    }

    public void WaterSoil()
    {
        if (currentState == CropState.Tilled || currentState == CropState.Planted)
        {
            isWatered = true;
        }
    }

    public bool PlantSeed(SeedPacket seed)
    {
        if (currentState == CropState.Tilled && seed != null)
        {
            seedPacket = seed;
            currentState = CropState.Planted;
            currentGrowthStage = 0;
            growthTimer = 0f;
            isWatered = false;

            UpdateTileSprite();
            return true;
        }
        return false;
    }

    public bool HarvestPlants()
    {
        if (currentState == CropState.Ready && seedPacket != null)
        {
            if (seedPacket.harvestablePrefab != null)
            {
                Object.Instantiate(seedPacket.harvestablePrefab, worldPosition, Quaternion.identity);
            }

            currentState = CropState.Empty;
            seedPacket = null;
            currentGrowthStage = 0;
            growthTimer = 0f;
            isWatered = false;

            cropTilemap.SetTile(cellPosition, null);
            return true;
        }
        return false;
    }

    public void UpdateGrowth(float deltaTime, TimeManager timeManager)
    {
        if (currentState == CropState.Planted || currentState == CropState.Growing)
        {
            if (seedPacket == null) return;
            if (!CanGrow(timeManager)) return;

            if (isWatered)
            {
                growthTimer += deltaTime;

                float stageTime = seedPacket.growthTimePerStage;
                int newStage = Mathf.FloorToInt(growthTimer / stageTime);

                if (newStage > currentGrowthStage && newStage < seedPacket.growthSprites.Length)
                {
                    currentGrowthStage = newStage;
                    currentState = (newStage >= seedPacket.growthSprites.Length - 1)
                        ? CropState.Ready
                        : CropState.Growing;
                    UpdateTileSprite();
                }

                if (growthTimer >= seedPacket.totalGrowthTime)
                {
                    currentState = CropState.Ready;
                    currentGrowthStage = seedPacket.growthSprites.Length - 1;
                }
            }
        }
    }

    private void UpdateTileSprite()
    {
        if (seedPacket == null || currentGrowthStage >= seedPacket.growthSprites.Length)
            return;

        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = seedPacket.growthSprites[currentGrowthStage];
        cropTilemap.SetTile(cellPosition, tile);
    }

    public bool CanGrow(TimeManager timeManager)
    {
        return timeManager != null &&
               timeManager.currentHour >= 6f &&
               timeManager.currentHour <= 18f;
    }

    public bool CanPlant() => currentState == CropState.Tilled;
    public bool CanHarvest() => currentState == CropState.Ready;
    public bool CanTill() => currentState == CropState.Empty;
    public bool CanWater() => currentState == CropState.Tilled ||
                                currentState == CropState.Planted ||
                                currentState == CropState.Growing;
}