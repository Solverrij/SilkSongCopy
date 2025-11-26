using UnityEngine;
using System.Collections;

public class PlayerLedgeGrab : MonoBehaviour
{
    [Header("Raycasts")]
    public float rayStartOffset = 0.5f;
    public float ray1Height = 1f;    // Upper ray: must NOT hit
    public float ray2Height = 0.5f;  // Lower ray: must hit
    public float rayLength = 0.6f;
    public LayerMask climbableLayer;
    [Header("Climb Settings")]
    public Vector3 grabOffset;       // Where player ends up relative to start
    public float grabSpeed = 3f;

    PlayerCore core;
    bool controlsEnabled = true;

    void Start()
    {
        core = GetComponent<PlayerCore>();
    }

    void Update()
    {
        if (!controlsEnabled) return;

        CheckForLedge();
    }

    void CheckForLedge()
    {
        float dir = GetDirectionInput();
        if (dir == 0) return;
        if (!Input.GetKey(KeyCode.Space)) return;

        Vector3 dirVec = dir > 0 ? Vector3.right : Vector3.left;
        float start = dir > 0 ? rayStartOffset : -rayStartOffset;

        // UPPER RAY - must NOT hit
        if (Physics.Raycast(transform.position + new Vector3(start, ray1Height, 0),
                            dirVec, rayLength * 10, climbableLayer))
        {
            Debug.Log("No ledge - upper ray hit");
            return;
        }
           

        // LOWER RAY - MUST hit
        if (!Physics.Raycast(transform.position + new Vector3(start, ray2Height, 0),
                             dirVec, rayLength, climbableLayer))
        {
            Debug.Log("No ledge - lower ray missed");
            return;
        }
           

        Debug.Log("LEDGE DETECTED!");

        StartCoroutine(LedgeClimbRoutine(dir));
    }

    IEnumerator LedgeClimbRoutine(float dir)
    {
        controlsEnabled = false;

        core.rb.linearVelocity = Vector3.zero;
        core.rb.angularVelocity = Vector3.zero;
        core.rb.useGravity = false;

        Vector3 target = transform.position + new Vector3(grabOffset.x * dir, grabOffset.y, 0);

        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, target, grabSpeed * Time.deltaTime);
            core.rb.MovePosition(newPos);
            yield return null;
        }

        core.rb.useGravity = true;
        controlsEnabled = true;
    }


    private float GetDirectionInput()
    {
        if (Input.GetKey(KeyCode.A)) return -1f;
        if (Input.GetKey(KeyCode.D)) return 1f;
        return 0f;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // Visualize rays
        Gizmos.DrawRay(transform.position + new Vector3(rayStartOffset, ray1Height, 0),
                       Vector3.right * rayLength);

        Gizmos.DrawRay(transform.position + new Vector3(rayStartOffset, ray2Height, 0),
                       Vector3.right * rayLength);

        Gizmos.DrawRay(transform.position + new Vector3(-rayStartOffset, ray1Height, 0),
                       Vector3.left * rayLength);

        Gizmos.DrawRay(transform.position + new Vector3(-rayStartOffset, ray2Height, 0),
                       Vector3.left * rayLength);
    }
}
