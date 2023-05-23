using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace TNSR
{
    [RequireComponent(typeof(CameraController))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] Transform player;
        PlayerController playerController;
        new Camera camera;
        LensDistortion lensDistortion;
        float originalIntensity;

        void Start()
        {
            playerController = player.GetComponent<PlayerController>();
            camera = GetComponent<Camera>();
            lensDistortion = camera.GetComponent<Volume>()
                .sharedProfile
                .components
                .Single(component => component is LensDistortion)
                    as LensDistortion;
            originalIntensity = lensDistortion.intensity.value;

            var buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (buildIndex != 0)
                camera.backgroundColor = Resources
                    .Load<LevelColours>("LevelColours")
                    .levelColours[buildIndex - 1];
        }

        void Update()
        {
            transform.position = Vector3.Lerp(
                transform.position,
                player.transform.position,
                Time.deltaTime * 5
            );
            lensDistortion.intensity.value = Mathf.Lerp(
                lensDistortion.intensity.value,
                playerController.isDashing ? originalIntensity + 0.3f : originalIntensity,
                Time.deltaTime * 5
            );
        }
    }
}
