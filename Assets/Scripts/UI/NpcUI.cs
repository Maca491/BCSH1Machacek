using UnityEngine;
using UnityEngine.UI;

public class NpcUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider staminaSlider;

    [SerializeField] private Fighter fighter;


    private void Update()
    {
        if (fighter == null) return;

        healthSlider.value = fighter.currentHealth/fighter.maxHealth;
        staminaSlider.value = fighter.currentStamina/fighter.maxStamina;
    }
}
