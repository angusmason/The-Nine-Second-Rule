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
        SpriteRenderer spriteRenderer;
        [SerializeField] float playerHeightThreshold;
        float originalHeight;
        float randomOffset;
        LevelSelectManager manager;
        bool completed;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            originalHeight = transform.position.y;
            randomOffset = Random.Range(-0.3f, 0.5f);
            manager = transform.parent.GetComponent<LevelSelectManager>();
            completed = LevelSaver.LevelCompleted(buildIndex);
        }

        void Update()
        {
            if (manager.levelLoading) return;
            var lerpSpeed = 20 * Time.deltaTime;
            var selected = Mathf.Abs
                (transform.position.x - player.transform.position.x) < threshold;
            transform.localScale = Vector3.one * Mathf.Lerp(
                transform.localScale.x,
                selected ? selectedSize : unselectedSize,
                lerpSpeed
            );

            levelNumber.color = Color.Lerp(
                levelNumber.color,
                LevelColour(selected, completed),
                lerpSpeed
            );
            spriteRenderer.color = Color.Lerp(
                spriteRenderer.color,
                LevelColour(selected, completed),
                lerpSpeed
            );
            spriteRenderer.sortingOrder = selected ? 1 : 0;
            levelNumber.text = (buildIndex + 1).ToString();

            if (selected && Mathf.Abs
                (originalHeight - player.position.y) < playerHeightThreshold)
            {
                FindFirstObjectByType<Crossfade>().FadeOut(
                    () => SceneManager.LoadScene(buildIndex + 1));
                manager.levelLoading = true;
            }

            var position = transform.position;
            position.y = originalHeight
                + Mathf.Sin(buildIndex + Time.time / 2) / 3
                + Mathf.Sin(buildIndex + Time.time * 2) / 7
                + randomOffset;
            transform.position = position;
        }

        Color LevelColour(bool selected, bool completed)
            => selected
                ? completed
                    ? Color.white
                    : Color.white
                : completed
                    ? Color.black
                    : Color.black;
    }
}
