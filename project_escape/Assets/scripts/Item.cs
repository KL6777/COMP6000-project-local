using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Items/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public int amount;
    
    //emotional values
    public int sad_happy;
    public int disgust_trust;
    public int anger_fear;
    public int anticipation_surprise;

    public Item(string name, int itemAmount, int happy, int trust, int fear, int surprise)
    {
        itemName = name;
        amount = itemAmount;
        sad_happy = happy;
        disgust_trust = trust;
        anger_fear = fear;
        anticipation_surprise = surprise;
    }

    public string GetName()
    {
        return (itemName);
    }
}
