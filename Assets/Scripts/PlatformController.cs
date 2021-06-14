using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController
{
    [Header("Interactable")]
    [SerializeField] private LayerMask moveableLayer;

    [Header("Waypoint Settings")]
    [SerializeField] private Vector3[] relWaypoints;
    private Vector3[] absWaypoints;
    [SerializeField] private bool waypointsVisible;
    [SerializeField] private bool cyclic;
    [SerializeField] private float waitTime = .5f;
    
    [Header("Movement Behavior")]
    [SerializeField] private float speed = 3f;
    private int currWaypoint;
    private float moveProgress;   // between current waypoint and next waypoint
    private float nextMoveTime;

    // Caching
    private Dictionary<Transform, Controller2D> otherDict;

    protected override void Start() {
        base.Start();
        otherDict = new Dictionary<Transform, Controller2D>();
        
        absWaypoints = new Vector3[relWaypoints.Length];
        for (int i = 0; i < relWaypoints.Length; i++) {
            absWaypoints[i] = relWaypoints[i] + transform.position;
        }
    }

    private void FixedUpdate() {
        CalculateOrigins();

        if (Time.time >= nextMoveTime) {
            Vector3 motion = GenerateMotion();
            if (motion.y > 0) {
                MoveOthers(motion);
                transform.Translate(motion);
            } else {
                transform.Translate(motion);
                MoveOthers(motion);
            }
        }
    }

    private Vector3 GenerateMotion() {

        currWaypoint %= absWaypoints.Length;
        int nextWaypoint = (currWaypoint + 1) % absWaypoints.Length;

        Vector3 currWaypointPos = absWaypoints[currWaypoint];
        Vector3 nextWaypointPos = absWaypoints[nextWaypoint];
        float distToNext = Vector3.Distance(currWaypointPos, nextWaypointPos);
        moveProgress += speed * Time.fixedDeltaTime / distToNext;
        Vector3 nextPos = Vector3.Lerp(currWaypointPos, nextWaypointPos, moveProgress);

        if (moveProgress >= 1) {
            moveProgress = 0;
            currWaypoint++;
            if (!cyclic && currWaypoint >= absWaypoints.Length - 1) {
                currWaypoint = 0;
                System.Array.Reverse(absWaypoints);
            }
            nextMoveTime = Time.time + waitTime;
        }

        return nextPos - transform.position;
    }

    private void MoveOthers(Vector3 motion) {
        HashSet<Transform> detected = new HashSet<Transform>();
        float xDir, rayLen;
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
                    controller.Move(new Vector3(motion.x - (hit.distance - rayMargin) * xDir, motion.y, 0), true);
                }
            }
            // Moveable on top
            origin = origins.topLeft;

            for (int i = 0; i < vRayCount; i++) {
                if (i > 0) {
                    origin += Vector2.right * vRaySpacing;
                }
                hit = Physics2D.Raycast(origin, Vector2.up, rayMargin * 2f, collidable);  // rayMargin too small by itself
                if (hit && !detected.Contains(hit.transform)) {
                    detected.Add(hit.transform);
                    if (otherDict.ContainsKey(hit.transform)) {
                        controller = otherDict[hit.transform];
                    } else {
                        controller = hit.transform.GetComponent<Controller2D>();
                        otherDict.Add(hit.transform, controller);
                    }
                    controller.Move(new Vector3(motion.x, motion.y, 0), true);
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
                hit = Physics2D.Raycast(origin, Vector2.up, rayMargin * 2f, collidable);
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

    private void OnDrawGizmos() {
        if (relWaypoints != null && waypointsVisible) {
            Gizmos.color = Color.red;
            for (int i = 0; i < relWaypoints.Length; i++) {
                Vector3 pos = (Application.isPlaying) ? absWaypoints[i] : relWaypoints[i] + transform.position;
                Gizmos.DrawSphere(pos, .1f);
            }
        }
    }
}
