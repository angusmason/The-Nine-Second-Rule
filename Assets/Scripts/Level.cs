using UnityEngine;

namespace TNSR.Levels
{
    public class LevelSelect : MonoBehaviour
    {
        [SerializeField] Transform player;
        [SerializeField] float threshold;
        [SerializeField] float size;

        void Update()
        {
            if (Mathf.Abs(transform.position.x - player.transform.position.x) < threshold)
            {
                transform.localScale = new Vector3(size, size, 0);
            }

            else
            {
                transform.localScale = new Vector3(0.3f, 0.3f, 0);
            }
        }
    }
}
