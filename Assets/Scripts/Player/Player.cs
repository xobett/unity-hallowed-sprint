using UnityEngine;

[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour, IDamageable
{
    private Health health;

    void Start()
    {
        health = GetComponent<Health>();
    }
    public void TakeDamage(float damage)
    {
        health.Damage(damage);
    }
}