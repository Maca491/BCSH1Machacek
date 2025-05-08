using UnityEngine;

public class MoveState : State<Fighter>
{
    public MoveState(Fighter owner) : base(owner){}

    public override void Enter()
    {
        Debug.Log("Entering Move State");
        owner.animator.SetBool("isWalking", true);
    }

    public override void Update()
    {
        if(owner is Player player){
            Vector2 input = player.currentMovementInput;

            if (input == Vector2.zero)
            {
                owner.stateMachine.ChangeState(new IdleState(owner));
                return;
            }
            if(player.runPressed && player.animator.GetBool("isRunning") == false && player.CanAct()){
                Debug.Log("Running");
                owner.ConsumeStamina(5f * Time.deltaTime);
                owner.animator.SetBool("isRunning", true);
            }
        } else if(owner is Knight knight){
            knight.Walk();
            //pro hráče se bude volat z FixedUpdate, kvůli rigidbody
            knight.MoveAndRotate();
        }
    }


    public override void Exit()
    {
        owner.StopMovement(); // Zastaví Rigidbody, animace atd.
    }
}
