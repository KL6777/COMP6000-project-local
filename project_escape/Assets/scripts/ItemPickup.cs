using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory inventory = other.GetComponent<Inventory>();
            inventory.AddItem(item);
            //gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
}

