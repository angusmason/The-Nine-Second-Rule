using UnityEngine;
using UnityEngine.AddressableAssets;

public class LightController : MonoBehaviour
{
    DistanceJoint2D distanceJoint;
    LineRenderer lineRenderer;

    void Start()
    {
        distanceJoint = gameObject.AddComponent<DistanceJoint2D>();
        distanceJoint.anchor = Vector2.up * 1.8f;
        distanceJoint.connectedAnchor = transform.position + Vector3.up * 4;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = lineRenderer.endWidth = 0.04f;
        lineRenderer.startColor = lineRenderer.endColor = Color.white;
        lineRenderer.numCapVertices = 5;
        Addressables.LoadAssetAsync<Material>("Sprite-Lit-Default").Completed += (material) => {
            lineRenderer.material = material.Result;
        };
    }
    void Update()
    {
        lineRenderer.SetPositions(new Vector3[] {
            transform.TransformPoint(distanceJoint.anchor),
            distanceJoint.connectedAnchor
        });
    }
}
