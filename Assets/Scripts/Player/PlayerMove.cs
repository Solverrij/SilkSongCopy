using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lastInput;

    PlayerCore core;

    void Start()
    {
        core = GetComponent<PlayerCore>();
    }

    void Update()
    {
        float input = Input.GetAxis("Horizontal");
        lastInput = input;

        // Movement
        transform.Translate(Vector3.right * input * moveSpeed * Time.deltaTime);

        // Flip player model
        if (input != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(input); // 1 when moving right, -1 when left
            transform.localScale = scale;
        }

        HandleAnimations(input);
    }

    void HandleAnimations(float input)
    {
        if (!core.IsGrounded()) return;

        if (input != 0)
            core.anim.Play("PlayerRun"); // 1 animation for both directions
        else
            core.anim.Play("Idle");
    }
}
