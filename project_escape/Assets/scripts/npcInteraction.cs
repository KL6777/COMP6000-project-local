using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    // Called when the player gives a gift to the NPC
    public void GiveGift()
    {
        // Logs a message to confirm the player has given a gift
        Debug.Log("Player gave a gift to the NPC.");
        

    }

    // Called when the player interacts with the NPC to start a conversation
    public void TalkToNPC()
    {
        // Logs a message to confirm the player has initiated dialogue
        Debug.Log("Player started talking to the NPC.");
        
        
    }
}
