using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Character2DController : MonoBehaviour
{
    private InputManager input;
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private SOPlayerMovementStats movementStats;
    private Vector2 moveVelocity;
    bool isGrounded;

    private float verticalVelocity = 0f;
    bool isJumping;
    bool isFastFalling; // controls our gravity
    bool isFalling;
    float fastFallTime;
    float fastFallReleaseSpeed;
    [SerializeField] int jumpsUsed;

    float apexPoint; // highest point of a jump
    float timePastApexThreshold;
    bool isPastApexThreshold;

    float jumpBufferTimer; // used to jump slightly before touching ground
    bool jumpReleasedDuringBuffer;

    float coyoteTimer; // used to jump slightly after leaving a platform

    bool bumpedHead;

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
        // press jump
        if (input.JumpWasPressed)
        {
            // reset all jump buffer values / timers to activate again
            jumpBufferTimer = movementStats.JumpBufferTime;
            jumpReleasedDuringBuffer = false;
        }

        // release jump
        if (input.JumpWasReleased)
        {
            if (jumpBufferTimer > 0f)
            {
                jumpReleasedDuringBuffer = true;
            }

            if (isJumping && verticalVelocity > 0f)
            {
                if (isPastApexThreshold)
                {
                    isPastApexThreshold = false;
                    fastFallTime = movementStats.TimeForUpwardsCancel;
                    verticalVelocity = 0;
                }
                else
                {
                    fastFallReleaseSpeed = verticalVelocity;
                }

                isFastFalling = true;
            }
        }

        // initiate jump with buffering and coyote
        if (jumpBufferTimer > 0f && !isJumping && (!isGrounded || coyoteTimer > 0f))
        {
            InitiateJump(1);

            if (jumpReleasedDuringBuffer)
            {
                isFastFalling = true;
                fastFallReleaseSpeed = verticalVelocity;
            }
        }
        // double jump
        if (jumpBufferTimer > 0 && isJumping && jumpsUsed < movementStats.NumberOfJumpsAllowed)
        {
            isFastFalling = false;
            InitiateJump(1);
        }
        // air jump after coyote time lapsed
        if (jumpBufferTimer > 0 && isFalling && jumpsUsed < movementStats.NumberOfJumpsAllowed)
        {
            isFastFalling = false;
            InitiateJump(2);
        }
        // landed
        if ((isJumping || isFalling) && isGrounded && verticalVelocity <= 0f)
        {
            isJumping = false;
            isFalling = false;
            isFastFalling = false;
            fastFallTime = 0f;
            isPastApexThreshold = false;
            jumpsUsed = 0;

            verticalVelocity = Physics2D.gravity.y;
        }
    }

    void InitiateJump(int numberOfJumps)
    {
        if (!isJumping) isJumping = true;

        jumpBufferTimer = 0f;
        jumpsUsed += numberOfJumps;
        verticalVelocity = movementStats.InitialJumpVelocity;
    }

    void Jump()
    {
        // apply gravity while jumping
        if (isJumping)
        {
            // check for head bump
            if (bumpedHead)
            {
                isFastFalling = true;
            }

            // gravity on ascending
            if (verticalVelocity >= 0f)
            {
                // apex controls
                apexPoint = Mathf.InverseLerp(movementStats.InitialJumpVelocity, 0f, verticalVelocity);
                if (apexPoint > movementStats.ApexThreshold)
                {
                    if (!isPastApexThreshold)
                    {
                        isPastApexThreshold = true;
                        timePastApexThreshold = 0f;
                    }

                    if (isPastApexThreshold)
                    {
                        timePastApexThreshold += Time.fixedDeltaTime;
                        if (timePastApexThreshold < movementStats.ApexHangTime)
                        {
                            verticalVelocity = 0f;
                        }
                        else
                        {
                            verticalVelocity = -0.01f;
                        }
                    }
                }
                // gravity on ascending but not past apex threshold
                else
                {
                    verticalVelocity += movementStats.Gravity * Time.fixedDeltaTime;
                    if (isPastApexThreshold) isPastApexThreshold = false;
                }
            }

            //gravity on descending
            else if (!isFastFalling)
            {
                verticalVelocity += movementStats.Gravity * movementStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }

            else if (verticalVelocity < 0f)
            {
                if (!isFalling) isFalling = true;
            }
        }
        // jump cut
        if (isFastFalling)
        {
            if (fastFallTime >= movementStats.TimeForUpwardsCancel)
            {
                verticalVelocity += movementStats.Gravity * movementStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (fastFallTime < movementStats.TimeForUpwardsCancel)
            {
                verticalVelocity = Mathf.Lerp(fastFallReleaseSpeed, 0f, (fastFallTime / movementStats.TimeForUpwardsCancel));
            }

            fastFallTime += Time.fixedDeltaTime;
        }

        // normal gravity when falling
        if (!isGrounded && !isJumping)
        {
            if (!isFalling) isFalling = true;

            verticalVelocity += movementStats.Gravity * Time.fixedDeltaTime;
        }


        // clamp fall speed
        verticalVelocity = Mathf.Clamp(verticalVelocity, -movementStats.MaxFallSpeed, 50f);

        rb.linearVelocity = new Vector2(rb.linearVelocityX, verticalVelocity);
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
        if (!isGrounded) coyoteTimer -= Time.deltaTime;
        else coyoteTimer = movementStats.JumpCoyoteTime;
    }

    #endregion Timers
}
