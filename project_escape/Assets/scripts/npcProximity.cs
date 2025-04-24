using UnityEngine;
using UnityEngine.UI;

public class NPCProximity : MonoBehaviour
{
    // Reference to the interaction panel that appears when the player is nearby
    public GameObject interactionPanel; 

    // Tracks if the player is currently within range
    private bool isPlayerNear = false;

    // Reference to the NPCs CircleCollider2D for detecting proximity
    private CircleCollider2D npcCollider;

    void Start()
    {
        // Get the NPCs CircleCollider2D component
        npcCollider = GetComponent<CircleCollider2D>();

        // Error handling ensure the NPC has a collider attached
        if (npcCollider == null)
        {
            Debug.LogError("No Collider found on NPC. Please add one.");
            return;
        }

        // Dynamically assign the interaction panel in case it’s not manually assigned
        if (interactionPanel == null)
        {
            interactionPanel = GameObject.FindWithTag("InteractionPanel");

            // Error handling alert the developer if the panel is missing
            if (interactionPanel == null)
            {
                Debug.LogError("No Interaction Panel found in the scene. Please assign one.");
                return;
            }
        }

        // Check if the player is already within range when the scene starts
        Collider2D playerCollider = Physics2D.OverlapCircle(
            transform.position, 
            npcCollider.radius, 
            LayerMask.GetMask("Player")  // Only detect objects on the Player layer
        );

        // Show or hide the interaction panel based on the player's position
        if (playerCollider != null)
        {
            ShowPanel();  // Player is already near at start
        }
        else
        {
            HidePanel();  // Player is not near at start
        }
    }

    // Triggered when another collider enters the NPCs detection area
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object entering is tagged as Player
        if (other.CompareTag("Player"))
        {
            ShowPanel();  // Display the interaction panel
        }
    }

    // Triggered when another collider exits the NPCs detection area
    void OnTriggerExit2D(Collider2D other)
    {
        // Check if the object exiting is tagged as Player
        if (other.CompareTag("Player"))
        {
            HidePanel();  // Hide the interaction panel
        }
    }

    // Displays the interaction panel and updates the player's proximity status
    void ShowPanel()
    {
        if (interactionPanel != null)
            interactionPanel.SetActive(true);

        isPlayerNear = true;  // Mark the player as near
    }

    // Hides the interaction panel and updates the player's proximity status
    void HidePanel()
    {
        if (interactionPanel != null)
            interactionPanel.SetActive(false);

        isPlayerNear = false;  // Mark the player as not near
    }
}
