using UnityEngine;

namespace TNSR
{
    public class Patrol : MonoBehaviour
    {
        [SerializeField] float speed;
        [SerializeField] float distance;

        bool movingRight = true;

        [SerializeField] Transform groundDetection;

        void Update()
        {
            transform.Translate(speed * Time.deltaTime * Vector2.right);
            RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distance);
            if (!groundInfo.collider)
            {
                transform.eulerAngles = new Vector3(0, movingRight ? -180 : 0, 0);
                movingRight = !movingRight;
            }
        }
    }
}
