using UnityEngine;
using UnityEngine.SceneManagement;

namespace TNSR
{
    [RequireComponent(typeof(CameraController))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] Transform player;

        void Start()
        {
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (buildIndex != 0)
                GameObject.Find("/Background").GetComponent<SpriteRenderer>().color = Resources
                    .Load<LevelColours>("LevelColours")
                    .levelColours[buildIndex - 1];
        }

        void LateUpdate()
        {
            transform.position = Vector3.Lerp(
                transform.position,
                player.position,
                Time.deltaTime * 5
            );
        }
    }
}
