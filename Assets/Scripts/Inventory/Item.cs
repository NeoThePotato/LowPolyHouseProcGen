using UnityEngine;

public class Item
{
    public ItemType itemType;

    public Item(ItemType type)
    {
        itemType = type;
    }

    public enum ItemType
    {
        None,
        Key
    }
}
