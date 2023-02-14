using System.Collections.Generic;
using UnityEngine;

namespace TNSR
{
    [RequireComponent(typeof(Camera))]
    public class MultipleTargetCamera : MonoBehaviour
    {
        public List<Transform> targets;

        public Vector3 offset;
        public float smoothTime = .2f;

        public float minZoom = 200f;
        public float maxZoom = 100f;
        public float zoomLimiter = 0f;

        private Vector3 velocity;
        private Camera cam;

        void Start()
        {
            cam = GetComponent<Camera>();
        }

        void LateUpdate()
        {
            if (targets.Count == 0)
            {
                return;
            }

            Move();
            Zoom();

        }

        void Zoom()
        {
            float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreatestDistance() / zoomLimiter);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.unscaledDeltaTime);
        }

        void Move()
        {
            Vector3 centerPoint = GetCenterPoint();

            Vector3 newPosition = centerPoint + offset;

            transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime * Time.timeScale);
        }

        float GetGreatestDistance()
        {
            var bounds = new Bounds(targets[0].position, Vector3.zero);
            for (int i = 0; i < targets.Count; i++)
            {
                bounds.Encapsulate(targets[i].position);
            }

            return bounds.size.x;
        }

        Vector3 GetCenterPoint()
        {
            if (targets.Count == 1)
            {
                return targets[0].position;
            }

            var bounds = new Bounds(targets[0].position, Vector3.zero);
            for (int i = 0; i < targets.Count; i++)
            {
                bounds.Encapsulate(targets[i].position);
            }

            return bounds.center;
        }
    }
}
