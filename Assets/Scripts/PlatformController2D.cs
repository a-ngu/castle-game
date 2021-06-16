using System.Collections.Generic;
using UnityEngine;

public class PlatformController2D : RaycastBody2D
{
    [SerializeField] private LayerMask moveableLayer;

    [Header("Waypoint Settings")]
    [SerializeField] private bool showWaypoints = false;
    [SerializeField] private Vector2[] relativePositions;
    private Vector2[] absolutePositions;
    [SerializeField] private bool cyclic = true;
    [SerializeField] private float waitTime = 0.5f;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 3f;
    private int currentWaypoint;
    private float movementProgress;
    private float currentWaitTime;

    // Caching
    private Dictionary<Transform, CharacterController2D> cached;

    protected override void Start()
    {
        base.Start();
        cached = new Dictionary<Transform, CharacterController2D>();

        int waypointCount = relativePositions.Length;
        absolutePositions = new Vector2[waypointCount];
        for (int i = 0; i < waypointCount; i++)
        {
            absolutePositions[i] =
                relativePositions[i] +
                new Vector2(transform.position.x, transform.position.y);
        }
    }

    private void FixedUpdate()
    {
        if (Time.time >= currentWaitTime)
        {
            UpdateRaycastOrigins();
            Vector2 displacement = CalculatePlatformDisplacement();
            if (displacement.y > 0)
            {
                MoveOthers(displacement);
                transform.Translate(displacement);
            }
            else
            {
                transform.Translate(displacement);
                MoveOthers(displacement);
            }
        }
    }

    private Vector2 CalculatePlatformDisplacement()
    {
        int waypointCount = absolutePositions.Length;
        currentWaypoint %= waypointCount;
        int nextWaypoint = (currentWaypoint + 1) % waypointCount;
        Vector2 currentPosition = absolutePositions[currentWaypoint];
        Vector2 nextPosition = absolutePositions[nextWaypoint];

        float distance = Vector2.Distance(currentPosition, nextPosition);
        movementProgress += movementSpeed * Time.fixedDeltaTime / distance;

        Vector2 position = Vector2.Lerp(currentPosition, nextPosition, movementProgress);

        if (movementProgress >= 1)
        {
            movementProgress = 0;
            currentWaypoint++;
            if (!cyclic && currentWaypoint >= waypointCount - 1)
            {
                currentWaypoint = 0;
                System.Array.Reverse(absolutePositions);
            }
            currentWaitTime = Time.time + waitTime;
        }

        return position - new Vector2(transform.position.x, transform.position.y);
    }

    /// <summary>
    /// Move moveable objects on top of platform by the same displacement.
    /// </summary>
    private void MoveOthers(Vector2 displacement)
    {
        HashSet<Transform> hasDetected = new HashSet<Transform>();

        if (displacement.x != 0 || displacement.y != 0)
        {
            float rayLength = Mathf.Abs(displacement.y) + skinWidth * 2f;
            Vector2 origin = origins.topLeft;

            for (int i = 0; i < verticalRayCount; i++)
            {
                if (i > 0)
                    origin += Vector2.right * verticalRaySpacing;
                RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, rayLength, moveableLayer);
                if (hit && !hasDetected.Contains(hit.transform) && hit.distance != 0)
                {
                    hasDetected.Add(hit.transform);
                    CharacterController2D controller;
                    if (cached.ContainsKey(hit.transform))
                        controller = cached[hit.transform];
                    else
                    {
                        controller = hit.transform.GetComponent<CharacterController2D>();
                        cached.Add(hit.transform, controller);
                    }
                    controller.Move(displacement, true);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (showWaypoints && relativePositions != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < relativePositions.Length; i++)
            {
                Vector2 position =
                    (Application.isPlaying) ? absolutePositions[i] : relativePositions[i] + new Vector2(transform.position.x, transform.position.y);
                Gizmos.DrawSphere(position, 0.1f);
            }
        }
    }
}
