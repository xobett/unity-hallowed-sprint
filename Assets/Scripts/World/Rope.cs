using UnityEngine;

public class Rope : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        var ropeHang = collision.gameObject.GetComponentInParent<RopeHang>();
        if (ropeHang == null) return;

        ropeHang.GrabRope(transform);
    }
}
