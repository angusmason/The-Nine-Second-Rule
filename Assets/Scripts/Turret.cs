using System.Collections;
using UnityEngine;

namespace TNSR
{
    public class Turret : MonoBehaviour
    {
        [SerializeField][Range(1, 500)] float shootForce;
        [SerializeField] GameObject projectilePrefab;
        float lastShotTime;

        void Update()
        {
            if (Time.time - lastShotTime > 1)
            {
                lastShotTime = Time.time;
                Rigidbody2D projectileRigidbody2D = Instantiate(
                    projectilePrefab,
                    transform.position,
                    Quaternion.identity,
                    transform
                ).GetComponent<Rigidbody2D>();
                projectileRigidbody2D.AddForce(-transform.right * shootForce);
            }
        }
    }
}
