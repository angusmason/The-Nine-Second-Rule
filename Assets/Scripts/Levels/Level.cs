using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        [SerializeField] ParticleSystem system;
        SpriteRenderer spriteRenderer;
        [SerializeField] float playerHeightThreshold;
        Vector3 originalPosition;
        float randomX;
        float randomY;
        LevelSelectManager manager;
        bool completed;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalPosition = transform.position;
            randomX = Random.Range(-2 * Mathf.PI, 2 * Mathf.PI);
            randomY = Random.Range(-0.3f, 0.5f);
            manager = transform.parent.GetComponent<LevelSelectManager>();
            completed = LevelSaver.LevelCompleted(buildIndex);
        }

        void Update()
        {
            var lerpSpeed = 20 * Time.deltaTime;
            var selected = Mathf.Abs
                (originalPosition.x - player.transform.position.x) < threshold;
            transform.localScale = Vector3.one * Mathf.Lerp(
                transform.localScale.x,
                selected ? selectedSize : unselectedSize,
                lerpSpeed
            );

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
            spriteRenderer.sortingOrder = selected ? 1 : 0;
            levelNumber.text = (buildIndex + 1).ToString();
            var emission = system.emission;
            emission.rateOverTime = new(selected ? 40 : 0);

            var position = transform.position;
            position.x = originalPosition.x
                + Mathf.Sin(randomX + Time.time / 2) / 5;
            position.y = originalPosition.y
                + Mathf.Sin(buildIndex + Time.time / 2) / 3
                + Mathf.Sin(buildIndex + Time.time * 2) / 7
                + randomY;
            transform.position = position;

            if (manager.levelLoading) return;

            if (selected && Mathf.Abs
                (originalPosition.y - player.position.y) < playerHeightThreshold)
            {
                manager.levelLoading = true;
                FindFirstObjectByType<Crossfade>().FadeOut(
                    () => SceneManager.LoadScene(buildIndex + 1));
            }
        }

        Color LevelColour(bool selected, bool completed)
            => selected
                ? completed
                    ? new Color(61, 227, 105)
                    : new Color(255, 181, 20)
                : completed
                    ? new Color(40, 138, 66)
                    : new Color(0, 0, 0);
    }
}
