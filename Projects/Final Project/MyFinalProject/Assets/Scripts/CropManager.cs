using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[RequireComponent(typeof(Grid))]
public class CropManager : MonoBehaviour
{
    [SerializeField] private Tilemap farmingTilemap;
    [SerializeField] private float growthUpdateInterval = 1f;
    // variables
    private Grid cropGrid;
    private CropBlock[,] cropGridData;
    private List<CropBlock> plantedCrops = new List<CropBlock>();
    private Vector2Int gridSize;
    private Vector3 gridOrigin;
    private float growthTimer = 0f;

    public static CropManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cropGrid = GetComponent<Grid>();

        if (farmingTilemap == null)
        {
            return;
        }
        CreateGridUsingTilemap(farmingTilemap);
    }

    // Update is called once per frame
    void Update()
    {
        growthTimer += Time.deltaTime;
        if (growthTimer >= growthUpdateInterval)
        {
            UpdateCropGrowth();
            growthTimer = 0f;
        }
    }

    public void CreateGridUsingTilemap(Tilemap tilemap)
    {
        farmingTilemap = tilemap;

        // get bounds of tilemap
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;

        gridSize = new Vector2Int(bounds.size.x, bounds.size.y);
        gridOrigin = tilemap.CellToWorld(bounds.min);

        // initialize 2d array
        cropGridData = new CropBlock[gridSize.x, gridSize.y];

        // create CropBlock for each tile position
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int gridPos = new Vector2Int(x, y);
                Vector3Int cellPos = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                Vector3 worldPos = tilemap.CellToWorld(cellPos);

                CropBlock cropBlock = new CropBlock(gridPos, cellPos, worldPos, farmingTilemap);
                cropGridData[x, y] = cropBlock;
            }
        }
    }

    public void CreateGridBlock(Tilemap tilemap, Vector2Int location, Vector3 position, CropBlock gridBlock)
    {
        if (cropGridData == null || location.x < 0 || location.x >= gridSize.x || location.y < 0 || location.y >= gridSize.y)
        {
            return;
        }

        cropGridData[location.x, location.y] = gridBlock;
    }

    public CropBlock GetCropBlockPosition(Vector3 worldPosition)
    {
        if (farmingTilemap == null || cropGridData == null)
        {
            return null;
        }

        Vector3Int cellPosition = farmingTilemap.WorldToCell(worldPosition);
        BoundsInt bounds = farmingTilemap.cellBounds;

        int x = cellPosition.x - bounds.xMin;
        int y = cellPosition.y - bounds.yMin;

        if (x >= 0 && x < gridSize.x && y >= 0 && y < gridSize.y)
        {
            return cropGridData[x, y];
        }

        return null;
    }

    public CropBlock GetCropBlockGridPos(Vector2Int gridPosition)
    {
        if (cropGridData == null)
        {
            return null;
        }

        if (gridPosition.x >= 0 && gridPosition.x < gridSize.x && gridPosition.y >= 0 && gridPosition.y < gridSize.y)
        {
            return cropGridData[gridPosition.x, gridPosition.y];
        }
        return null;
    }

    public void AddToPlantedCrops(CropBlock cropBlock)
    {
        if (cropBlock != null && !plantedCrops.Contains(cropBlock))
        {
            plantedCrops.Add(cropBlock);
        }
    }

    public void RemoveFromPlantedCrops(CropBlock cropBlock)
    {
        if (cropBlock != null && plantedCrops.Contains(cropBlock))
        {
            plantedCrops.Remove(cropBlock);
        }
    }

    private void UpdateCropGrowth()
    {
        TimeManager timeManager = FindFirstObjectByType<TimeManager>();

        for (int i = plantedCrops.Count - 1; i >= 0; i--)
        {
            if (plantedCrops[i] != null)
            {
                plantedCrops[i].UpdateGrowth(growthUpdateInterval, timeManager);

                // remove if harvested
                if (plantedCrops[i].currentState == CropState.Empty)
                {
                    RemoveFromPlantedCrops(plantedCrops[i]);
                }
            }
        }
    }
}
