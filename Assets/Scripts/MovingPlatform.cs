using UnityEngine;

namespace TNSR
{
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField] Vector2 startPosition;
        [SerializeField] Vector2 endPosition;
        [SerializeField] float offset;

        void Update()
        {
            transform.position = Vector2.Lerp(
                startPosition,
                endPosition,
                Mathf.Sin(offset + Mathf.PI * (Time.time + 0.5f)) / 2 + 0.5f
            );
        }
    }
}
