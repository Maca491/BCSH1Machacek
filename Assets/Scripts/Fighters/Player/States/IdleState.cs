using UnityEngine;

public class IdleState : State<Fighter>
{
    public IdleState(Fighter owner) : base(owner) { }

    public override void Enter()
    {
        owner.StopMovement();

        if(owner is Player player){
            player.SetCameraFreeLook(true);
        }
    }
}
