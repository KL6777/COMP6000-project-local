using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject inventoryUI; // Reference to the Inventory UI GameObject
    public Inventory inventory;
    public playerMovement playerMoverment; 
    public GameObject talkButton;
    public GameObject giftButton;
    private bool isActive = false;
    
    void Update()
    {
        isActive = inventoryUI.activeSelf;

        // Close the menu when "escape" is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isActive)
            {
                ToggleInventory();
            }
        }

        // Toggle inventory when "i" is pressed
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }

        // Hide buttons when the inventory is open
        if (isActive)
        {
            talkButton.SetActive(false);
            giftButton.SetActive(false);
        }
        else if (!isActive)
        {
            talkButton.SetActive(true);
            giftButton.SetActive(true);
        }
    }

    // Open/ close the inventory
    public void ToggleInventory()
    {
        isActive = inventoryUI.activeSelf;
        inventoryUI.SetActive(!isActive);
        playerMoverment.enabled = isActive; // Disable player movement while inventory is active

        if (!isActive)
        {
            inventory.ShowItems();
        }
    }
}
