using System;
using System.Collections;
using TNSR.Levels;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace TNSR
{
    public class PlayerController : MonoBehaviour
    {

        // Basic movement variables
        [SerializeField] float speed;
        [SerializeField] float jumpForce;
        [HideInInspector] public Vector2 MoveInput;
        [SerializeField] float coyoteTime;
        float coyoteTimeCounter;

        // Finished and at start variables

        [SerializeField] float startDistance;

        // Animations
        Animator animator;

        // Rigidbody variable
        Rigidbody2D rb;

        // Jumping variables
        int extraJumps;
        [SerializeField] int extraJumpsValue;

        // Ground check variables
        bool grounded;
        bool isGrounded;
        [SerializeField] Transform groundCheck;
        [SerializeField] float checkRadius;
        [SerializeField] LayerMask whatIsGround;

        // Wall sliding variables
        bool isTouchingFront;
        [SerializeField] Transform frontCheck;
        bool wallSliding;
        [SerializeField] float wallSlidingSpeed;
        [SerializeField] LayerMask whatIsWall;
        [SerializeField] float checkWallRadius;

        // Wall jumping variables
        bool wallJumping;
        [SerializeField] float xWallForce;
        [SerializeField] float yWallForce;
        [SerializeField] float wallJumpTime;

        // Spring variables
        Collider2D currentSpring;
        public LayerMask spring;
        [SerializeField] float springForce;
        Countdown countdown;
        bool finished = false;
        [HideInInspector] public float PlayerSize;
        PlatformEffector2D[] oneWayPlatforms;

        void Start()
        {
            // Jumping
            extraJumps = extraJumpsValue;
            // Gets rigidbody of player
            rb = GetComponent<Rigidbody2D>();
            // Gets animator
            animator = GetComponent<Animator>();
            countdown = FindFirstObjectByType<Countdown>();
            countdown.TimeUp += (object sender, EventArgs e) => Respawn();
            PlayerSize = transform.localScale.y;
            oneWayPlatforms = FindObjectsByType<PlatformEffector2D>(FindObjectsSortMode.None);
        }

        void FixedUpdate()
        {
            rb.velocity = new Vector2(MoveInput.x * speed, rb.velocity.y);
            if (rb.simulated)
                // Checks if the direction which the player sprite is facing should be flipped
                transform.localScale = new Vector3(Mathf.Sign(MoveInput.x) * PlayerSize, PlayerSize, PlayerSize);
            grounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
            // Coyote time
            coyoteTimeCounter = grounded ? coyoteTime : coyoteTimeCounter - Time.deltaTime;
            isGrounded = coyoteTimeCounter > 0f;
            // Wall sliding
            isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, checkWallRadius, whatIsWall);

            wallSliding = isTouchingFront && !isGrounded && MoveInput.x != 0;

            if (wallSliding)
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));

            // Wall jumping
            if (wallJumping)
                rb.velocity = new Vector2(xWallForce * -MoveInput.x, yWallForce);

            // Checks if player is on spring
            currentSpring = Physics2D.OverlapCircle(groundCheck.position, checkRadius, spring);
        }

        void Update()
        {
            // Jumping
            if (isGrounded) extraJumps = extraJumpsValue;

            // Spring jumping
            if (currentSpring != null)
            {
                animator.SetTrigger("takeOff");
                rb.velocity = currentSpring.transform.up * springForce;
                extraJumps = -1;
            }
            animator.SetBool("isRunning", MoveInput.x != 0);
            animator.SetBool("isJumping", !isGrounded);
            animator.SetBool("isWallSliding", wallSliding);

            if (wallSliding)
                extraJumps = extraJumpsValue;
            if (!(Vector3.Distance(transform.localPosition, Vector3.zero) < startDistance) && !finished)
                countdown.StartCounting();

            foreach (var platform in oneWayPlatforms)
            {
                platform.rotationalOffset = MoveInput.y < 0 ? 180 : 0;
            }
        }

        // Sets wall jumping to false
        IEnumerator DisableWallJumping()
        {
            yield return new WaitForSeconds(wallJumpTime);
            wallJumping = false;
        }

        // Respawning
        void Respawn()
        {
            if (!rb.simulated) return;
            transform.position = Vector3.zero;
            rb.velocity = Vector3.zero;
            countdown.StopCounting();
            countdown.ResetTime();
        }

        // Collisions
        void OnCollisionEnter2D(Collision2D collision)
        {
            switch (collision.gameObject.tag)
            {
                case "Enemy":
                case "Void":
                    Respawn();
                    break;
            }
        }

        public void DisableMotion()
        {
            rb.simulated = false;
        }
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Finish"))
            {
                finished = true;
                DisableMotion();
                countdown.StopCounting();
                FindFirstObjectByType<Crossfade>().FadeIn
                    (
                        () => SceneManager.LoadScene
                            (SceneManager.GetActiveScene().buildIndex + 1)
                    );
                LevelSaver.UpdateData(new(SceneManager.GetActiveScene().buildIndex - 1, countdown.Time));
            }
        }

        // Called on every frame that the player is on the moving platform
        void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.GetComponent<MovingPlatform>())
            {
                // Set the parent of the player to the moving platform, so it moves with it
                transform.parent = collider.transform;
            }
        }

        // Called on the frame that the player exits the trigger
        void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.GetComponent<MovingPlatform>())
            {
                // Clear the parent of the player
                transform.parent = null;
            }
        }

        public void OnMove(InputValue value)
            => MoveInput = value.Get<Vector2>();
        public void OnReset()
            => Respawn();
        public void OnEscape()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
                return;
            SceneManager.LoadScene(0);
        }
        public void OnJump()
        {
            if (!wallSliding)
            {
                if (isGrounded || extraJumps > 0)
                {
                    coyoteTimeCounter = 0f;
                    animator.SetTrigger("takeOff");
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    extraJumps--;
                }
            }
            else
            {
                wallJumping = true;
                StartCoroutine(DisableWallJumping());
            }
        }
    }
}
