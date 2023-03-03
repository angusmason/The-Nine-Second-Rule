using UnityEngine;
using UnityEngine.SceneManagement;

namespace TNSR
{
    [RequireComponent(typeof(CameraController))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] Transform player;
        const float smoothTime = .2f;
        Vector3 velocity;

        void Start()
        {
            var camera = GetComponent<Camera>();

            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (buildIndex != 0)
                camera.backgroundColor = Resources
                    .Load<LevelColours>("LevelColours")
                    .levelColours[buildIndex - 1];
        }

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
