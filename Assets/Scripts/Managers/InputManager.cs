using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    public static InputManager Instance { get; private set; }
    public Vector2 Movement { get; private set; }
    public bool JumpWasPressed { get; private set; }
    public bool JumpIsHeld { get; private set; }
    public bool JumpWasReleased { get; private set; }
    public bool IsSprinting { get; private set; }

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    private PlayerControls playerControls;

    void Awake()
    {
        playerControls = new PlayerControls();
        moveAction = playerControls.Player.Move;
        jumpAction = playerControls.Player.Jump;
        sprintAction = playerControls.Player.Sprint;

        Instance = this;
    }

    void Update()
    {
        Movement = moveAction.ReadValue<Vector2>();
        JumpWasPressed = jumpAction.WasPressedThisFrame();
        JumpIsHeld = jumpAction.IsPressed();
        JumpWasReleased = jumpAction.WasReleasedThisFrame();
        IsSprinting = sprintAction.IsPressed();
    }

    void OnEnable()
    {
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();
    }
}