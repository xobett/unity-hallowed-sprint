using System;
using Unity.Mathematics;
using UnityEngine;

[CreateAssetMenu(fileName = "SOPlayerMovementStats", menuName = "Scriptable Objects/SOPlayerMovementStats")]
public class SOPlayerMovementStats : ScriptableObject
{
    [Header("Walk")]
    [Range(1f, 100f)] public float MaxWalkSpeed = 12.5f;
    [Range(0.25f, 50f)] public float GroundAcceleration = 5f;
    [Range(0.25f, 50f)] public float GroundDeceleration = 20f;
    [Range(0.25f, 50f)] public float AirAcceleration = 5f;
    [Range(0.25f, 50f)] public float AirDeceleration = 5f;

    [Header("Run")]
    [Range(1f, 100f)] public float MaxRunSpeed = 20f;

    [Header("Grounded/Collision Checks")]
    public LayerMask GroundLayer;
    public float GroundDetectionRayLength = 0.02f;

    [Header("Jump")]
    [Range(1f, 6f)] public float JumpHeight = 3.5f;
    [Range(1f, 1.1f)] public float JumpHeightCompensationFactor = 1.054f;

    [Header("Jump Buffer")]
    [Range(0f, 1f)] public float JumpBufferTime = 0.125f;

    [Header("Gravity")]
    public float MaxFallSpeed = 26;
    public float TimeTillJumpApex = 0.35f;

    // gravity multiplier on fast jump
    public float OnJumpReleaseMultiplier = 1.5f;
    // gravity multiplier on fast jump
    public float OnFallMultiplier = 2.5f;

    public float Gravity { get; private set; }
    public float InitialJumpVelocity { get; private set; }

    void OnValidate()
    {
        CalculateValues();
    }

    void OnEnable()
    {
        CalculateValues();
    }

    private void CalculateValues()
    {
        // formula to calculate gravity
        // our jump height defines how tall we want to jump and our time till jump apex determines how fast or slow we want to get there
        // our gravity is calculated by the jump height and the time it takes to get there
        var adjustedJumpHeight = JumpHeight * JumpHeightCompensationFactor;
        Gravity = -(2f * adjustedJumpHeight) / Mathf.Pow(TimeTillJumpApex, 2f);
        InitialJumpVelocity = Math.Abs(Gravity) * TimeTillJumpApex;
    }
}