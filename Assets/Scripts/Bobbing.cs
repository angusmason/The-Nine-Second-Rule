using UnityEngine;

public class Bobbing : MonoBehaviour
{
    Transform sprite;
    float rotation;
    Vector2 randomOffset;

    void Start()
    {
        sprite = transform.Find("Sprite");
        rotation = Random.Range(-100, 100);
        randomOffset = new(
            Random.Range(-2 * Mathf.PI, 2 * Mathf.PI),
            Random.Range(-2 * Mathf.PI, 2 * Mathf.PI)
        );
    }

    void Update()
    {
        if (gameObject.name == "Finish")
        {
            sprite.Rotate(0, 0, rotation * Time.deltaTime);
        }
        sprite.transform.localPosition = new Vector3(
            Mathf.Sin(Time.time * 2 + randomOffset.x) * 0.5f,
            Mathf.Sin(Time.time * 2 + randomOffset.y) * 0.5f,
            0
        );
    }
}
