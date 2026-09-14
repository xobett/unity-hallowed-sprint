using UnityEngine;

public class DeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var respawnable = other.GetComponentInParent<IRespawnable>();

        if (respawnable == null) return;
        respawnable.Respawn();
    }
}
