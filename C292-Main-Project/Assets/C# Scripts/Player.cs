using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour // This class comes from the same tutorial found in Controller2D.cs, but with updated calculation logic for the jump height by willowaway: https://github.com/whughes7/RaycastPlatformer/blob/03_Jump_Physics_Velocity_Verlet/Assets/Scripts/Player.cs
{

    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxJumpHeight;
    [SerializeField] private float timeToJumpApex;

    Controller2D controller;
    private float jumpForce;
    private float gravity;

    Vector3 velocity;
    Vector3 prevVelocity;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller2D>();

        gravity = -2 * maxJumpHeight / Mathf.Pow(timeToJumpApex, 2);

        jumpForce = 2 * maxJumpHeight / timeToJumpApex;
    }

    // Input dependent variables should be checked here because Update is called more
    // frequently than FixedUpdate
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && controller.collisions.below)
        {
            velocity.y = jumpForce;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        velocity.x = input.x * moveSpeed;
    }

    void FixedUpdate()
    {
        prevVelocity = velocity;

        velocity.y += gravity * Time.fixedDeltaTime;
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
}
