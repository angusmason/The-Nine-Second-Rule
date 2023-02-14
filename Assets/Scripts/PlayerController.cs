using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TNSR
{
    public class PlayerController : MonoBehaviour
    {
        // Basic movement variables
        [SerializeField] float speed;
        [SerializeField] float jumpForce;
        float moveInput;
        [SerializeField] float coyoteTime;
        float coyoteTimeCounter;

        // Finished and at start variables

        [SerializeField] bool finished = false;
        [SerializeField] float r;

        // Animations
        Animator anim;

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
        Collider2D spring;
        public LayerMask whatIsSpring;
        [SerializeField] float springForce;

        Countdown countdown;

        void Start()
        {
            // Jumping
            extraJumps = extraJumpsValue;
            // Gets rigidbody of player
            rb = GetComponent<Rigidbody2D>();
            // Gets animator
            anim = GetComponent<Animator>();
            // Not finished
            finished = false;
            countdown = FindFirstObjectByType<Countdown>();
            countdown.TimeUp += (object sender, EventArgs e) => Respawn();
        }

        void FixedUpdate()
        {
            // Gets the input and moves the player according to that input
            moveInput = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

            // Checks if the direction which the player sprite is facing should be flipped
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);

            // Checks if the player is on the ground
            grounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

            // Coyote time
            if (grounded)
            {
                coyoteTimeCounter = coyoteTime;
            }
            else
            {
                coyoteTimeCounter -= Time.deltaTime;
            }

            if (coyoteTimeCounter > 0f)
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }


            // Wall sliding
            isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, checkWallRadius, whatIsWall);

            if (isTouchingFront && !isGrounded && moveInput != 0)
            {
                wallSliding = true;
            }
            else
            {
                wallSliding = false;
            }

            if (wallSliding)
            {
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            }

            // Wall jumping
            if (wallJumping)
            {
                rb.velocity = new Vector2(xWallForce * -moveInput, yWallForce);
            }

            // Checks if player is on spring
            spring = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsSpring);
        }

        void Update()
        {
            // Jumping
            if (isGrounded) extraJumps = extraJumpsValue;

            if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && extraJumps > 0 && !wallSliding)
            {
                coyoteTimeCounter = 0f;
                anim.SetTrigger("takeOff");
                rb.velocity = Vector2.up * jumpForce;
                extraJumps--;
            }
            else if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && extraJumps == 0 && isGrounded && !wallSliding)
            {
                coyoteTimeCounter = 0f;
                anim.SetTrigger("takeOff");
                rb.velocity = Vector2.up * jumpForce;
                extraJumps--;
            }

            // Wall jumping
            if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && wallSliding)
            {
                wallJumping = true;
                Invoke(nameof(SetWallJumpingToFalse), wallJumpTime);
            }

            // Spring jumping
            if (spring)
            {
                anim.SetTrigger("takeOff");
                rb.velocity = spring.transform.up * springForce;
                extraJumps = -1;
            }
            anim.SetBool("isRunning", moveInput != 0);
            anim.SetBool("isJumping", !isGrounded);
            anim.SetBool("isWallSliding", wallSliding);

            if (wallSliding)
                extraJumps = extraJumpsValue;
            if (Input.GetKeyDown(KeyCode.R))
                Respawn();
            if (!(Vector3.Distance(transform.localPosition, Vector3.zero) < r || finished))
                countdown.StartCounting();
        }

        // Sets wall jumping to false
        void SetWallJumpingToFalse()
        {
            wallJumping = false;
        }

        // Respawning
        void Respawn()
        {
            transform.position = Vector3.zero;
            rb.velocity = Vector3.zero;
            countdown.ResetTime();
            countdown.StopCounting();
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

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Finish"))
            {
                countdown.StopCounting();
                finished = true;

                FindFirstObjectByType<Crossfade>().FadeOut
                    (() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1));
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
    }
}
