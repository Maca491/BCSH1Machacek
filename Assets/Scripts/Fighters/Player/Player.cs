using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Fighter
{
    public InputSystem_Actions input { get; private set; }
    
    private int isRunningHash, isWalkingHash;

    public Vector2 currentMovementInput { get; private set; }

    public bool movementPressed { get; private set; }
    
    public bool runPressed { get; set; }
    public bool isArmed { get; set; }

    private CinemachineCamera playerCamera;

    private Coroutine resetCameraCoroutine;

    [SerializeField] private float rotationSpeed = 2.0f;

    #region weapon

    [Header("Weapon")]
    [SerializeField] private GameObject weaponPrefabAsset;
    private GameObject currentWeapon;
    [SerializeField] private Transform weaponAtHip, weaponInHand;

    public delegate void AttackStartedHandler();
    public event AttackStartedHandler OnAttackStarted;

    #endregion

    private void Awake()
    {
        #region variables initialization
        input = new InputSystem_Actions();

        playerCamera = FindFirstObjectByType<CinemachineCamera>();

        animator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody>();

        stateMachine = new StateMachine<Fighter>();
        
        Debug.Log("Player weapon at hip: " + weaponAtHip.name);
        Debug.Log("Player weapon in hand: " + weaponInHand.name);

        currentHealth = maxHealth;
        currentStamina = maxStamina;
        damage = 50;

        #endregion

        #region input handling

        input.PlayerControls.Movement.performed += ctx => {
            currentMovementInput = ctx.ReadValue<Vector2>();
            movementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
            if (!stateMachine.IsInState<MoveState>()) {
                stateMachine.ChangeState(new MoveState(this));
            }
        };

        input.PlayerControls.Movement.canceled += ctx => {
            Debug.Log("Movement canceled");
            movementPressed = false;
            runPressed = false;
            currentMovementInput = Vector2.zero;
        };

        input.PlayerControls.Run.performed += ctx => runPressed = true;
        input.PlayerControls.Run.canceled += ctx => runPressed = false;

        input.PlayerControls.Roll.performed += ctx => {
            if (movementPressed && CanAct()) {
                RotateImmediatelyToMovement();
                stateMachine.ChangeState(new RollingState(this));
            }
        };

        input.PlayerControls.Drawweapon.performed += ctx => {
            isArmed = !isArmed;
            animator.SetBool("isArmed", isArmed);
            if(isArmed){
                SpawnWeaponInHand();
            } else{
                SpawnWeaponAtHip();
            }
        };

        input.PlayerControls.Attack.performed += ctx => {
            if (isArmed && CanAct() && !stateMachine.IsInState<AttackState>()){
                stateMachine.ChangeState(new AttackState(this));
            }
        };

        input.PlayerControls.Pausegame.performed += ctx => {
            GameManager.instance?.TogglePause();
        };
        
        #endregion


    }

    private void Start()
    {
        stateMachine.Initialize(new IdleState(this));

        SpawnWeaponAtHip();

        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
    }

    private void Update()
    {
        stateMachine.Update();
        if(!runPressed && !stateMachine.IsInState<AttackState>() && !stateMachine.IsInState<RollingState>()){
            UpdateStamina();
        }
    }


   void FixedUpdate()
    {
        //<---------------------- important ------------------------------>
        //voláno z FixedUpdate, kvůli rigidbody a fyzice
        //pro npc voláno z Update, kvůli agentovi
        if(stateMachine.IsInState<MoveState>()){
            MoveAndRotate();
        }
    }

    public void UpdatePlayerAnimator()
    {
        animator.SetBool(isWalkingHash, movementPressed);
        animator.SetBool(isRunningHash, runPressed);
    }

    public override void StopMovement()
    {
        currentMovementInput = Vector2.zero;
        movementPressed = false;
        runPressed = false;
        rb.linearVelocity = Vector3.zero;
        animator.SetBool(isWalkingHash, false);
        animator.SetBool(isRunningHash, false);
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    public void SetCameraFreeLook(bool isFree)
    {
        var axisController = playerCamera.GetComponent<CinemachineInputAxisController>();
        var orbitalFollow = playerCamera.GetComponent<CinemachineOrbitalFollow>();

        if (axisController != null)
            axisController.enabled = isFree;

        if (orbitalFollow != null)
        {
            if (!isFree)
            {
                if (resetCameraCoroutine != null)
                    StopCoroutine(resetCameraCoroutine);

                resetCameraCoroutine = StartCoroutine(SmoothResetCamera(orbitalFollow));
            }
            else
            {
                if (resetCameraCoroutine != null)
                    StopCoroutine(resetCameraCoroutine);
            }
        }
    }


    private IEnumerator SmoothResetCamera(CinemachineOrbitalFollow orbitalFollow)
    {
        float time = 0f;
        float duration = 1f;

        float startHorizontal = orbitalFollow.HorizontalAxis.Value;
        float targetHorizontal = orbitalFollow.HorizontalAxis.Center;

        float startVertical = orbitalFollow.VerticalAxis.Value;
        float targetVertical = orbitalFollow.VerticalAxis.Center;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            orbitalFollow.HorizontalAxis.Value = Mathf.Lerp(startHorizontal, targetHorizontal, t);
            orbitalFollow.VerticalAxis.Value = Mathf.Lerp(startVertical, targetVertical, t);

            yield return null;
        }

        orbitalFollow.HorizontalAxis.Value = targetHorizontal;
        orbitalFollow.VerticalAxis.Value = targetVertical;
    }
    
    public void SpawnWeaponAtHip()
    {
        StartCoroutine(SpawnWeaponWithFrameDelay(weaponAtHip, 2)); 
    }

    public void SpawnWeaponInHand()
    {
        StartCoroutine(SpawnWeaponWithFrameDelay(weaponInHand, 2));
    }

    private IEnumerator SpawnWeaponWithFrameDelay(Transform target, int frameDelay)
    {
        Destroy(currentWeapon);

        for (int i = 0; i < frameDelay; i++)
            yield return null;

        currentWeapon = Instantiate(weaponPrefabAsset, target.position, target.rotation);
        currentWeapon.transform.SetParent(target);
        currentWeapon.transform.localScale = Vector3.one;
    }

    public void NotifyNPCAttackStarted()
    {
        OnAttackStarted?.Invoke();
    }

    public override void MoveAndRotate()
    {
        Vector3 cameraForward = playerCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = playerCamera.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector3 moveDir = cameraForward * currentMovementInput.y + cameraRight * currentMovementInput.x;
        moveDir.Normalize();

        float speed = CanAct() && runPressed ? runSpeed : walkSpeed;

        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveDir.x * speed;
        velocity.z = moveDir.z * speed;
        rb.linearVelocity = velocity;

        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
        }
    }

    private void RotateImmediatelyToMovement()
    {
        Vector3 cameraForward = playerCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 cameraRight = playerCamera.transform.right;
        cameraRight.y = 0;
        cameraRight.Normalize();

        Vector3 targetDirection = cameraForward * currentMovementInput.y + cameraRight * currentMovementInput.x;

        if (targetDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = targetRotation; 
        }
    }

}