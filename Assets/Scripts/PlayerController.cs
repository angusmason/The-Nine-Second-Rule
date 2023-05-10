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

        [SerializeField] float speed;
        [SerializeField] float jumpForce;
        [HideInInspector] public Vector2 MoveInput;
        [SerializeField] float coyoteTime;
        float coyoteTimeCounter;
        // Dashing variables
        bool dashed = false;
        [SerializeField] float dashTime;

        // Finished and at start variables

        [SerializeField] float startDistance;
        Animator animator;
        Rigidbody2D rb;
        int extraJumpsRemaining;
        [SerializeField] int extraJumps;
        bool grounded;
        [SerializeField] Transform groundCheck;
        [SerializeField] float checkRadius;
        [SerializeField] LayerMask whatIsGround;
        [SerializeField] Transform frontCheck;
        bool wallSliding;
        [SerializeField] float wallSlidingSpeed;
        [SerializeField] LayerMask whatIsWall;
        [SerializeField] float checkWallRadius;

        bool wallJumping;
        [SerializeField] float xWallForce;
        [SerializeField] float yWallForce;
        [SerializeField] float wallJumpTime;

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
        ParticleSystem dust;
        ParticleSystem jumpParticles;
        ParticleSystem deathParticles;

        void Start()
        {
            extraJumpsRemaining = extraJumps;
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            countdown = FindFirstObjectByType<Countdown>();
            countdown.TimeUp += (object sender, EventArgs e) => Respawn();
            PlayerSize = transform.localScale.y;
            oneWayPlatforms = FindObjectsByType<PlatformEffector2D>(FindObjectsSortMode.None);
            crossfade = FindFirstObjectByType<Crossfade>();
            trailRenderer = GetComponentInChildren<TrailRenderer>();
            newBest = FindFirstObjectByType<NewBest>();
            dust = transform.Find("Dust").GetComponent<ParticleSystem>();
            jumpParticles = transform.Find("Jump Particles").GetComponent<ParticleSystem>();
            deathParticles = transform.Find("Death Particles").GetComponent<ParticleSystem>();
            dashed = false;
        }

        void FixedUpdate()
        {
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
            if (rb.simulated)
                transform.localScale = new Vector3(Mathf.Sign(MoveInput.x) * PlayerSize, PlayerSize, PlayerSize);
            grounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
            wallSliding = Physics2D.OverlapCircle
                (frontCheck.position, checkWallRadius, whatIsWall)
                && !grounded && MoveInput.x != 0;
            if (wallSliding)
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            if (wallJumping)
                rb.velocity = new Vector2(xWallForce * -MoveInput.x, yWallForce);
            currentSpring = Physics2D.OverlapCircle(groundCheck.position, checkRadius, spring);
        }

        void Update()
        {
            if (grounded || wallSliding) extraJumpsRemaining = extraJumps;
            if (currentSpring != null)
            {
                animator.SetTrigger("takeOff");
                rb.velocity = currentSpring.transform.up * springForce;
                extraJumpsRemaining = -1;
            }
            animator.SetBool("isRunning", MoveInput.x != 0);
            animator.SetBool("isJumping", !grounded);
            animator.SetBool("isWallSliding", wallSliding);

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

            var dustEmission = dust.emission;
            dustEmission.rateOverTime = MoveInput.x != 0 && grounded ? 50 : 0;
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
            yield return new WaitForSeconds(dashTime);
            speed /= 2;
        }

        // Respawning
        void Respawn()
        {
            trailRenderer.enabled = false;
            var newDeathParticles = Instantiate(deathParticles.gameObject, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();
            newDeathParticles.Play();
            Destroy(newDeathParticles.gameObject, newDeathParticles.main.startLifetime.constantMax);
            if (!rb.simulated) return;
            transform.position = Vector3.zero;
            rb.velocity = Vector3.zero;
            countdown.StopCounting();
            countdown.ResetTime();
            trailRenderer.Clear();
            trailRenderer.enabled = true;
            dashed = false;
            currentSpring = null;
        }

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

        void OnTriggerStay2D(Collider2D collider)
        {
            if (collider.GetComponent<MovingPlatform>())
            {
                transform.parent = collider.transform;
            }
        }

        void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.GetComponent<MovingPlatform>())
            {
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
                if (!grounded && extraJumpsRemaining <= 0)
                    return;
                animator.SetTrigger("takeOff");
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                extraJumpsRemaining--;
                jumpParticles.Play();
            }
            else
            {
                wallJumping = true;
                StartCoroutine(DisableWallJumping());
            }
        }
        public void OnDash()
        {
            if (dashed) return;
            speed *= 2;
            if (SceneManager.GetActiveScene().buildIndex != 0)
            {
                dashed = true;
            }
            StartCoroutine(Dash());
        }
    }
}
