using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Character2DController : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    [SerializeField] private InputManager input;
    void Start()
    {
        input = InputManager.Instance;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        var raw = input.Movement;
        var movement = new Vector2(raw.x, raw.y);
        rb.MovePosition(rb.position + speed * Time.fixedDeltaTime * movement);
    }
}
