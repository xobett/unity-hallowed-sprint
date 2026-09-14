using UnityEngine;

[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour, IDamageable, IRespawnable
{
    private Health health;
    private Vector3 LastCheckpoint;

    void Start()
    {
        LastCheckpoint = transform.position;
        health = GetComponent<Health>();
    }
    
    public void TakeDamage(float damage)
    {
        health.Damage(damage);
    }

    public void Respawn() {
        transform.position = LastCheckpoint;
    }
}