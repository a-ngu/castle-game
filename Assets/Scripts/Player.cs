using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    // Horizontal Movement
    [SerializeField] private float movementSpeed = 6f;
    // Vertical Movement
    [SerializeField] private float maxHeight = 4f;
    [SerializeField] private float timeToMaxHeight = 0.4f;
    [Range(0, 1)] [SerializeField] private float endJumpDamping = 0.5f;
    private float gravity;
    private float initialJumpVelocity;
    // Movement Smoothing
    [SerializeField] private float smoothTimeAirborne = 0.2f;
    [SerializeField] private float smoothTimeGrounded = 0.1f;
    private float currentSmoothVelocity;

    [Header("Input Sensitivity")]
    [SerializeField] private float persistanceTime = 0.2f;

    // State tracking
    private Vector2 input;
    private float jumpingTimer, groundedTimer;
    private bool jumping, canDoubleJump, endJump;
    private Vector2 previousVelocity, velocity;

    private CharacterController2D controller;

    private void Start()
    {
        controller = GetComponent<CharacterController2D>();
        ComputePhysicalSettings();
    }

    private void ComputePhysicalSettings()
    {
        gravity = -2 * maxHeight / Mathf.Pow(timeToMaxHeight, 2);
        initialJumpVelocity = 2 * maxHeight / timeToMaxHeight;
    }
    private void Update()
    {
        ProcessInput();
        CheckState();

        if (jumpingTimer > 0 && groundedTimer > 0)
        {
            jumping = canDoubleJump = true;
            jumpingTimer = 0;
            groundedTimer = 0;
        }
        if (jumpingTimer > 0 && groundedTimer <= 0 && canDoubleJump)
        {
            jumping = true;
            canDoubleJump = false;
            jumpingTimer = 0;
        }
        if (jumpingTimer > 0)
            jumpingTimer -= Time.deltaTime;
        if (groundedTimer > 0)
            groundedTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        CalculateVelocity();
        controller.Move(0.5f * Time.fixedDeltaTime * (velocity + previousVelocity));
        previousVelocity = velocity;
    }

    private void ProcessInput()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space))
            jumpingTimer = persistanceTime;
        if (Input.GetKeyUp(KeyCode.Space))
            endJump = true;
    }

    private void CheckState()
    {
        if (controller.status.below)
            groundedTimer = persistanceTime;
    }

    private void CalculateVelocity()
    {
        // Horizontal Velocity
        velocity.x = Mathf.SmoothDamp(
            velocity.x,
            input.x * movementSpeed,
            ref currentSmoothVelocity,
            (controller.status.below) ? smoothTimeGrounded : smoothTimeAirborne);
        // Vertical Velocity
        if (controller.status.below || controller.status.above)
            velocity.y = 0;
        if (jumping)
        {
            jumping = false;
            velocity.y = initialJumpVelocity;
        }
        if (endJump)
        {
            endJump = false;
            if (velocity.y > 0)
                velocity.y *= endJumpDamping;
        }
        velocity.y += gravity * Time.fixedDeltaTime;
    }
}
