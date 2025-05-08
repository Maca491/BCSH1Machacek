using UnityEngine;

public class IdleState : State<Fighter>
{
    public IdleState(Fighter owner) : base(owner) { }

    public override void Enter()
    {
        Debug.Log("Entering Idle State");
        owner.animator.SetBool("isWalking", false);
        owner.animator.SetBool("isRunning", false);
        if(owner is Player player){
            player.SetCameraFreeLook(true);
        }
    }
}
