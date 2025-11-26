using UnityEngine;

public class TMovement : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float z = Input.GetAxis("Horizontal");
        float x = Input.GetAxis("Vertical");

       // Vector3 movement = new Vector3(Time.deltaTime);
    }
}
