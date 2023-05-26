using System;
using UnityEngine;
namespace TNSR
{
    public class Key : MonoBehaviour
    {
        Transform player;
        [SerializeField] float collectDistance;
        public bool collected;
        Vector3 originalPosition;

        void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            player.GetComponent<PlayerController>().Respawned +=
                (object sender, EventArgs e) => collected = false;
            originalPosition = transform.position;
        }

        void Update()
        {
            transform.position = Vector3.Lerp(
                transform.position,
                collected
                    ? player.position + Vector3.up * 1
                    : originalPosition,
                Time.deltaTime * (collected ? 5 : 2)
            );
            if (Vector3.Distance(transform.position, player.position) < collectDistance)
                collected = true;
        }
    }
}
