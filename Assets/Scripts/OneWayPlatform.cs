using UnityEngine;

namespace TNSR
{
    public class OneWayPlatform : MonoBehaviour
    {
        // Ground check variables
        private bool isGrounded;
        public Transform groundCheck;
        public float checkRadius;
        public LayerMask whatIsGround;

        // One way platform variables
        private PlatformEffector2D effector;
        private float waitTimeCounter;
        public float waitTime = 0.1f;

        void Start()
        {
            effector = GetComponent<PlatformEffector2D>();
        }

        void FixedUpdate()
        {
            // Ground check
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        }

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
            {
                waitTimeCounter = waitTime;
            }

            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                if (waitTimeCounter <= 0)
                {
                    effector.rotationalOffset = 180f;
                    waitTimeCounter = waitTime;
                }
                else
                {
                    waitTimeCounter -= Time.deltaTime;
                }
            }

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                effector.rotationalOffset = 0f;
            }

            if (!isGrounded)
            {
                effector.rotationalOffset = 0f;
            }
        }
    }
}
