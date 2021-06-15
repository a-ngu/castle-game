using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastBody2D : MonoBehaviour
{
    [SerializeField] protected LayerMask collidable;
    [SerializeField] private float raySpacing = 0.25f;
    protected const float skinWidth = 0.015f;
    protected int horizontalRayCount, verticalRayCount;
    protected float horizontalRaySpacing, verticalRaySpacing;

    new private BoxCollider2D collider;
    protected RaycastOrigins origins;

    protected virtual void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        Calibrate();
    }

    private void Calibrate()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = (int)(bounds.size.y / raySpacing) + 2;
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);

        verticalRayCount = (int)(bounds.size.x / raySpacing) + 2;
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    protected void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        origins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        origins.topRight = new Vector2(bounds.max.x, bounds.max.y);
        origins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        origins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
    }

    protected struct RaycastOrigins
    {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
    }
}
