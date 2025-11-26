using UnityEngine;

public class EnemyDie : MonoBehaviour
{
    public float delay = 2f;

    void Start()
    {
        Destroy(gameObject, delay);
    }
}