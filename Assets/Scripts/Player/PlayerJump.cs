using UnityEngine;

public class PlayerJump : PlayerMove
{
    public float jumpForce = 7f;
    [SerializeField] float extraJumpTime = 0.2f;
    private float _jumpTimeCounter;
    PlayerMove move;

    PlayerCore core;

    void Start()
    {
        core = GetComponent<PlayerCore>();
        move = GetComponent<PlayerMove>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && core.IsGrounded())
        {
            core.rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            _jumpTimeCounter = extraJumpTime;

            if (move.lastInput < 0)
            {
                core.anim.Play("PlayerJumpLeft");
                Debug.Log("Jumped Left");
            }
                
            else if (move.lastInput > 0)
            {
                core.anim.Play("PlayerJumpRight");
                Debug.Log("Jumped Right");
            }
              
            else
            {
                core.anim.Play("PlayerJump");
                Debug.Log("Jumped");
            }
                
        }

        /*   if ( Input.GetKey(KeyCode.Space) && _jumpTimeCounter > 0)
           {
               core.rb.AddForce(Vector3.up * jumpForce * Time.deltaTime, ForceMode.Impulse);
               _jumpTimeCounter -= Time.deltaTime;

           } */


    }
}
