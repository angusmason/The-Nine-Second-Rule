using System;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    Vector2 startPosition;
    [SerializeField]

    Vector2 endPosition;
    [SerializeField] float t;
    [SerializeField] float speed;

    void Update()
    {
        transform.position = Vector2.Lerp(
            startPosition,
            endPosition,
            Mathf.Sin(t + Time.time * speed) / 2 + 0.5f
        );
    }
}
