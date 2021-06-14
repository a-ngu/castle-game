using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{
    [SerializeField] private LayerMask moveableLayer;
    [SerializeField] private Vector3 movement;
    
    // Caching
    private Dictionary<Transform, Controller2D> otherDict;

    protected override void Start() {
        base.Start();
        otherDict = new Dictionary<Transform, Controller2D>();
    }

    private void FixedUpdate() {
        CalculateOrigins();
        Vector3 motion = movement * Time.fixedDeltaTime;

        if (motion.y > 0) {
            MoveOthers(motion);
            transform.Translate(motion);
        } else {
            transform.Translate(motion);
            MoveOthers(motion);
        }
    }

    private void MoveOthers(Vector3 motion) {
        HashSet<Transform> detected = new HashSet<Transform>();
        float xDir, yDir, rayLen;
        Vector2 origin;
        RaycastHit2D hit;
        Controller2D controller;

        // Horizontal motion
        if (motion.x != 0) {
            // Moveable on left/right
            xDir = Mathf.Sign(motion.x);
            rayLen = Mathf.Abs(motion.x) + rayMargin;
            origin = (xDir < 0) ? origins.bottomLeft : origins.bottomRight;

            for (int i = 0; i < hRayCount; i++) {
                if (i > 0) {
                    origin += Vector2.up * hRaySpacing;
                }
                hit = Physics2D.Raycast(origin, Vector2.right * xDir, rayLen, collidable); 
                if (hit && !detected.Contains(hit.transform)) {
                    detected.Add(hit.transform);
                    if (otherDict.ContainsKey(hit.transform)) {
                        controller = otherDict[hit.transform];
                    } else {
                        controller = hit.transform.GetComponent<Controller2D>();
                        otherDict.Add(hit.transform, controller);
                    }
                    controller.Move(new Vector3(motion.x - (hit.distance - rayMargin) * xDir, 0, 0), true);
                }
            }
            // Moveable on top
            origin = origins.topLeft;

            for (int i = 0; i < vRayCount; i++) {
                if (i > 0) {
                    origin += Vector2.right * vRaySpacing;
                }
                hit = Physics2D.Raycast(origin, Vector2.up, rayMargin * 1.5f, collidable);  // rayMargin too small by itself
                if (hit && !detected.Contains(hit.transform)) {
                    detected.Add(hit.transform);
                    if (otherDict.ContainsKey(hit.transform)) {
                        controller = otherDict[hit.transform];
                    } else {
                        controller = hit.transform.GetComponent<Controller2D>();
                        otherDict.Add(hit.transform, controller);
                    }
                    controller.Move(new Vector3(motion.x - (hit.distance - rayMargin) * xDir, 0, 0), true);
                }
            }
        }

        // Vertical motion
        if (motion.y > 0) {
            rayLen = motion.y + rayMargin;
            origin = origins.topLeft;

            for (int i = 0; i < vRayCount; i++) {
                if (i > 0) {
                    origin += Vector2.right * vRaySpacing;
                }
                hit = Physics2D.Raycast(origin, Vector2.up, rayLen, collidable);
                if (hit && !detected.Contains(hit.transform)) {
                    detected.Add(hit.transform);
                    if (otherDict.ContainsKey(hit.transform)) {
                        controller = otherDict[hit.transform];
                    } else {
                        controller = hit.transform.GetComponent<Controller2D>();
                        otherDict.Add(hit.transform, controller);
                    }
                    controller.Move(new Vector3(0, motion.y - (hit.distance - rayMargin), 0), true);
                }
            }            
        } else if (motion.y < 0) {
            origin = origins.topLeft;
            for (int i = 0; i < vRayCount; i++) {
                if (i > 0) {
                    origin += Vector2.right * vRaySpacing;
                }
                hit = Physics2D.Raycast(origin, Vector2.up, rayMargin * 1.5f, collidable);
                if (hit && !detected.Contains(hit.transform)) {
                    detected.Add(hit.transform);
                    if (otherDict.ContainsKey(hit.transform)) {
                        controller = otherDict[hit.transform];
                    } else {
                        controller = hit.transform.GetComponent<Controller2D>();
                        otherDict.Add(hit.transform, controller);
                    }
                    controller.Move(new Vector3(0, motion.y - (hit.distance - rayMargin), 0), true);
                }
            }   
        }
    }
}
