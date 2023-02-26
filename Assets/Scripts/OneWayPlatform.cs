using UnityEngine;

namespace TNSR
{
    public class OneWayPlatform : MonoBehaviour
    {
        // Ground check variables
        bool isGrounded;
        [SerializeField] Transform groundCheck;
        [SerializeField] float checkRadius;
        [SerializeField] LayerMask whatIsGround;

        // One way platform variables
        PlatformEffector2D effector;
        float waitTimeCounter;
        [SerializeField] float waitTime = 0.1f;

        PlayerController playerController;

        void Start()
        {
            effector = GetComponent<PlatformEffector2D>();
            playerController = FindFirstObjectByType<PlayerController>();
        }

        void FixedUpdate()
        {
            // Ground check
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        }

        void Update()
        {
            if (playerController.MoveInput.y >= 0)
            {
                waitTimeCounter = waitTime;
            }
            else
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

            if (!(playerController.MoveInput.y > 0))
                return;
            if (!isGrounded)
                return;
            effector.rotationalOffset = 0f;
        }
    }
}
