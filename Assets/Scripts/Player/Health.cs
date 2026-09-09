using System;
using UnityEngine;

class Health
{
    [SerializeField] private float health;
    private const float maxHealth = 100f;

    public void Heal()
    {
        health = maxHealth;
    }

    public void Damage(float amount)
    {
        health -= amount;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // player dies
    }
}