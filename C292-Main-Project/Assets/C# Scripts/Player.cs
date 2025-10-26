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

    Controller2D controller;
    GlobalData globalData;
    private float jumpForce;
    private float gravity;
    private int maxDashCount;

    private Vector3 velocity;
    private Vector3 prevVelocity;
    [SerializeField] private bool canInput;
    [SerializeField] private int dashCount;
    [SerializeField] private bool isDashing;
    


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
        canInput = true;
        dashCount = maxDashCount;
        isDashing = false;
    }

    // Input dependent variables should be checked here because Update is called more
    // frequently than FixedUpdate (reminder from willowaway)
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Z)) && controller.collisions.below && canInput) // Jump logic from SL tutorial
        {
            velocity.y = jumpForce;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")); // SL tutorial
        
        if ((Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.V)) && dashCount > 0)
        {
            StopCoroutine(Dash());
            canInput = false;
            dashCount -= 1;

            velocity = new Vector3(input.x, input.y, 0).normalized * dashSpeed;
            StartCoroutine(Dash()); // Learned how to use Coroutines from ChatGPT: https://chatgpt.com/share/68fabe04-1c64-8002-bfe6-50316ef5527d 

        }

        if (canInput)
        {
            velocity.x = input.x * moveSpeed; // SL tutorial
        }
    }

    void FixedUpdate() // Comes from willowaway
    {
        prevVelocity = velocity;
        if (canInput)
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }
        Vector3 deltaPosition = (prevVelocity + velocity) * 0.5f * Time.fixedDeltaTime;
        controller.Move(deltaPosition);

        if (controller.collisions.below || controller.collisions.above)
        {
            velocity.y = 0;
            Debug.Log("Grounded!");
            if (isDashing)
            {
                Debug.Log("Stopped!");
                StopCoroutine(Dash());
                isDashing = false;
                canInput = true;
            }
        }

        if (controller.collisions.left || controller.collisions.right)
        {
            velocity.x = 0;
            if (isDashing)
            {
                StopCoroutine(Dash());
                isDashing = false;
                canInput = true;
            }
        }

        if (dashCount <= 0)
        {
            if (controller.collisions.below) { dashCount = maxDashCount; }
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;

        if (velocity.y == dashSpeed || velocity.y == (-1 * dashSpeed))
        {
            yield return new WaitForSeconds(verticalDashDuration);
            canInput = true;
        }
        else if (velocity.x == dashSpeed || velocity.x == (-1 * dashSpeed))
        {
            yield return new WaitForSeconds(horizontalDashDuration);
            canInput = true;
        }
        else
        {
            yield return new WaitForSeconds(diagonalDashDuration);
            float tempXVelocity = velocity.x;
            canInput = true;
            while (velocity.y > 0.5)
            {
                velocity.x = tempXVelocity;
                yield return null;
            }

        }

        isDashing = false;
    }
}
