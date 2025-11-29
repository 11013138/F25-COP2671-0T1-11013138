using UnityEngine;

public class Harvestable : MonoBehaviour
{
    // variables
    public string itemName;
    public int value = 1;

    public ItemData itemData;

    public GameObject collectEffect;

    private Inventory inventory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // find inventorry
        if (inventory == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                inventory = player.GetComponent<Inventory>();
            }

            // if not on player, find in scene
            if (inventory == null)
            {
                inventory = Object.FindFirstObjectByType<Inventory>();
            }
        }

        // destroy objects if not collected
        Destroy(gameObject, 30f);
    }

   public void Collect()
    {
        // spawn collect effect
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        // add to inventory
        if (inventory != null)
        {
            if (itemData != null)
            {
                // use itemdata
                inventory.AddItem(itemData, 1);
            }
        }
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // collect on player touch
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }
}
