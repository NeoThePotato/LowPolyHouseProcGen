using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public Item.ItemType itemType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory playerInventory = other.GetComponent<Inventory>();
            if (playerInventory != null)
            {
                Item item = new Item(itemType); // Example item type
                playerInventory.AddItem(item);
                Destroy(gameObject); // Remove the item from the scene after picking it up
            }
        }
    }
}
