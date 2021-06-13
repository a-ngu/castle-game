using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class PlayerController2D : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 6f;
    [SerializeField] private float gravity = -20;
    [SerializeField] private float jumpVelocity = 8f;

    private Vector2 input;
    private bool jumpPressed;
    private Vector3 velocity;

    private Controller2D controller;

    private void Start() {
        controller = GetComponent<Controller2D>();
    }

    private void Update() {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.GetKey(KeyCode.Space) && controller.collisions.below) {
            jumpPressed = true;
        }
    }

    private void FixedUpdate() {
        // Determine horizontal velocity
        velocity.x = input.x * movementSpeed;
        // Determine vertical velocity
        if (controller.collisions.above || controller.collisions.below) {
            velocity.y = 0;
        }
        if (jumpPressed) {
            jumpPressed = false;
            velocity.y = jumpVelocity;
        }
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}