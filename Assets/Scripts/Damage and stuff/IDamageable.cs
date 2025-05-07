using UnityEngine;

public interface IDamagable
{
    float maxHealth { get; set;}

    float currentHealth { get; set;}
    
    void TakeDamage(int damage);

    void Die();
}
