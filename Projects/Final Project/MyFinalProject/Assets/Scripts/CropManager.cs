using UnityEngine;

[RequireComponent(typeof(Grid))]
public class CropManager : MonoBehaviour
{

    private Grid cropGrid;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cropGrid = GetComponent<Grid>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
