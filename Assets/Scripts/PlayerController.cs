using System;
using System.Collections;
using System.Linq;
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
        bool blockInput = false;
        float originalGravity;

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
            originalGravity = rb.gravityScale;
        }

        void Update()
        {
            // Jumping
            if (isGrounded) extraJumps = extraJumpsValue;

            // Spring jumping
            if (currentSpring != null)
            {
                animator.SetTrigger("takeOff");
                rb.linearVelocity = currentSpring.transform.up * springForce;
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

            if (!isDashing)
            {
                if (!blockInput)
                {
                    rb.linearVelocity = new Vector2(
                        MoveInput.x == 0
                        ? 0
                        : MoveInput.x > 0
                            ? speed
                            : -speed,
                        rb.linearVelocity.y
                    );
                    transform.localScale = new Vector3(
                        MoveInput.x == 0
                            ? transform.localScale.x
                            : MoveInput.x > 0
                                ? PlayerSize
                                : -PlayerSize,
                        PlayerSize,
                        PlayerSize
                    );
                }
                else
                {
                    animator.SetBool("isRunning", false);
                }
            }

            // Wall sliding
            isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, checkWallRadius, whatIsWall);

            wallSliding = isTouchingFront && !isGrounded && MoveInput.x != 0;

            if (wallSliding)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));

            // Wall jumping
            if (wallJumping)
                rb.linearVelocity = new Vector2(xWallForce * -MoveInput.x, yWallForce);

            // Checks if player is on spring
            currentSpring = Physics2D.OverlapCircle(groundCheck.position, checkRadius, spring);


            if (trailRenderer == null)
                return;
            trailRenderer.widthMultiplier = Mathf.Lerp(
                trailRenderer.widthMultiplier,
                isDashing ? 2 : 1,
                Time.deltaTime * 7
            );
            trailRenderer.colorGradient = new Gradient()
            {
                colorKeys = new[] {
                        new GradientColorKey(Color.white, 0),
                        new GradientColorKey(Color.white, 1),
                    },
                alphaKeys = new[] {
                        new GradientAlphaKey(0, 0),
                        new GradientAlphaKey(Mathf.Lerp(
                            trailRenderer.colorGradient.alphaKeys.Where(key => key.alpha > 0).Single().alpha * 255,
                            isDashing ? 255 : 110,
                            Time.deltaTime * 9
                        ) / 255f, 0.25f),
                        new GradientAlphaKey(0, 1),
                    }
            };
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
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
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
            rb.linearVelocity = Vector3.zero;
            countdown.StopCounting();
            countdown.ResetTime();
            trailRenderer.Clear();
            trailRenderer.enabled = true;
            currentSpring = null;
            canDash = true;
            blockInput = true;
        }

        // Collisions
        void OnCollisionEnter2D(Collision2D collision)
        {
            if (whatIsGround == (whatIsGround | (1 << collision.gameObject.layer)))
                isGrounded = true;
            switch (collision.gameObject.tag)
            {
                case "Enemy":
                case "Void":
                    Respawn();
                    break;
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            if (whatIsGround == (whatIsGround | (1 << collision.gameObject.layer)))
                isGrounded = false;
        }

        public void DisableMotion() => rb.simulated = false;
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
                            var levelDatum = LevelSaver.GetLevel(buildIndex - 1);
                            if (
                                countdown.Time.TotalMilliseconds <
                                (
                                    levelDatum != null
                                        ? levelDatum.TimeMilliseconds
                                        : double.MaxValue
                                )
                            )
                            {
                                newBest.Show();
                                newBest.OnDone += (object sender, EventArgs e) =>
                                {
                                    LoadNextScene(buildIndex);
                                };
                            }
                            else
                                LoadNextScene(buildIndex);
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
                // Set the parent of the player to the moving platform, so it moves with it
                transform.parent = collider.transform;
        }

        // Called on the frame that the player exits the trigger
        void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.GetComponent<MovingPlatform>())
                // Clear the parent of the player
                transform.parent = null;
        }

        public void OnMove(InputValue value)
        {
            blockInput = false;
            MoveInput = value.Get<Vector2>();
        }

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
            if (blockInput) return;
            if (!wallSliding)
            {
                if (!isGrounded && extraJumps <= 0)
                    return;
                animator.SetTrigger("takeOff");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
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
            if (canDash && !blockInput)
                StartCoroutine(Dash());
        }
    }
}
