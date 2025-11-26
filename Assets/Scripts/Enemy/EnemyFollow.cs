using System;
using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    private Transform player;

    public void StartFollow(Transform target)
    {
        player = target;
    }
    void Start()
    {
        Console.WriteLine("dit iseen script dat de enemy het volgt wanneer het de collision in gaat");
    }

    void Update()
    {
        if (player == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
            //transform.position = player.transform.position *moveSpeed * Time.deltaTime;

        );
    }
}
