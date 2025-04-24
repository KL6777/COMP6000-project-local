using UnityEngine;
using TMPro; // Required for TextMeshPro components

public class DialogueManager : MonoBehaviour
{
    public GameObject dialogueUI; // Reference to the Dialogue UI GameObject
    public TMP_Text dialogueText; // Reference to the NPC dialogue Text (TextMeshPro)
    public TMP_InputField playerInputField; // Reference to the Player Input Field (TextMeshPro)
    public playerMovement playerMoverment; 
    private bool isNearNPC = false; // Tracks if the player is near an NPC
    private bool inConversation = false;

    void Update()
    {
        // Close dialogue when "Escape" is pressed
        if (dialogueUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            EndDialogue();
            playerMoverment.enabled = true;
        }
    }
    
    // Starts a conversation when the button is pressed
    public void OpenDialogue()
    {
        StartDialogue("Hello there! How can I help you?");
        playerMoverment.enabled = false;
    }

    public void StartDialogue(string npcDialogue)
    {
        dialogueUI.SetActive(true);
        dialogueText.text = npcDialogue; // Set the NPC dialogue text
        playerInputField.text = ""; // Clear input field
        playerInputField.ActivateInputField(); // Focus the input field
    }

    public void EndDialogue()
    {
        dialogueUI.SetActive(false); // Hide the dialogue UI
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            isNearNPC = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            isNearNPC = false;
        }
    }
}
