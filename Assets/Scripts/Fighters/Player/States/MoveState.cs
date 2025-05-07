using UnityEngine;

public class MoveState : State<Fighter>
{
    public MoveState(Fighter owner) : base(owner){}

    public override void Update()
    {
        if(owner is Player player){
            Vector2 input = player.GetMovementInput();

            if (input == Vector2.zero)
            {
                owner.stateMachine.ChangeState(new IdleState(owner));
                return;
            }

            if(player.isRunning()){
                owner.ConsumeStamina(5f * Time.deltaTime);
                owner.animator.SetBool("isWalking", true);
                owner.animator.SetBool("isRunning", true);
            }else {
                owner.animator.SetBool("isWalking", true);
                owner.animator.SetBool("isRunning", false);
            }
        } else if(owner is Knight knight){
            /*if (knight.CanAct()){
                owner.ConsumeStamina(5f * Time.deltaTime);
                knight.Run();
            } else {
                knight.Walk();
            }*/
            knight.Walk();
        }
        owner.MoveAndRotate();  
    }


    public override void Exit()
    {
        owner.StopMovement(); // Zastaví Rigidbody, animace atd.
    }
}
