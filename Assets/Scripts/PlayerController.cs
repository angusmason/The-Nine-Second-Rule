using System;
using System.Collections;
using System.Security.Cryptography;
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
        // Dashing variables
        bool canDash = true;
        bool isDashing;
        [SerializeField] float dashingPower = 24f;
        [SerializeField] float dashingTime = 0.2f;
        [SerializeField] float dashTime;

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
        bool flipping;
        Crossfade crossfade;
        TrailRenderer trailRenderer;
        NewBest newBest;

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
            crossfade = FindFirstObjectByType<Crossfade>();
            trailRenderer = GetComponentInChildren<TrailRenderer>();
            newBest = FindFirstObjectByType<NewBest>();
            canDash = true;
        }

        void FixedUpdate()
        {
            if (isDashing)
                return;

            rb.velocity = new Vector2(
                (
                    MoveInput.x == 0
                        ? 0
                        : MoveInput.x > 0
                            ? 1
                            : -1
                ) * speed,
                rb.velocity.y
            );

            transform.localScale = new Vector3(
                (
                    MoveInput.x == 0
                        ? transform.localScale.x
                        : Mathf.Sign(MoveInput.x)
                ) * PlayerSize,
                PlayerSize,
                PlayerSize
            );

            // if (rb.simulated)
                // Checks if the direction which the player sprite is facing should be flipped
                // transform.localScale = new Vector3(Mathf.Sign(MoveInput.x) * PlayerSize, PlayerSize, PlayerSize);

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
            if (isDashing)
                return;

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
            if (oneWayPlatforms.Length > 0)
                if (oneWayPlatforms[0].rotationalOffset == 180 && !flipping)
                    StartCoroutine(FlipEffectors());
            if (MoveInput.y < 0)
                foreach (var platform in oneWayPlatforms)
                {
                    platform.rotationalOffset = 180;
                }
        }

        IEnumerator DisableWallJumping()
        {
            yield return new WaitForSeconds(wallJumpTime);
            wallJumping = false;
        }

        IEnumerator FlipEffectors()
        {
            flipping = true;
            yield return new WaitForSeconds(0.3f);
            foreach (var platform in oneWayPlatforms)
            {
                platform.rotationalOffset = 0;
            }
            flipping = false;
        }

        IEnumerator Dash()
        {
            if (SceneManager.GetActiveScene().buildIndex != 0)
                canDash = false;
            isDashing = true;
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
            yield return new WaitForSeconds(dashingTime);
            rb.gravityScale = originalGravity;
            isDashing = false;
        }

        // Respawning
        void Respawn()
        {
            trailRenderer.enabled = false;
            if (!rb.simulated) return;
            transform.position = Vector3.zero;
            rb.velocity = Vector3.zero;
            countdown.StopCounting();
            countdown.ResetTime();
            trailRenderer.Clear();
            trailRenderer.enabled = true;
            currentSpring = null;
            canDash = true;
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
        void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.gameObject.CompareTag("Finish") && crossfade.FadingState != Crossfade.Fading.FadingIn)
            {
                finished = true;
                DisableMotion();
                countdown.StopCounting();
                countdown.Finished = true;
                int buildIndex = SceneManager.GetActiveScene().buildIndex;
                var vacuum = new GameObject("Vacuum");
                vacuum.transform.position = collider.transform.position;
                transform.parent = vacuum.transform;
                crossfade.FadeIn
                    (
                        () =>
                        {
                            if (LevelSaver.GetLevel(buildIndex - 1) != null
                                && countdown.Time.TotalMilliseconds < LevelSaver
                                    .GetLevel(buildIndex - 1).TimeMilliseconds)
                            {
                                newBest.Show();
                                newBest.OnDone += (object sender, EventArgs e) =>
                                {
                                    LoadNextScene(buildIndex);
                                };
                            }
                            else
                            {
                                LoadNextScene(buildIndex);
                            }
                            LevelSaver.UpdateData(new(buildIndex - 1, countdown.Time));
                        },
                        (alpha) =>
                        {
                            vacuum.transform.localScale = Vector3.one * (1 - alpha);
                            vacuum.transform.localRotation = Quaternion.Euler(0, 0, 360 * alpha);
                            transform.localRotation = Quaternion.Euler(0, 0, 360 * 2 * alpha);
                        }
                    );
            }

            static void LoadNextScene(int buildIndex)
            {
                SceneManager.LoadScene
                    (
                        SceneManager.sceneCountInBuildSettings == buildIndex + 1
                            ? 0
                            : buildIndex + 1
                    );
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
            {
                if (crossfade.FadingState == Crossfade.Fading.NotFading)
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
                }
                return;
            }
            if (crossfade.FadingState == Crossfade.Fading.FadingIn)
                return;
            DisableMotion();
            crossfade.FadeIn(() => SceneManager.LoadScene(0));
        }
        public void OnJump()
        {
            if (!wallSliding)
            {
                if (!isGrounded && extraJumps <= 0)
                    return;
                coyoteTimeCounter = 0f;
                animator.SetTrigger("takeOff");
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                extraJumps--;
            }
            else
            {
                wallJumping = true;
                StartCoroutine(DisableWallJumping());
            }
        }
        public void OnDash()
        {
            if (canDash)
            {
                StartCoroutine(Dash());
            }
        }
    }
}
