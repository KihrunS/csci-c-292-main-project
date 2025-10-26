using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour // Many parts of this class comes from the same tutorial found in Controller2D.cs (denoted as SL tutorial from now on), but with updated calculation logic for the jump height by willowaway: https://github.com/whughes7/RaycastPlatformer/blob/03_Jump_Physics_Velocity_Verlet/Assets/Scripts/Player.cs
{

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

    Controller2D controller;
    GlobalData globalData;
    private float jumpForce;
    private float gravity;
    private int maxDashCount;
    private bool wallSliding;

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



    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller2D>();

        globalData = FindObjectOfType<GlobalData>();

        gravity = -2 * maxJumpHeight / Mathf.Pow(timeToJumpApex, 2);

        jumpForce = 2 * maxJumpHeight / timeToJumpApex; // Gravity and jump force calculations come from SL tutorial

        horizontalDashDuration = dashDistance / dashSpeed;
        verticalDashDuration = horizontalDashDuration / 2;
        diagonalDashDuration = horizontalDashDuration * (float)0.75;
        maxDashCount = globalData.MaxDashCount;
        canMove = true;
        canJump = true;
        dashCount = maxDashCount;
        isDashing = false;
        groundedDashJump = false;
        gravityOn = true;
        wallSliding = false;
    }

    // Input dependent variables should be checked here because Update is called more
    // frequently than FixedUpdate (reminder from willowaway)
    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); // SL tutorial

        if ((Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Z)) && canJump) // Jump logic from SL tutorial
        {
            if (wallSliding)
            {
                canMove = false;
                isWallJumping = true;

                velocity.x = -wallDirX * moveSpeed;
                velocity.y = jumpForce;

                StartCoroutine("WallJump");
            }
            if (controller.collisions.below)
            {
                velocity.y = jumpForce;
            }
        }
        
        if ((Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.V)) && dashCount > 0)
        {
            StopCoroutine("Dash");
            canJump = canMove = gravityOn = false;
            dashCount -= 1;

            velocity = new Vector3(input.x, input.y, 0).normalized * dashSpeed;

            StartCoroutine("Dash"); // Learned how to use Coroutines from ChatGPT: https://chatgpt.com/share/68fabe04-1c64-8002-bfe6-50316ef5527d 
            // Learned more proper Coroutine syntax from https://discussions.unity.com/t/coroutine-keep-working-after-stopcoroutine-call/257280
        }

        if (canMove)
        {
            velocity.x = input.x * moveSpeed; // SL tutorial
        }
    }

    void FixedUpdate() // Comes from willowaway
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y != 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

        }

        prevVelocity = velocity;
        if (gravityOn)
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }
        Vector3 deltaPosition = (prevVelocity + velocity) * 0.5f * Time.fixedDeltaTime;
        controller.Move(deltaPosition);

        if (controller.collisions.below || controller.collisions.above)
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

        if (controller.collisions.left || controller.collisions.right)
        {
            velocity.x = 0;
            if (isDashing)
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
            yield return new WaitForSeconds(diagonalDashDuration);
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
        yield return new WaitUntil(() => velocity.y <= 0);
        canMove = true;
        isWallJumping = false;
    }
}
