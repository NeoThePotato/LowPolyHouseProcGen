using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    List<Item> items = new List<Item>();

    public void AddItem(Item newItem)
    {
        if (newItem != null && items.Contains(newItem))
        {
            Debug.Log("Item already in inventory.");
            return;
        }
        items.Add(newItem);
    }

    public void RemoveItem(Item itemToRemove)
    {
        if (items.Count == 0) return;

        if (itemToRemove != null && items.Contains(itemToRemove))
        {
            items.Remove(itemToRemove);
        }
    }

    public (bool, Item) HasItemOfType(Item.ItemType itemTypeToCheck)
    {
        if (items.Count == 0) return (false, null);

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemType == itemTypeToCheck)
            {
                return (true, items[i]);
            }
        }
        return (false, null);
    }
}
