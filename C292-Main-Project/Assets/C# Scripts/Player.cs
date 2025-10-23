using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{

    float gravity = -20;
    Vector3 velocity;

    Controller2D controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<Controller2D>();
    }

    private void Update()
    {
        velocity.y += gravity * Time.deltaTime;
    }

    private void FixedUpdate() // Found this fix in a youtube comment under episode 2!: https://www.youtube.com/watch?v=OBtaLCmJexk&lc=UgxoMGzPq1m3koeDzZ94AaABAg
    {
        controller.Move(velocity * Time.deltaTime);
    }
}
