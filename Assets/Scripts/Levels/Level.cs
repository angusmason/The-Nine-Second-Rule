using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace TNSR.Levels
{
    public class Level : MonoBehaviour
    {
        [HideInInspector] public Transform player;
        [SerializeField] float threshold;
        [SerializeField] float selectedSize;
        [SerializeField] float unselectedSize;
        [HideInInspector] public int buildIndex;
        [SerializeField] TextMeshProUGUI levelNumber;
        [SerializeField] TextMeshProUGUI bestTime;
        SpriteRenderer spriteRenderer;
        [SerializeField] float playerHeightThreshold;
        float randomX;
        float randomY;
        bool completed;
        Vector3 originalPosition;
        Crossfade crossfade;
        public Color colour;
        public SpriteRenderer background;
        new Light2D light;

        void Start()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (buildIndex % 10 == 0)
                transform.position += Vector3.up * 2;
            originalPosition = spriteRenderer.transform.position;
            randomX = Random.Range(-2 * Mathf.PI, 2 * Mathf.PI);
            randomY = Random.Range(-0.3f, 0.5f);
            completed = LevelSaver.GetLevel(buildIndex) != null;
            crossfade = FindFirstObjectByType<Crossfade>();
            light = spriteRenderer.GetComponent<Light2D>();
        }

        void Update()
        {
            var lerpSpeed = 6 * Time.deltaTime;
            var selected = Mathf.Abs
                (transform.position.x - player.transform.position.x) < threshold;

            spriteRenderer.transform.localScale = Vector3.one * Mathf.Lerp(
                spriteRenderer.transform.localScale.x,
                selected ? selectedSize : unselectedSize,
                lerpSpeed
            );
            levelNumber.transform.localScale = 8.5f * spriteRenderer.transform.localScale;
            spriteRenderer.sortingOrder = selected ? 1 : 0;
            Color spriteColour = LevelColour(selected, completed);
            levelNumber.color = Color.Lerp(
                levelNumber.color,
                spriteColour,
                lerpSpeed
            );
            spriteRenderer.color = Color.Lerp(
                spriteRenderer.color,
                spriteColour,
                lerpSpeed
            );
            levelNumber.text = (buildIndex + 1).ToString();

            var position = spriteRenderer.transform.position;
            position.x = originalPosition.x
                + Mathf.Sin(randomX + Time.time / 2) / 5;
            position.y = originalPosition.y
                + Mathf.Sin(buildIndex + Time.time / 2) / 3
                + Mathf.Sin(buildIndex + Time.time * 2) / 7
                + randomY;
            spriteRenderer.transform.position = position;

            var timeCompleted = LevelSaver.GetLevel(buildIndex)?.TimeMilliseconds;
            bestTime.text =
                selected
                    ? timeCompleted == null
                        ? "Not completed"
                        : $@"Best Time: {TimeSpan.FromMilliseconds
                            ((double)timeCompleted):s\.fff\s}"
                    : string.Empty;
            Camera.main.backgroundColor = Color.Lerp(
                Camera.main.backgroundColor,
                selected ? colour : Camera.main.backgroundColor,
                lerpSpeed
            );
            background.color = Color.Lerp(
                background.color,
                selected ? colour : background.color,
                lerpSpeed
            );
            light.color = Color.Lerp(
                light.color,
                selected ? colour : light.color,
                lerpSpeed
            );
            light.intensity = Mathf.Lerp(
                light.intensity,
                selected ? 1 : 0,
                lerpSpeed
            );
            if (crossfade.FadingState == Crossfade.Fading.FadingIn) return;
            if (!selected) return;
            if (Mathf.Abs(transform.position.y - player.position.y) < playerHeightThreshold)
            {
                var vacuum = new GameObject("Vacuum");
                vacuum.transform.position = transform.position;
                player.transform.parent = vacuum.transform;
                player.GetComponent<PlayerController>()
                    .DisableMotion();
                crossfade.FadeIn(
                    () => SceneManager.LoadScene(buildIndex + 1),
                    (alpha) =>
                    {
                        vacuum.transform.localScale = Vector3.one * Mathf.Lerp(vacuum.transform.localScale.x, 1 - alpha, 0.1f);
                        vacuum.transform.localRotation = Quaternion.Euler(0, 0,
                            Mathf.Lerp(vacuum.transform.localRotation.eulerAngles.z, 360 * alpha, 0.1f)
                        );
                        player.transform.localRotation = Quaternion.Euler(0, 0,
                            Mathf.Lerp(player.transform.localRotation.eulerAngles.z, 360 * 2 * alpha, 0.1f)
                        );
                    }
                );
            }
        }

        Color LevelColour(bool selected, bool completed)
            => selected
                ? completed
                    ? new Color32(255, 255, 255, 255) // Selected and completed
                    : new Color32(126, 126, 126, 255) // Selected and not completed
                : completed
                    ? new Color32(255, 255, 255, 126) // Not selected and completed
                    : new Color32(126, 126, 126, 126); // Not selected and not completed
    }
}
