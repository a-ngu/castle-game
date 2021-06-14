using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastController
{
    [Header("Debugger Settings")]
    [SerializeField] private bool debugMode = false;

    public Collisions collisions;

    protected override void Start() {
        base.Start();
    }

    public void Move(Vector3 motion) {
        CalculateOrigins();
        collisions.Reset();

        if (motion.x != 0) {
            detectCollisionHori(ref motion);
        }
        if (motion.y != 0) {
            detectCollisionVert(ref motion);
        }
        transform.Translate(motion);
    }

    // Detect and adjust horizontal motion based on raycast collisions.
    private void detectCollisionHori(ref Vector3 motion) {
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
    private void detectCollisionVert(ref Vector3 motion) {
        float yDir = Mathf.Sign(motion.y);
        float rayLen = Mathf.Abs(motion.y) + rayMargin;
        Vector2 origin = (yDir < 0) ? origins.bottomLeft : origins.topLeft;
        origin += Vector2.right * motion.x; // Account for position change occured in detectCollisionHori.

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

    public struct Collisions {
        public bool left, right, above, below;
        public void Reset() {
            left = right = above = below = false;
        }
    }
}