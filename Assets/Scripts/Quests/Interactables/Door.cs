using UnityEngine;

public class Door : Interactable
{
    public override void Interact(GameObject instigator)
    {
        if (instigator.TryGetComponent(out Inventory playerInventory) && playerInventory.HasItemOfType(Item.ItemType.Key).Item1)
        {
            playerInventory.RemoveItem(playerInventory.HasItemOfType(Item.ItemType.Key).Item2);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Interact(other.gameObject);
        }
    }
}
