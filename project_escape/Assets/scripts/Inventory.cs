using System.Collections.Generic;
using UnityEngine;
using TMPro; // For TextMeshPro components
using PluginClassLibrary; //plugin 
using UnityEngine.UI;
public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    public List<Item> items = new List<Item>();
    public GameObject itemPrefab;
    public Transform itemPosition;
    GameObject obj;

    //Start instance on scene load
    private void Awake()
    {
        Instance = this;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Item")
        {
            Debug.Log("Pickup");
        }
    }

    // Adds an item to the inventory
    public void AddItem(Item item)
    {
        items.Add(item);
    }

    // Removes an item
    public void RemoveItem(Item item)
    {
        items.Remove(item); 
    }

    // Gives the selected item to a nearby NPC
    public void GiveItem(Item item)
    {
        // if npc nearby (link with npc proximity)
        Debug.Log("give item:");
        RemoveItem(item); 
        // change NPC emotions
    }

    // Add items to UI
    public void ShowItems()
    {
        // Empties the UI to avoid duplicates
        foreach(Transform item in itemPosition)
        {
            Destroy(item.gameObject);
        }

        // Adds each item to UI
        foreach (var item in items)
        {
            obj = Instantiate(itemPrefab, itemPosition);

            // Add OnClick event to the button
            Button button = obj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnItemClick(item));
            }
        }
    }

    // Print out inventory in debug.log 
    public void ListItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Debug.Log(items[i].name);
        }
    }

    private void OnItemClick(Item item)
    {
        Debug.Log("Item clicked: " + item.name);
        DialogueManagerP dialogueManager = FindObjectOfType<DialogueManagerP>();
        if (dialogueManager != null)
        {
            dialogueManager.GiveItem(item);
        }
    }


}
