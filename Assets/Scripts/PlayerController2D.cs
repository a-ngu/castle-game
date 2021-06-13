using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movement Settings")]
    // Horizontal Movement
    [SerializeField] private float movementSpeed = 6f;
    [SerializeField] private float smoothTimeAirborne = 0.2f;
    [SerializeField] private float smoothTimeGrounded = 0.1f;
    private float currSmoothVel;
    // Vertical Movement
    [SerializeField] private float maxHeight = 4f;
    [SerializeField] private float timeToMaxHeight = .4f;
    private float gravity;
    private float initJumpVelocity;

    // State Tracking
    private Vector2 input;
    private bool jumpPressed;
    private Vector3 prevVelocity, velocity;

    // Component Tracking
    private Controller2D controller;

    private void Start() {
        controller = GetComponent<Controller2D>();
        gravity = -2 * maxHeight / Mathf.Pow(timeToMaxHeight, 2);
        initJumpVelocity = 2 * maxHeight / timeToMaxHeight;
    }

    // Check for user imput
    private void Update() {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetKey(KeyCode.Space) && controller.collisions.below) {
            jumpPressed = true;
        }
    }

    // Process physical changes
    private void FixedUpdate() {
        // Determine horizontal velocity
        float targetVelX = input.x * movementSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelX, ref currSmoothVel, 
            (controller.collisions.below) ? smoothTimeGrounded : smoothTimeAirborne);

        // Determine vertical velocity
        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }
        if (jumpPressed) {
            jumpPressed = false;
            velocity.y = initJumpVelocity;
        }
        velocity.y += gravity * Time.fixedDeltaTime;

        // Using verlet integration for reduced error
        Vector3 deltaPos = (velocity + prevVelocity) * Time.fixedDeltaTime * .5f;
        controller.Move(deltaPos);

        // Track current velocity
        prevVelocity.x = velocity.x;
        prevVelocity.y = velocity.y;
    }
}