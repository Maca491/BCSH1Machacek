using UnityEngine;
using System.Collections.Generic;

public class HitBox : MonoBehaviour
{
    private HashSet<IDamagable> alreadyHit = new();

    private void OnEnable()
    {
        alreadyHit.Clear(); // důležité! nový útok = čistý seznam
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hitbox triggered with: " + other.name);

        if (other.TryGetComponent<IDamagable>(out var target))
        {
            if (!alreadyHit.Contains(target))
            {
                Fighter owner = GetComponentInParent<Fighter>();
                int damage = owner.damage;

                target.TakeDamage(damage);
                alreadyHit.Add(target);
                Debug.Log($"Hit: {other.name} for {damage} dmg");
                Debug.Log($"Max health: {target.maxHealth}");
                Debug.Log($"Current health: {target.currentHealth}");
            }
        }
    }

}
