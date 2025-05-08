using System;
using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEngine;

public abstract class Fighter : MonoBehaviour, IDamagable
{
    [Header("HP & Stamina")]
    [field: SerializeField] public float maxHealth { get; set; } = 100f;
    [field: SerializeField] public float maxStamina { get; set; } = 100f;
    [field: SerializeField] public float staminaRegenRate { get; set; } = 10f;
    [field: SerializeField] public int damage { get; set; } = 10;

    public bool CanAct() => currentStamina >= 0;

    public float currentStamina;

    [field: SerializeField] public float walkSpeed { get; set; } = 2.0f;
    [field: SerializeField] public float runSpeed { get; set; } = 4.0f;
    
    public float currentHealth { get; set; }

    public Animator animator { get; set; }
    
    public Rigidbody rb { get; set; }

    [SerializeField] private GameObject hurtBox; // hurtbox - když nás/nebo npc zasáhne útok

    [SerializeField] private GameObject hitBox; // hitbox - čím zasahujeme my/nebo npc

    public StateMachine<Fighter> stateMachine { get; set; }

    public abstract void MoveAndRotate();

    public abstract void StopMovement();

    public void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void UpdateStamina()
    {
        // Regenerujeme, dokud nejsme zpět v „bezpečí“
        currentStamina = Mathf.Min(currentStamina + staminaRegenRate * Time.deltaTime, maxStamina);
    }

    public void ConsumeStamina(float amount)
    {
        currentStamina -= amount;
    }

    #region Called by Animation Events

    public void OnAttackAnimationComplete()
    {
        stateMachine.ReturnToPreviousState();
    }

    //zapínání vypínání i-framů
    public void DisableHurtBox() => hurtBox.SetActive(false);
    public void EnableHurtBox() => hurtBox.SetActive(true);

    //zapínání/vypínání hitboxu
    public void DisableHitBox() => hitBox.SetActive(false);
    public void EnableHitBox(){
        hitBox.SetActive(true); 
    Debug.Log("Hitbox enabled");
    }

    public void LeaveRollState()
    {
        if(this is Player player){
            player.input.PlayerControls.Roll.Enable();
            player.input.PlayerControls.Movement.Enable();
            player.input.PlayerControls.Run.Enable();
            player.input.PlayerControls.Attack.Enable();
            player.input.PlayerControls.Drawweapon.Enable();
        }
        stateMachine.ChangeState(new IdleState(this));
    }

    #endregion

    public event Action OnDeath;
}
