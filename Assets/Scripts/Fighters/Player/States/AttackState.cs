using UnityEngine;
public class AttackState : State<Fighter>
{
    public AttackState(Fighter owner) : base(owner) {}

    public override void Enter()
    {
        if(owner is Knight knight) Debug.Log("Attack started at:" + owner.currentStamina);
        float cost = 30f;
        if (owner.currentStamina < 0)
        {
            Debug.LogWarning("Stamina too low for attack. Cancelling.");
            owner.stateMachine.ReturnToPreviousState();
            return;
        }

        owner.ConsumeStamina(cost);
        Debug.Log("Stamina now:" + owner.currentStamina);

        owner.animator.applyRootMotion = true;
        owner.animator.SetTrigger("attack");

        if(owner is Player player){
            player.NotifyNPCAttackStarted(); // <-- zde
            player.input.PlayerControls.Attack.Disable();
            player.input.PlayerControls.Roll.Disable();
        }
    }

    public override void Exit()
    {
        owner.animator.applyRootMotion = false;
        if(owner is Knight knight){
            knight.StopMovement();
        }

        if(owner is Player player){
            player.input.PlayerControls.Attack.Enable();
            player.input.PlayerControls.Roll.Enable();
        }
    }
}