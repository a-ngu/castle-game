using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{
    [SerializeField] private LayerMask moveableLayer;
    [SerializeField] private Vector3 movement;
    
    protected override void Start() {
        base.Start();
    }

    private void FixedUpdate() {
        CalculateOrigins();
        Vector3 motion = movement * Time.fixedDeltaTime;
        MoveOthers(motion);
        transform.Translate(motion);
    }

    private void MoveOthers(Vector3 motion) {
        HashSet<Transform> detected = new HashSet<Transform>();
        
        // Horizontal motion
        if (motion.x != 0) {
            // Moveable on left/right
            float xDir = Mathf.Sign(motion.x);
            float rayLen = Mathf.Abs(motion.x) + rayMargin;
            Vector2 origin = (xDir < 0) ? origins.bottomLeft : origins.bottomRight;

            for (int i = 0; i < hRayCount; i++) {
                if (i > 0) {
                    origin += Vector2.up * hRaySpacing;
                }
                RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * xDir, rayLen, collidable); 
                if (hit && !detected.Contains(hit.transform)) {
                    detected.Add(hit.transform);
                    hit.transform.Translate(new Vector3(motion.x - (hit.distance - rayMargin) * xDir, 0, 0));
                }
            }
            // Moveable on top
            origin = origins.topLeft;

            for (int i = 0; i < vRayCount; i++) {
                if (i > 0) {
                    origin += Vector2.right * vRaySpacing;
                }
                RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, rayMargin * 1.5f, collidable);  // rayMargin too small by itself
                if (hit && !detected.Contains(hit.transform)) {
                    detected.Add(hit.transform);
                    hit.transform.Translate(new Vector3(motion.x - (hit.distance - rayMargin) * xDir, 0, 0));
                }
            }
        }

        // Vertical motion and Moveable on top
        if (motion.y != 0) {
            float yDir = Mathf.Sign(motion.y);
            float rayLen = Mathf.Abs(motion.y) + rayMargin;
            Vector2 origin = (yDir < 0) ? origins.bottomLeft : origins.topLeft;

            for (int i = 0; i < vRayCount; i++) {
                if (i > 0) {
                    origin += Vector2.right * vRaySpacing;
                }
                RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up * yDir, rayLen, collidable);
                if (hit && !detected.Contains(hit.transform)) {
                    detected.Add(hit.transform);
                    hit.transform.Translate(new Vector3(0, motion.y - (hit.distance - rayMargin) * yDir, 0));
                }
            }
        }
    }
}
