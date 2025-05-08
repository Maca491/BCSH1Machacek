using UnityEngine;
public class RollingState : State<Fighter>
{
    public RollingState(Fighter owner) : base(owner) {}

    public override void Enter()
    {
        owner.animator.applyRootMotion = true;
        if(owner is Knight knight) Debug.Log("Roll started at:" + owner.currentStamina);

        float cost = 30f;
        owner.ConsumeStamina(cost);

        if(owner is Player player){
            player.input.PlayerControls.Roll.Disable();
            player.input.PlayerControls.Movement.Disable();
            player.input.PlayerControls.Run.Disable();
            player.input.PlayerControls.Attack.Disable();
            player.input.PlayerControls.Drawweapon.Disable();
        }
        owner.animator.SetTrigger("startRoll");
    }

    public override void Exit()
    {
        owner.animator.applyRootMotion = false;
        owner.animator.ResetTrigger("startRoll");
        
        if(owner is Knight knight) {
            Debug.Log("Remaining stamina:" + owner.currentStamina);
            knight.ResetReaction();    
        }
    }
}