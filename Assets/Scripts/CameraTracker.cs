using UnityEngine;

namespace TNSR
{
    [RequireComponent(typeof(Camera))]
    public class CameraTracker : MonoBehaviour
    {
        [SerializeField] Transform player;
        const float smoothTime = .2f;

        Vector3 velocity;

        void LateUpdate()
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                player.position,
                ref velocity,
                smoothTime * Time.timeScale
            );
        }
    }
}
