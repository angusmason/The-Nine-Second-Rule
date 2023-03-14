using UnityEngine;

namespace TNSR
{
    public class Patrol : MonoBehaviour
    {
        Vector2 StartPosition;
        const float distance = 4.689f;
        [SerializeField] float offset;

        void Start()
        {
            StartPosition = transform.position;
        }
        void Update()
        {
            transform.position = Vector2.Lerp(
                StartPosition,
                StartPosition + Vector2.right * distance,
                Mathf.PingPong(Time.time + offset, 1)
            );
        }
    }
}
