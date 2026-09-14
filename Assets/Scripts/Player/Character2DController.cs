using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Character2DController : MonoBehaviour
{
    // input and rb
    private InputManager input;
    private Rigidbody2D rb;

    // horizontal movement
    [Header("Movement")]
    [SerializeField] private SOPlayerMovementStats movementStats;
    private Vector2 moveVelocity;

    // jump and vertical movement
    bool isGrounded;

    float verticalVelocity;
    bool isJumping;

    float jumpBufferTimer;

    // TODO
    // Implement: Jump Buffer, Coyote Time, and Head Bump
    // Implement: Holded jump and jump cut

    [Header("Colliders")]
    [SerializeField] private BoxCollider2D bodyCollider;
    [SerializeField] private BoxCollider2D feetCollider;

    void Start()
    {
        input = InputManager.Instance;
        rb = GetComponent<Rigidbody2D>();
        moveVelocity = Vector2.zero;

        if (movementStats == null)
        {
            Debug.LogError("Movement stats are not assigned");
            Debug.Break();
        }
    }

    void Update()
    {
        JumpTimers();
        JumpChecks();
    }

    void FixedUpdate()
    {
        if (movementStats == null) return;
        CollisionChecks();
        Gravity();
        Jump();

        float acceleration = isGrounded ? movementStats.GroundAcceleration : movementStats.AirAcceleration;
        float deceleration = isGrounded ? movementStats.GroundDeceleration : movementStats.AirDeceleration;
        Move(acceleration, deceleration); ;
    }

    private void Move(float acceleration, float deceleration)
    {
        // receive raw input on horizontal axis
        Vector2 targetVelocity = new(input.Movement.x, 0f);

        // if our player is moving, it multiplies the input by the speed, which depends of its sprinting action
        // please do note that Vector2.Lerp() interpolates smoothly our current speed, moveVelocity, with the target
        // that is being calculated with the input

        // also note that we multiply by deltaTime to ensure constant movement regardless of the framerate
        if (input.Movement != Vector2.zero)
        {
            targetVelocity *= input.IsSprinting ? movementStats.MaxRunSpeed : movementStats.MaxWalkSpeed;
            moveVelocity = Vector2.Lerp(moveVelocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }

        // at the end, we move through the rb's linear velocity on X, 
        rb.linearVelocity = new Vector2(moveVelocity.x, rb.linearVelocityY);
    }

    void JumpChecks()
    {
        if (input.JumpWasPressed)
        {
            jumpBufferTimer = movementStats.JumpBufferTime;
        }

        if (jumpBufferTimer > 0 && !isJumping)
        {
            InitiateJump();
        }
    }

    void InitiateJump()
    {
        isJumping = true;
        jumpBufferTimer = 0f;
        verticalVelocity = movementStats.InitialJumpVelocity;
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocityX, verticalVelocity);
    }

    void Gravity()
    {
        float gravity = 0f;
        // make jump more responsive and less constant
        // if we are falling, make gravity stronger
        // if (verticalVelocity < 0) {
        //     gravity = movementStats.Gravity * movementStats.NoReleaseMultiplier;
        // }
        // else if (verticalVelocity > 0 && input.Jump.notPressed XD) {
        //     gravity = movementStats.Gravity * movementSTats.FastReleaseMultiplier;
        // }

        if (verticalVelocity <= 0 && isGrounded)
        {
            isJumping = false;
            verticalVelocity = 0;
        }
        else
        {
            verticalVelocity += movementStats.Gravity * Time.fixedDeltaTime;
            verticalVelocity = Mathf.Clamp(verticalVelocity, -movementStats.MaxFallSpeed, 50f);
        }
    }

    #region Collisions

    void IsGrounded()
    {
        // gets the position at its center lowest y coordinate
        var origin = new Vector2(feetCollider.bounds.center.x, feetCollider.bounds.min.y);
        var size = new Vector2(feetCollider.bounds.size.x, movementStats.GroundDetectionRayLength);

        // note that although our box has a size, we still do need to provide the distance at which we want to verify a hit
        var hit = Physics2D.BoxCast(origin, size, 0, Vector2.down, movementStats.GroundDetectionRayLength, movementStats.GroundLayer);

        if (hit) isGrounded = true;
        else isGrounded = false;
    }

    void OnDrawGizmos()
    {
        var origin = new Vector2(feetCollider.bounds.center.x, feetCollider.bounds.min.y);
        var size = new Vector2(feetCollider.bounds.size.x, movementStats.GroundDetectionRayLength);

        var color = Color.rebeccaPurple;

        // draws the area of the box cast, starting from the origin
        Debug.DrawRay(new Vector2(origin.x - size.x / 2, origin.y), Vector2.down * movementStats.GroundDetectionRayLength, color);
        Debug.DrawRay(new Vector2(origin.x + size.x / 2, origin.y), Vector2.down * movementStats.GroundDetectionRayLength, color);
        Debug.DrawRay(new Vector2(origin.x - size.x / 2, origin.y - movementStats.GroundDetectionRayLength), Vector2.right * size.x, color);
    }

    void CollisionChecks()
    {
        IsGrounded();
    }

    #endregion Collisions

    #region Timers

    void JumpTimers()
    {
        jumpBufferTimer -= Time.deltaTime;
    }

    #endregion Timers
}
