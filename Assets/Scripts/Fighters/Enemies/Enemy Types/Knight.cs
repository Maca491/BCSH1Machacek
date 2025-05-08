using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Knight : Fighter
{
    private Transform playerTarget;

    private NavMeshAgent agent;

    [SerializeField] private float attackDistance = 2.0f;

    //[HideInInspector]
    public float reactionChance { get; set;}

    private void Awake()
    {
        animator = GetComponentInParent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        stateMachine = new StateMachine<Fighter>();
        animator.SetBool("isArmed", true);
        playerTarget = FindFirstObjectByType<Player>().transform;
        //najdi hráče podle tagu
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }
    
    private void Start()
    {
        stateMachine.Initialize(new IdleState(this));
        Player player = playerTarget.GetComponent<Player>();
        if (player != null)
        {
            player.OnAttackStarted += ReactToAttack;
        }
    }

    private bool hasReactedToAttack = false;

    private void Update()
    {
        if(hasReactedToAttack) return;
        if (playerTarget == null) return;

        float distance = Vector3.Distance(transform.position, playerTarget.position);

        // Základní AI chování:
        if (distance > attackDistance)
        {
            if(!stateMachine.IsInState<MoveState>())
            {
                stateMachine.ChangeState(new MoveState(this));
            }
        } 
        if (distance < attackDistance && CanAct() && !stateMachine.IsInState<AttackState>())
        {
            // Zastavení agenta a přechod do stavu útoku
            agent.isStopped = true;
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            Quaternion targetRot = Quaternion.LookRotation(playerTarget.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
            stateMachine.ChangeState(new AttackState(this));
            return;
        }

        stateMachine.Update();
    }

    public override void MoveAndRotate()
    {
        if (playerTarget == null) return;

        agent.isStopped = false;
        agent.SetDestination(playerTarget.position);

        // otočení za běhu
        Vector3 direction = agent.desiredVelocity;
        if (direction.sqrMagnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
        }
    }

    public void Run(){
        agent.speed = runSpeed;
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", true);
    }

    public void Walk(){
        UpdateStamina();
        agent.speed = walkSpeed;
        animator.SetBool("isWalking", true);
        animator.SetBool("isRunning", false);
    }

    public override void StopMovement()
    {
        agent.isStopped = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("isRunning", false);
    }

    private void ReactToAttack()
    {
        if (!hasReactedToAttack && CanAct() && !stateMachine.IsInState<RollingState>() && !stateMachine.IsInState<AttackState>())
        {
            float roll = Random.Range(0f, 1f);
            if (roll <= reactionChance){
            hasReactedToAttack = true;
            stateMachine.ChangeState(new RollingState(this));
            }
        }
    }

    public void ResetReaction()
    {
        hasReactedToAttack = false;
    }
}
