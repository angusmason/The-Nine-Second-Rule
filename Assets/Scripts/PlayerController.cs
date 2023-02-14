using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Basic movement variables
    [SerializeField] float speed;
    [SerializeField] float jumpForce;
    float moveInput;
    public GameObject player;
    [SerializeField] float coyoteTime;
    float coyoteTimeCounter;

    // Finished and at start variables

    public bool finished = false;
    [SerializeField] float r;
    bool atStart => Vector3.Distance(transform.position, Vector3.zero) < r;

    // Animations
    Animator anim;
    public Animator transition;
    [SerializeField] float transitionTime;

    // Rigidbody variable
    Rigidbody2D rb;

    // Direction which character sprite is facing variable
    bool facingRight = true;

    // Jumping variables
    int extraJumps;
    [SerializeField] int extraJumpsValue;

    // Ground check variables
    bool grounded;
    bool isGrounded;
    public Transform groundCheck;
    public float checkRadius;
    public LayerMask whatIsGround;

    // Wall sliding variables
    bool isTouchingFront;
    public Transform frontCheck;
    bool wallSliding;
    [SerializeField] float wallSlidingSpeed;
    public LayerMask whatIsWall;
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
    }

    void FixedUpdate()
    {
        // Gets the input and moves the player according to that input
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        // Checks if the direction which the player sprite is facing should be flipped
        if (facingRight == false && moveInput > 0)
        {
            Flip();
        }
        else if (facingRight == true && moveInput < 0)
        {
            Flip();
        }

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
        if (isGrounded)
        {
            extraJumps = extraJumpsValue;
        }

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

        // Animations
        if (moveInput == 0)
        {
            anim.SetBool("isRunning", false);
        }
        else
        {
            anim.SetBool("isRunning", true);
        }

        if (isGrounded)
        {
            anim.SetBool("isJumping", false);
            // anim.ResetTrigger("takeOff");
        }
        else
        {
            anim.SetBool("isJumping", true);
        }

        if (wallSliding)
        {
            anim.SetBool("isWallSliding", true);
            extraJumps = extraJumpsValue;
        }
        else
        {
            anim.SetBool("isWallSliding", false);
        }

        // Respawning
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }

        // Countdown
        // if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        if (!atStart && !Countdown.counting && !finished)
        {
            Countdown.counting = true;
        }
        if (Countdown.timeLeft == 0)
        {
            Respawn();
        }
    }

    // Sets wall jumping to false
    void SetWallJumpingToFalse()
    {
        wallJumping = false;
    }

    // Flips direction which player sprite is facing
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    // Respawning
    void Respawn()
    {
        player.transform.position = new Vector2(0, 0);
        rb.velocity = Vector3.zero;
        Countdown.counting = false;
        Countdown.timeLeft = Countdown.initialTime;
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
        if (collision.gameObject.tag == "Finish")
        {
            Countdown.counting = false;
            finished = true;
            StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
        }
    }

    // Level loader transition
    IEnumerator LoadLevel(int levelIndex)
    {
        // Play animation
        transition.SetTrigger("Start");

        // Wait
        yield return new WaitForSeconds(transitionTime);

        // Load scene
        SceneManager.LoadScene(levelIndex);
    }

    // Called on every frame that the player is on the moving platform
    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.TryGetComponent<MovingPlatform>(out MovingPlatform movingPlatform))
        {
            // Set the parent of the player to the moving platform, so it moves with it
            transform.parent = collider.transform;
        }
    }

    // Called on the frame that the player exits the trigger
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.TryGetComponent<MovingPlatform>(out MovingPlatform movingPlatform))
        {
            // Clear the parent of the player
            transform.parent = null;
        }
    }
}
