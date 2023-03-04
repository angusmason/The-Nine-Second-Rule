using System;
using TMPro;
using UnityEngine;
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
        LevelSelectManager manager;
        bool completed;
        Vector3 originalPosition;
        Crossfade crossfade;
        public Color colour;

        void Start()
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            originalPosition = spriteRenderer.transform.position;
            randomX = Random.Range(-2 * Mathf.PI, 2 * Mathf.PI);
            randomY = Random.Range(-0.3f, 0.5f);
            manager = transform.parent.GetComponent<LevelSelectManager>();
            completed = LevelSaver.GetLevel(buildIndex) != null;
            crossfade = FindFirstObjectByType<Crossfade>();
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

            Camera.main.backgroundColor = Color.Lerp(
                Camera.main.backgroundColor,
                selected ? colour : Camera.main.backgroundColor,
                lerpSpeed
            );

            var timeCompleted = LevelSaver.GetLevel(buildIndex)?.TimeMilliseconds;
            bestTime.text =
                selected
                    ? timeCompleted == null
                        ? "Not completed"
                        : $@"Best Time: {TimeSpan.FromMilliseconds
                            ((double)timeCompleted):s\.fff\s}"
                    : string.Empty;

            if (manager.levelLoading) return;
            if (!selected) return;
            if (Mathf.Abs(transform.position.y - player.position.y) < playerHeightThreshold)
            {
                manager.levelLoading = true;
                var vacuum = new GameObject("Vacuum");
                vacuum.transform.position = transform.position;
                player.transform.parent = vacuum.transform;
                player.GetComponent<PlayerController>()
                    .DisableMotion();
                vacuum.transform.position = transform.position;
                player.transform.parent = vacuum.transform;
                crossfade.FadeIn(
                    () => SceneManager.LoadScene(buildIndex + 1),
                    (alpha) =>
                    {
                        vacuum.transform.localScale = Vector3.one * (1 - alpha);
                        vacuum.transform.localRotation = Quaternion.Euler(0, 0, 360 * alpha);
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
