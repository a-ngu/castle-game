using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    [Header("Debugger Settings")]
    [SerializeField] private bool debugMode = false;

    [Header("Collision Behavior")]
    [SerializeField] private LayerMask collidable;
    
    [Header("Raycase Configuration")]
    private const float rayMargin = .015f;
    [Range(0, 12)] [SerializeField] private int hRayCount = 4;
    [Range(0, 12)] [SerializeField] private int vRayCount = 4;
    private float hRaySpacing, vRaySpacing;

    new private BoxCollider2D collider;
    private RaycastOrigins origins;
    public Collisions collisions;

    private void Start() {
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    public void Move(Vector3 motion) {
        CalculateOrigins();
        collisions.Reset();

        if (motion.x != 0) {
            detectCollisionHorizontal(ref motion);
        }
        if (motion.y != 0) {
            detectCollisionVertical(ref motion);
        }
        transform.Translate(motion);
    }

    // Detect and adjust horizontal motion based on raycast collisions.
    private void detectCollisionHorizontal(ref Vector3 motion) {
        float xDir = Mathf.Sign(motion.x);
        float rayLen = Mathf.Abs(motion.x) + rayMargin;
        Vector2 origin = (xDir < 0) ? origins.bottomLeft : origins.bottomRight;

        for (int i = 0; i < hRayCount; i++) {
            if (i > 0) {
                origin += Vector2.up * hRaySpacing;
            }
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * xDir, rayLen, collidable); 
            if (debugMode) {
                Debug.DrawRay(origin, Vector2.right * xDir * rayLen, Color.red, .01f);
            }
            if (hit) {
                rayLen = hit.distance;
                motion.x = (rayLen - rayMargin) * xDir;

                collisions.left = (xDir < 0);
                collisions.right = (xDir > 0); 
            }
        }
    }

    // Detect and adjust vertical motion based on raycast collisions.
    private void detectCollisionVertical(ref Vector3 motion) {
        float yDir = Mathf.Sign(motion.y);
        float rayLen = Mathf.Abs(motion.y) + rayMargin;
        Vector2 origin = (yDir < 0) ? origins.bottomLeft : origins.topLeft;
        origin += Vector2.right * motion.x; // Account for position change occured in detectCollisionHorizontal.

        for (int i = 0; i < vRayCount; i++) {
            if (i > 0) {
                origin += Vector2.right * vRaySpacing;
            }
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up * yDir, rayLen, collidable);
            if (debugMode) {
                Debug.DrawRay(origin, Vector2.up * yDir * rayLen, Color.red, .01f);
            }
            if (hit) {
                rayLen = hit.distance;
                motion.y = (rayLen - rayMargin) * yDir;

                collisions.below = (yDir < 0);
                collisions.above = (yDir > 0);
            }
        }
    }

    private void CalculateOrigins() {
        Bounds b = collider.bounds;
        b.Expand(rayMargin * -2);

        origins.bottomLeft = new Vector2(b.min.x, b.min.y);
        origins.bottomRight = new Vector2(b.max.x, b.min.y);
        origins.topLeft = new Vector2(b.min.x, b.max.y);
        origins.topRight = new Vector2(b.max.x, b.max.y);
    }

    private void CalculateRaySpacing() {
        Bounds b = collider.bounds;
        b.Expand(rayMargin * -2);

        hRaySpacing = b.size.y / (hRayCount - 1);
        vRaySpacing = b.size.x / (vRayCount - 1);
    }

    private struct RaycastOrigins {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
    }

    public struct Collisions {
        public bool left, right, above, below;
        public void Reset() {
            left = right = above = below = false;
        }
    }
}