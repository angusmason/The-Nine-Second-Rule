using UnityEngine;
using UnityEngine.AddressableAssets;

public class LightController : MonoBehaviour
{
    DistanceJoint2D distanceJoint;
    LineRenderer lineRenderer;

    public Material material;

    void Start()
    {
        distanceJoint = gameObject.AddComponent<DistanceJoint2D>();
        distanceJoint.anchor = Vector2.up * 1.8f;
        distanceJoint.connectedAnchor = transform.position + Vector3.up * 2;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = lineRenderer.endWidth = 0.14f;
        lineRenderer.startColor = lineRenderer.endColor = Color.white;
        lineRenderer.numCapVertices = 5;
        lineRenderer.material = material;
    }
    void Update()
    {
        lineRenderer.SetPositions(new Vector3[] {
            transform.TransformPoint(distanceJoint.anchor),
            distanceJoint.connectedAnchor
        });
    }
}
