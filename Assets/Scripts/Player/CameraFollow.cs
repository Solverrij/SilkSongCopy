using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; // speler
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset;

    private void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;

        // Smooth follow
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.position = smoothedPosition;
    }
}
