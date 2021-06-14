using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    [Header("Collision Behavior")]
    [SerializeField] protected LayerMask collidable;
    
    [Header("Raycase Configuration")]
    protected const float rayMargin = .015f;
    [Range(4, 20)] [SerializeField] protected int hRayCount = 4;
    [Range(4, 20)] [SerializeField] protected int vRayCount = 4;
    protected float hRaySpacing, vRaySpacing;

    new protected BoxCollider2D collider;
    protected RaycastOrigins origins;
    
    protected virtual void Start() {
        collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    private void CalculateRaySpacing() {
        Bounds b = collider.bounds;
        b.Expand(rayMargin * -2);

        hRaySpacing = b.size.y / (hRayCount - 1);
        vRaySpacing = b.size.x / (vRayCount - 1);
    }

    protected void CalculateOrigins() {
        Bounds b = collider.bounds;
        b.Expand(rayMargin * -2);

        origins.bottomLeft = new Vector2(b.min.x, b.min.y);
        origins.bottomRight = new Vector2(b.max.x, b.min.y);
        origins.topLeft = new Vector2(b.min.x, b.max.y);
        origins.topRight = new Vector2(b.max.x, b.max.y);
    }

    public struct RaycastOrigins {
        public Vector2 topLeft, topRight, bottomLeft, bottomRight;
    }
}
