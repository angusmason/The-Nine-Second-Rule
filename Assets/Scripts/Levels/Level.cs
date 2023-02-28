using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
        [SerializeField] ParticleSystem system;
        SpriteRenderer spriteRenderer;
        [SerializeField] float playerHeightThreshold;
        float randomX;
        float randomY;
        LevelSelectManager manager;
        bool completed;
        Vector3 originalPosition;
        Crossfade crossfade;

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
            var lerpSpeed = 20 * Time.deltaTime;
            var selected = Mathf.Abs
                (transform.position.x - player.transform.position.x) < threshold;

            spriteRenderer.transform.localScale = Vector3.one * Mathf.Lerp(
                spriteRenderer.transform.localScale.x,
                selected ? selectedSize : unselectedSize,
                lerpSpeed
            );
            levelNumber.transform.localScale = 8.5f * spriteRenderer.transform.localScale;
            spriteRenderer.sortingOrder = selected ? 1 : 0;
            Color colour = LevelColour(selected, completed);
            levelNumber.color = Color.Lerp(
                levelNumber.color,
                colour,
                lerpSpeed
            );
            spriteRenderer.color = Color.Lerp(
                spriteRenderer.color,
                colour,
                lerpSpeed
            );
            levelNumber.text = (buildIndex + 1).ToString();
            var emission = system.emission;
            emission.rateOverTime = new(selected ? 40 : 0);

            var position = spriteRenderer.transform.position;
            position.x = originalPosition.x
                + Mathf.Sin(randomX + Time.time / 2) / 5;
            position.y = originalPosition.y
                + Mathf.Sin(buildIndex + Time.time / 2) / 3
                + Mathf.Sin(buildIndex + Time.time * 2) / 7
                + randomY;
            spriteRenderer.transform.position = position;

            double? timeCompleted = LevelSaver.GetLevel(buildIndex)?.TimeMilliseconds;
            bestTime.text =
                selected
                    ? timeCompleted == null
                        ? "Not completed"
                        : $@"Best Time: {TimeSpan.FromMilliseconds
                            ((double)timeCompleted):s's 'fff'ms'}"
                    : string.Empty;

            if (manager.levelLoading) return;

            if (selected && Mathf.Abs
                (transform.position.y - player.position.y) < playerHeightThreshold)
            {
                manager.levelLoading = true;
                player.GetComponent<PlayerController>()
                    .DisableMotion();
                crossfade.FadeIn(
                    () => SceneManager.LoadScene(buildIndex + 1)
                );
            }
        }

        Color LevelColour(bool selected, bool completed)
            => selected
                ? completed
                    ? new Color(0.99f, 0.5f, 0.83f)
                    : new Color(0.67f, 0.47f, 0.78f)
                : completed
                    ? new Color(0.84f, 0.35f, 0.68f)
                    : new Color(0, 0, 0);
    }
}
