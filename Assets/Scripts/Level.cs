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
        bool levelLoading;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            if (levelLoading) return;
            var selected = Mathf.Abs
                (transform.position.x - player.transform.position.x) < threshold;
            transform.localScale = Vector3.one * Mathf.Lerp(
                transform.localScale.x,
                selected ? selectedSize : unselectedSize,
                10 * Time.deltaTime
            );
            levelNumber.color = Color.Lerp(
                levelNumber.color,
                selected ? Color.white : Color.black,
                10 * Time.deltaTime
            );
            spriteRenderer.color = Color.Lerp(
                spriteRenderer.color,
                selected ? Color.white : Color.black,
                10 * Time.deltaTime
            );
            spriteRenderer.sortingOrder = selected ? 1 : 0;
            levelNumber.text = (buildIndex + 1).ToString();

            if (selected && Mathf.Abs
                (transform.position.y - player.position.y) < playerHeightThreshold)
            {
                FindFirstObjectByType<Crossfade>().FadeOut(
                    () => SceneManager.LoadScene(buildIndex));
                levelLoading = true;
            }
        }
    }
}
