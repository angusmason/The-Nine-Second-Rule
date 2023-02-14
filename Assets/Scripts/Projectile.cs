using UnityEngine;

namespace TNSR
{
    public class Projectile : MonoBehaviour
    {
        float timer;
        [SerializeField] float timerValue;

        void Start()
        {
            timer = timerValue;
        }

        void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                Destroy(gameObject);
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            Destroy(gameObject);
        }
    }
}
