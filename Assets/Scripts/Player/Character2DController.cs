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

    void FixedUpdate()
    {
        if (movementStats == null) return;
        CollisionChecks();

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
}
