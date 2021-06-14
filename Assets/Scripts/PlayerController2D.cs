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
    // Jumping
    [Range(0, 1)] [SerializeField] private float jumpReleaseDamping = 0.5f;
    
    [Header("Input Settings")]
    [SerializeField] private float statePersistance = .2f;
    // State Tracking
    private Vector2 input;
    private float jumpTimer, groundedTimer;
    private bool jumping, canDoubleJump, cutJump;
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
        if (Input.GetKeyDown(KeyCode.Space)) {
            jumpTimer = statePersistance;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            cutJump = true;
        }
        if (controller.collisions.below) {
            groundedTimer = statePersistance;
        }
        if (jumpTimer > 0 && groundedTimer > 0) {
            jumping = canDoubleJump = true;
            jumpTimer = -1;
            groundedTimer = -1;
        } 
        
        if (jumpTimer > 0 && groundedTimer <= 0 && canDoubleJump)
        {
            jumping = true;
            canDoubleJump = false;
            jumpTimer = -1;
        }

        if (jumpTimer > 0)
        {
            jumpTimer -= Time.deltaTime;
        }
        if (groundedTimer > 0)
        {
            groundedTimer -= Time.deltaTime;
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
        if (jumping) {
            jumping = false;
            Debug.Log(canDoubleJump);
            velocity.y = initJumpVelocity;
        }
        if (cutJump)
        {
            cutJump = false;
            if (velocity.y > 0)
            {
                velocity.y *= jumpReleaseDamping;     // Maybe serialize later
            }
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