using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] [Range(0, 2)] float delay;
    [SerializeField] [Range(1, 500)] float shootForce;
    [SerializeField] GameObject projectilePrefab;

    void Start()
    {
        StartCoroutine(ShootProjectile());
    }

    void Update()
    {
        
    }

    IEnumerator ShootProjectile()
    {
        while (true)
        {
            Rigidbody2D projectileRigidbody2D = Instantiate(
                projectilePrefab,
                transform.position,
                Quaternion.identity,
                transform
            ).GetComponent<Rigidbody2D>();
            projectileRigidbody2D.AddForce(-transform.right * shootForce);            
            yield return new WaitForSeconds(delay);
        }
    }
}
