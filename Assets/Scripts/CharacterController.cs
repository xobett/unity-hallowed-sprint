using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
{
    private PlayerControls playerControls;
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    void Awake()
    {
        playerControls = new PlayerControls();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        var raw = playerControls.Player.Move.ReadValue<Vector2>();
        var movement = new Vector2(raw.x, 0);
        rb.MovePosition(rb.position + speed * Time.fixedDeltaTime * movement);

        if (playerControls.Player.Jump.WasPressedThisFrame())
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }
    #region ENABLE / DISABLE
    void OnEnable()
    {
        playerControls.Enable();
    }

    void OnDisable()
    {
        playerControls.Disable();   
    }

    #endregion ENABLE/DISABLE
}
