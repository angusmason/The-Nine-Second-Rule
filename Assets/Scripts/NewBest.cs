using TMPro;
using UnityEngine;

public class NewBest : MonoBehaviour
{
    ParticleSystem system;
    TextMeshProUGUI text;
    ParticleSystem.EmissionModule module;
    Vector2 random;

    void Start()
    {
        system = GetComponent<ParticleSystem>();
        text = GetComponentInChildren<TextMeshProUGUI>();
        text.gameObject.SetActive(false);
        module = system.emission;
        module.enabled = false;
        random = new Vector2(Random.Range(0, 2 * Mathf.PI), Random.Range(0, 2 * Mathf.PI));
    }

    public void Show()
    {
        text.gameObject.SetActive(true);
        module.enabled = true;
    }

    void Update()
    {
        text.transform.position = new Vector3(
            Mathf.Sin(Time.time * 2 + random.x) * 0.1f,
            Mathf.Sin(Time.time * 2 + random.y) * 0.1f
        ) + transform.position;
    }
}
