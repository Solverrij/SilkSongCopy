using UnityEngine;
using System.Collections;

public class LedgeClimb : MonoBehaviour
{
    [Header("Wall Cling/Slide Settings")]
    [Tooltip("The speed at which the player slides down the wall while clinging.")]
    [SerializeField] private float wallSlideSpeed = 1f;
    [Tooltip("The distance of the raycast used to detect a wall in front of the player.")]
    [SerializeField] private float wallCheckDistance = 0.6f;
    [Tooltip("The LayerMask for objects considered walls (usually the same as Ground).")]
    [SerializeField] private LayerMask wallLayer;

    [Header("Wall Jump Settings")]
    [Tooltip("The force applied when wall jumping. X is the horizontal push, Y is the vertical push.")]
    [SerializeField] private Vector2 wallJumpForce = new Vector2(10f, 12f);
    [Tooltip("Time in seconds to prevent the player from re-clinging immediately after a wall jump (coyote time).")]
    [SerializeField] private float wallJumpClingDisableTime = 0.25f;

    private Rigidbody rb;
    private bool isWallClinging = false;
    private float wallDirection = 0f; // -1 for left wall, 1 for right wall
    private float wallJumpTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on the GameObject. Wall movement disabled.");
            enabled = false;
        }
    }

    void Update()
    {
        // Decrement the timer used to disable wall clinging after a wall jump
        wallJumpTimer -= Time.deltaTime;

        // Wall Jump Input: Triggered when clinging and Space is pressed
        if (isWallClinging && Input.GetKeyDown(KeyCode.Space))
        {
            PerformWallJump();
        }
    }

    void FixedUpdate()
    {
        // 1. Check for wall contact and user input
        CheckForWallClingAttempt();

        // 2. Apply Wall Cling/Slide logic
        if (isWallClinging)
        {
            HandleWallCling();
        }
    }

   
    private void CheckForWallClingAttempt()
    {
        // A. If the player is grounded or cannot cling yet (post-jump timer), reset cling state and exit.
        // Assuming 'Grounded' is handled by your other script, this simply checks if the wall jump timer is active.
        if (wallJumpTimer > 0f)
        {
            isWallClinging = false;
            wallDirection = 0f;
            return;
        }

        // B. Check for wall contact on the sides
        Vector3 origin = transform.position;

        bool wallLeft = Physics.Raycast(origin, Vector3.left, wallCheckDistance, wallLayer);
        bool wallRight = Physics.Raycast(origin, Vector3.right, wallCheckDistance, wallLayer);

        // C. Get horizontal input (A/D)
        float horizontalInput = 0f;
        if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;
        if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;

        // D. Determine if the player is actively pressing into a wall while not grounded (implicitly, since we ignore the jump timer)
        if (wallLeft && horizontalInput < 0)
        {
            isWallClinging = true;
            wallDirection = -1f; // Wall on the left
        }
        else if (wallRight && horizontalInput > 0)
        {
            isWallClinging = true;
            wallDirection = 1f; // Wall on the right
        }
        else
        {
            // If not pressing into a wall, or no wall detected, stop clinging
            isWallClinging = false;
            wallDirection = 0f;
        }
    }


    private void HandleWallCling()
    {
        // Zero out horizontal velocity to stick to the wall, and set a slow vertical slide speed.
        // This overrides the 'gravity' and 'falling' motion for this specific frame.
        rb.linearVelocity = new Vector3(0, -wallSlideSpeed, 0);
    }

    private void PerformWallJump()
    {
        // Calculate the direction vector: opposite of the wall direction, and upward
        // The jump direction is away from the wall (e.g., if wall is left (-1), jump is right (+1))
        Vector2 jumpVector = new Vector2(-wallDirection * wallJumpForce.x, wallJumpForce.y);

        // Immediately stop current velocity for a clean jump feel
        rb.linearVelocity = Vector3.zero;

        // Apply the jump force using VelocityChange for an immediate, non-mass-dependent effect
        rb.AddForce(jumpVector, ForceMode.VelocityChange);

        // Exit wall clinging state
        isWallClinging = false;

        // Set the timer to temporarily prevent re-clinging right after the jump
        wallJumpTimer = wallJumpClingDisableTime;
    }

  
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = transform.position;
        // Draw left wall check
        Gizmos.DrawLine(origin, origin + Vector3.left * wallCheckDistance);
        // Draw right wall check
        Gizmos.DrawLine(origin, origin + Vector3.right * wallCheckDistance);
    }
}