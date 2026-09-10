using UnityEngine;

class Health : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    private const float maxHealth = 100f;

    void Start()
    {
        health = maxHealth;
    }

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