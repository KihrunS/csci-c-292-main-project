using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour // This class comes from the same tutorial found in Controller2D.cs, but with updated calculation logic for the jump height by willowaway: https://github.com/whughes7/RaycastPlatformer/blob/03_Jump_Physics_Velocity_Verlet/Assets/Scripts/Player.cs
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
    private float jumpForce;
    private float gravity;

    Vector3 velocity;
    Vector3 prevVelocity;
    [SerializeField] private bool canInput;
    [SerializeField] private bool canDash;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller2D>();

        gravity = -2 * maxJumpHeight / Mathf.Pow(timeToJumpApex, 2);

        jumpForce = 2 * maxJumpHeight / timeToJumpApex;

        horizontalDashDuration = dashDistance / dashSpeed;
        verticalDashDuration = horizontalDashDuration / 2;
        diagonalDashDuration = horizontalDashDuration * (float)0.75;

        canInput = true;

        canDash = true;
    }

    // Input dependent variables should be checked here because Update is called more
    // frequently than FixedUpdate
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && controller.collisions.below && canInput)
        {
            velocity.y = jumpForce;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
        if (Input.GetKeyDown(KeyCode.X) && canInput && canDash)
        {
            canDash = canInput = false;

            velocity = new Vector3(input.x, input.y, 0).normalized * dashSpeed;
            StartCoroutine(Dash());
            
        }

        if (canInput)
        {
            velocity.x = input.x * moveSpeed;
        }
    }

    void FixedUpdate()
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
        }

        if (controller.collisions.left || controller.collisions.right)
        {
            velocity.x = 0;
        }
    }

    IEnumerator Dash()
    {
        if (velocity.y == dashSpeed || velocity.y == (-1 * dashSpeed))
        {
            yield return new WaitForSeconds(verticalDashDuration);
            canDash = canInput = true;
        }
        else if (velocity.x == dashSpeed || velocity.x == (-1 * dashSpeed))
        {
            yield return new WaitForSeconds(horizontalDashDuration);
            canDash = canInput = true;
        }
        else
        {
            yield return new WaitForSeconds(diagonalDashDuration);
            canDash = canInput = true;

        }
    }
}
