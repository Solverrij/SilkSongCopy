using UnityEngine;

public class PlayerCore : MonoBehaviour
{
    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundRayOffset = 0.1f;
    public float groundRayLength = 0.2f;

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        // Lock rotation for platformer feel
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ |
                         RigidbodyConstraints.FreezePositionZ;
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(
            transform.position + Vector3.down * groundRayOffset,
            Vector3.down,
            groundRayLength,
            groundLayer
        );
    }
}
