using UnityEngine;

public class Coin : MonoBehaviour, IInteractable
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO: handle multiple collisions with multiple colliders on player
        var player = collision.GetComponentInParent<Character2DController>();
        if (player == null) return;
        Debug.Log("Player entered!");
    }

    public void OnInteract()
    {
        // add coin to the player's inventory
    }
}