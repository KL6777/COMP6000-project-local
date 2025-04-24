using UnityEngine;
using TMPro; // For TextMeshPro components
using PluginClassLibrary; //plugin 
public class DialogueManagerP : MonoBehaviour
{
    public GameObject dialogueUI; // Reference to the Dialogue UI GameObject
    public TMP_Text dialogueText; // Reference to the NPC dialogue text (TextMeshPro)
    public TMP_InputField playerInputField; // Reference to the Player Input Field (TextMeshPro)

    public GameObject inventoryUI; // Reference to the Inventory UI GameObject
    public Inventory inventory;

    private bool isNearNPC = false; // Tracks if the player is near an NPC
    private string currentNPCDialogue = ""; // Stores the dialogue of the nearby NPC
    public playerMovement playerMoverment; // Reference to the PlayerMovement script
    public InventoryManager inventoryManager; // Reference to the InventoryManager script

    private GameObject player; //reference to player obeject

    private npc Current_npc; // Reference to the current NPC the player is near by

    private bool GeneratingText = false; //bug fix to stop player from spamming enter breaking the text generation

    //public NPC kevin = new NPC("Kevin");

    void start()
    {
        player = GameObject.Find("Player");
    }
    void Update()
    {
        // Close dialogue when "Escape" is pressed
        if (dialogueUI.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            EndDialogue();
        }

        // Generate NPC text when "Enter" is pressed
        if(dialogueUI.activeSelf && Input.GetKeyDown(KeyCode.Return) && !GeneratingText)
        {
            GenerateNPCText();
        }
    }


    //when button is pressed when near npc open up the inventory to give a gift
    // Open/ close the inventory on button press
    public void OpenInventory()
    {
        bool isActive = inventoryUI.activeSelf;
        inventoryUI.SetActive(!isActive);
        playerMoverment.enabled = isActive; // Disable player movement while inventory is active

        if (isNearNPC && !isActive && !dialogueUI.activeSelf)
        {
            inventory.ShowItems();
        }
    }


    //same as inventory but for dialogue
    // Starts dialogue when the button is pressed
    public void OpenDia(){
        currentNPCDialogue = Current_npc.GetCurrentText();
        if (isNearNPC  && !dialogueUI.activeSelf && !inventoryUI.activeSelf)
        {
            StartDialogue(currentNPCDialogue);
        }
    }

    // Generate NPC text based on player input
    //async function to allow for the text generation to be done in the background
    public async void GenerateNPCText()
    {
        string playerInput = playerInputField.text; // Get the player's input        
        string reaction = "";
        GeneratingText = true;
        dialogueText.text = "Generating response...";
        if (Current_npc != null)
        {
            reaction = await Current_npc.GenerateReaction(playerInput);
        }
        dialogueText.text = reaction;
        GeneratingText = false;
        Current_npc.SetCurrentText(reaction);
    }


    // Give an clicked item to the NPC
    public async void GiveItem(Item item)
    {
        string playerInput = "User gives you" + item.itemName; // Get the player's input
        string reaction = "";

        if (Current_npc != null)
        {
            reaction = await Current_npc.ReciveItem(playerInput, item);
        }
        inventoryUI.SetActive(false);
        StartDialogue(reaction);
    }

    //toggles ui for talking to npc
    public void StartDialogue(string npcDialogue)
    {
        dialogueUI.SetActive(true);
        dialogueText.text = npcDialogue; // Display NPC dialogue
        playerInputField.text = ""; // Clear input field
        playerInputField.ActivateInputField(); // Focus the input field
        playerMoverment.enabled = false; // Disable player movement while in dialogue
        inventoryManager.enabled = false; // Disable inventory management while in dialogue
    }

    //closes ui for talking to npc
    public void EndDialogue()
    {
        dialogueUI.SetActive(false); // Hide the dialogue UI
        playerMoverment.enabled = true; // Enable player movement
        inventoryManager.enabled = true; // Enable inventory management
    }


    // Check if the player is near an NPC and set the current NPC to it
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            isNearNPC = true;

            Current_npc = other.GetComponent<npc>();
        }
    }

    //clears the current npc when the player leaves the proximity of the npc
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("NPC"))
        {
            isNearNPC = false;
            currentNPCDialogue = ""; // Clear dialogue when leaving the NPC area
            
        }
    }
}
