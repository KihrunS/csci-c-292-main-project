using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour // Many parts of this class comes from the same tutorial found in Controller2D.cs (denoted as SL tutorial from now on), but with updated calculation logic for the jump height by willowaway: https://github.com/whughes7/RaycastPlatformer/blob/03_Jump_Physics_Velocity_Verlet/Assets/Scripts/Player.cs
{

    // SerializeField variables
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxJumpHeight;
    [SerializeField] private float timeToJumpApex;
    [SerializeField] private float dashDistance;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float horizontalDashDuration;
    [SerializeField] private float verticalDashDuration;
    [SerializeField] private float diagonalDashDuration;
    [SerializeField] private float wallSlideSpeedMax;
    [SerializeField] private Vector2 wallJumpClimb;
    [SerializeField] private float wallStickTime;
    [SerializeField] private float jumpGraceTime;
    [SerializeField] private float coyoteTime;

    // Variables set/calculated on start only
    Controller2D controller;
    SpriteRenderer sprite;
    GameManager gameManager;
    private float jumpForce;
    private float gravity;
    private int maxDashCount;

    // Variables updated during gameplay
    private Vector3 velocity;
    private Vector3 prevVelocity;
    [SerializeField] private bool canMove;
    [SerializeField] private bool canJump;
    [SerializeField] private int dashCount;
    [SerializeField] private bool isDashing;
    private bool groundedDashJump;
    private bool gravityOn;
    [SerializeField] private bool isWallJumping;
    private int wallDirX;
    private float timeToWallUnstick;
    [SerializeField] private float timeToJumpAttemptEnd;
    private Vector2 input;
    [SerializeField] private bool coyoteJump;
    [SerializeField] private float timeToCoyoteEnd;
    [SerializeField] private bool isGrounded;
    [SerializeField] private int dirFacing;
    [SerializeField] private bool wallSliding;

    // Debug variables
    [SerializeField] private float timer = 0;
    [SerializeField] private bool timerStart; 



    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller2D>();

        sprite = GetComponentInChildren<SpriteRenderer>();

        gameManager = FindObjectOfType<GameManager>();

        gravity = -2 * maxJumpHeight / Mathf.Pow(timeToJumpApex, 2);

        jumpForce = 2 * maxJumpHeight / timeToJumpApex; // Gravity and jump force calculations come from SL tutorial

        horizontalDashDuration = dashDistance / dashSpeed;
        verticalDashDuration = horizontalDashDuration / 2;
        diagonalDashDuration = horizontalDashDuration * (float)0.75;
        maxDashCount = gameManager.MaxDashCount;
        canMove = true;
        canJump = true;
        dashCount = maxDashCount;
        isDashing = false;
        groundedDashJump = false;
        gravityOn = true;
        wallSliding = false;
        coyoteJump = false;
        timeToCoyoteEnd = 0;
        isGrounded = true;
        dirFacing = 1;
    }

    // Input dependent variables should be checked here because Update is called more
    // frequently than FixedUpdate (reminder from willowaway)
    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); // SL tutorial

        if (canMove)
        {
            velocity.x = input.x * moveSpeed; // SL tutorial
        }

        if ((Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Z)) && canJump) // Jump logic started from SL tutorial, eventually fully reworked by me
        {
            timeToJumpAttemptEnd = jumpGraceTime;

            StopCoroutine("AttemptJump");
            StopCoroutine("JumpAttemptTimer");
            StartCoroutine("AttemptJump");
            StartCoroutine("JumpAttemptTimer");
        }
        
        if ((Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.V)) && dashCount > 0)
        {
            StopCoroutine("Dash");
            StopCoroutine("WallJump"); // Allows you to interrupt walljump by dashing
            canJump = canMove = gravityOn = isWallJumping = false;
            dashCount -= 1;

            if (input.x == 0 && input.y == 0) // This makes sure you dash in the direction you're facing if you don't input anything while pressing dash
            {
                input.x = dirFacing;
            }
            velocity = new Vector3(input.x, input.y, 0).normalized * dashSpeed;

            StartCoroutine("Dash"); // Learned how to use Coroutines from ChatGPT: https://chatgpt.com/share/68fabe04-1c64-8002-bfe6-50316ef5527d 
            // Learned more proper Coroutine syntax from https://discussions.unity.com/t/coroutine-keep-working-after-stopcoroutine-call/257280
        }

        // debug
        if (!timerStart && !controller.collisions.left && !controller.collisions.right)
        {
            timerStart = true;
            StartCoroutine("DebugTimer");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StopCoroutine("DebugTimer");
            timerStart = false;
            timer = 0;
        }
        
        if (Input.GetKeyDown(KeyCode.D))
        {
            Death();
        }
    }

    void FixedUpdate() // Comes from willowaway, walljump/slide logic comes from SL
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y != 0)
        {
            wallSliding = true;

            if (input.x == wallDirX)
            {
                dirFacing = wallDirX;
                
                if (velocity.y < -wallSlideSpeedMax)
                {
                    velocity.y = -wallSlideSpeedMax;
                }
            }

            if (timeToWallUnstick > 0 && !isWallJumping && !isDashing)
            {
                velocity.x = 0;

                if (input.x != wallDirX && input.x != 0)
                {
                    timeToWallUnstick -= Time.fixedDeltaTime;
                    dirFacing = -wallDirX; // Allows you to change directions without unsticking
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }

        prevVelocity = velocity;
        if (gravityOn)
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }
        Vector3 deltaPosition = (prevVelocity + velocity) * 0.5f * Time.fixedDeltaTime;
        controller.Move(deltaPosition);
        
        if (velocity.x != 0)
        {
            dirFacing = (velocity.x > 0) ? 1 : -1; // Changes the direciton you're facing based on your velocity, but only if you're moving
        }

        if (controller.collisions.below)
        {
                velocity.y = 0;
            if (isDashing && !groundedDashJump)
            {
                StopCoroutine("Dash");
                StopCoroutine("GroundedDash");
                isDashing = groundedDashJump = false;
                canMove = canJump = gravityOn = true;
            }
        }

        if (controller.collisions.above)
        {
            if (!isDashing)
            {
                velocity.y = 0;
            }
        }

        if (controller.collisions.left || controller.collisions.right)
        {
            velocity.x = 0;
            coyoteJump = false;

            if (isDashing && !(velocity.y == dashSpeed || velocity.y == (-1 * dashSpeed)))
            {
                StopCoroutine("Dash");
                StopCoroutine("GroundedDash");
                isDashing = groundedDashJump = false;
                canMove = canJump = gravityOn = true;
            }

            if (isWallJumping)
            {
                StopCoroutine("WallJump");
                isWallJumping = false;
                canMove = true;
            }
        }

        if (dashCount <= 0 && !groundedDashJump)
        {
            if (controller.collisions.below) 
            { 
                dashCount = maxDashCount;
            }
        }

        if (!controller.collisions.below)
        {
            if (isGrounded)
            {
                timeToCoyoteEnd = coyoteTime;
                coyoteJump = true;
            }

            isGrounded = false;

            if (timeToCoyoteEnd > 0)
            {
                timeToCoyoteEnd -= Time.fixedDeltaTime;
            }
            else
            {
                coyoteJump = false;
            }
        }
        else
        {
            isGrounded = true;
            coyoteJump = false;
            timeToCoyoteEnd = 0;
        }
    }

    public void Death()
    {
        Destroy(gameObject);
        gameManager.SpawnPlayer();
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator AttemptJump()
    {
        yield return new WaitUntil(() => controller.collisions.below || wallSliding || coyoteJump || timeToJumpAttemptEnd <= 0);
        
        if (controller.collisions.below || coyoteJump)
        {
            velocity.y = jumpForce;
            coyoteJump = false;
            isGrounded = false;
        }
        else if (wallSliding)
        {
            canMove = false;
            isWallJumping = true;

            velocity.x = -wallDirX * moveSpeed;
            velocity.y = jumpForce;

            StartCoroutine("WallJump");
        }
    }

    IEnumerator JumpAttemptTimer()
    {
        while (timeToJumpAttemptEnd > 0)
        {
            timeToJumpAttemptEnd -= Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;

        if (velocity.y == dashSpeed || velocity.y == (-1 * dashSpeed))
        {
            yield return new WaitForSeconds(verticalDashDuration);
            canMove = canJump = gravityOn = true;
        }
        else if (velocity.x == dashSpeed || velocity.x == (-1 * dashSpeed))
        {
            if (controller.collisions.below) { StartCoroutine("GroundedDash");  }

            yield return new WaitForSeconds(horizontalDashDuration/2);
            gravityOn = true;
            yield return new WaitForSeconds(horizontalDashDuration/2);
            canJump = canMove = true;
        }
        else
        {
            yield return new WaitForSeconds(diagonalDashDuration/2);
            
            if (!controller.collisions.above)
            {
                yield return new WaitForSeconds(diagonalDashDuration / 2);
            }

            float tempXVelocity = velocity.x;
            canMove = canJump = gravityOn = true;
            while (velocity.y > 0.5)
            {
                velocity.x = tempXVelocity;
                yield return null;
            }

        }

        isDashing = groundedDashJump = false;
    }

    IEnumerator GroundedDash()
    {
        yield return new WaitForSeconds(horizontalDashDuration/4);
        canJump = gravityOn = groundedDashJump = true;
        yield return new WaitForSeconds(horizontalDashDuration/4);
        dashCount = maxDashCount;
    }

    IEnumerator WallJump()
    {
        yield return new WaitForSeconds(timeToJumpApex * (float)1.01);
        canMove = true;
        isWallJumping = false;
    }

    /* debug
    IEnumerator DebugTimer()
    {
        while (timerStart)
        {
            timer += Time.deltaTime;
            if (controller.collisions.right)
            {
                timerStart = false;
            }
            yield return null;
        }

    } */
}
