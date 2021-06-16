using UnityEngine;

public class CharacterController2D : RaycastBody2D
{
    [SerializeField] private bool debugMode = false;
    [SerializeField] protected LayerMask collidableLayer;
    public CollisionStatus status;

    protected override void Start()
    {
        base.Start();
    }

    public void Move(Vector2 displacement, bool onPlatform = false)
    {
        UpdateRaycastOrigins();
        status.Reset();

        bool xAdjusted = false;
        bool yAdjusted = false;
        if (displacement.x != 0)
            xAdjusted = AdjustX(ref displacement);
        if (displacement.y != 0)
            yAdjusted = AdjustY(ref displacement);
        if (displacement.x != 0 && displacement.y != 0 && !xAdjusted && !yAdjusted)
            AdjustCorner(ref displacement);

        transform.Translate(displacement);

        if (onPlatform)
            status.below = true;
    }

    /// <summary>
    /// Adjust displacement to acconut for potential obstacle collisions in the x-direction.
    /// </summary>
    private bool AdjustX(ref Vector2 displacement)
    {
        bool adjusted = false;
        float directionX = Mathf.Sign(displacement.x);
        float rayLength = Mathf.Abs(displacement.x) + skinWidth;

        Vector2 origin = (directionX < 0) ? origins.bottomLeft : origins.bottomRight;
        for (int i = 0; i < horizontalRayCount; i++)
        {
            if (i > 0)
                origin += Vector2.up * horizontalRaySpacing;

            if (debugMode)
                Debug.DrawRay(origin, directionX * rayLength * Vector2.right, Color.red, 0.02f);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * directionX, rayLength, collidableLayer);

            if (hit && hit.distance != 0)
            {
                rayLength = hit.distance;
                displacement.x = (rayLength - skinWidth) * directionX;
                adjusted = true;

                if (directionX < 0)
                    status.left = true;
                else if (directionX > 0)
                    status.right = true;
            }
        }
        return adjusted;
    }

    /// <summary>
    /// Adjust displacement to account for potential obstacle collisions in the y-direction.
    /// </summary>
    private bool AdjustY(ref Vector2 displacement)
    {
        bool adjusted = false;
        float directionY = Mathf.Sign(displacement.y);
        float rayLength = Mathf.Abs(displacement.y) + skinWidth;

        Vector2 origin = (directionY < 0) ? origins.bottomLeft : origins.topLeft;
        // origin += Vector2.right * displacement.x;
        for (int i = 0; i < verticalRayCount; i++)
        {
            if (i > 0)
                origin += Vector2.right * verticalRaySpacing;

            if (debugMode)
                Debug.DrawRay(origin, directionY * rayLength * Vector2.up, Color.red, 0.02f);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up * directionY, rayLength, collidableLayer);

            if (hit && hit.distance != 0)
            {
                rayLength = hit.distance;
                displacement.y = (rayLength - skinWidth) * directionY;
                adjusted = true;

                if (directionY < 0)
                    status.below = true;
                else if (directionY > 0)
                    status.above = true;
            }
        }
        return adjusted;
    }

    /// <summary>
    /// Adjust displacement to account for corner cases not detected by AdjustX and AdjustY.
    /// </summary>
    private void AdjustCorner(ref Vector2 displacement)
    {
        // TODO
    }

    public struct CollisionStatus
    {
        public bool left, right;
        public bool above, below;
        public void Reset()
        {
            left = right = above = below = false;
        }
    }
}
