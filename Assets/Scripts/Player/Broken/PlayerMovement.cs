using UnityEngine;

public class PlayerMovement : MonoBehaviour

{

    [Header("Movement Settings")]

    [SerializeField] private float speed = 5f;

    [SerializeField] private float jumpForce = 7f;

    [SerializeField] private LayerMask groundLayer;



    private Rigidbody rb;

    private JumpState currentJumpState = JumpState.Grounded;

    private bool jumpRequested = false;



    private enum JumpState

    {

        Grounded,

        Jumping,

        Falling

    }



    void Start()

    {

        rb = GetComponent<Rigidbody>();

    }



    void Update()

    {

        // Jump input

        if (Input.GetKeyDown(KeyCode.Space) && currentJumpState == JumpState.Grounded)

        {

            jumpRequested = true;

        }

    }



    void FixedUpdate()

    {

        Move();

        HandleJump();

        UpdateJumpState();

    }



    private void Move()

    {

        float horizontal = 0f;



        if (Input.GetKey(KeyCode.A)) horizontal = -1f;

        if (Input.GetKey(KeyCode.D)) horizontal = 1f;



        Vector3 newVelocity = rb.linearVelocity;

        newVelocity.x = horizontal * speed;

        rb.linearVelocity = newVelocity;

    }



    private void HandleJump()

    {

        if (jumpRequested)

        {

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            jumpRequested = false;

        }

    }



    private void UpdateJumpState()

    {

        if (rb.linearVelocity.y > 0.1f)

            currentJumpState = JumpState.Jumping;

        else if (rb.linearVelocity.y < -0.1f)

            currentJumpState = JumpState.Falling;

        else

            currentJumpState = JumpState.Grounded;

    }

}

