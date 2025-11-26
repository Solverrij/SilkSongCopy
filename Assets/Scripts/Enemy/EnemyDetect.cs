using UnityEngine;

public class EnemyDetect : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player"; 
    [SerializeField] private EnemyFollow followScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            followScript.StartFollow(other.transform);
            Debug.Log($"{name} detected player: {other.gameObject.name}", this);
        }
    }
}