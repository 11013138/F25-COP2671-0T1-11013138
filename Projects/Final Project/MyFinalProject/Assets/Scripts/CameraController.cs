using UnityEngine;

public class CameraController : MonoBehaviour
{
    // variables
    public Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        if (player != null)
        {
            // follow player with z position at -10
            Vector3 newPosition = player.position;
            newPosition.z = -10;
            transform.position = newPosition;
        }
    }
}
