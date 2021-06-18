using UnityEngine;
using Game.Collision;

namespace Game.Character
{
    [RequireComponent(typeof(RaycastBody2D))]
    public class CharacterController2D : MonoBehaviour
    {
        [SerializeField] private LayerMask _collidableLayer;
        
        // Collision Status
        public bool CollideLeft { get; private set; }
        public bool CollideRight { get; private set; }
        public bool CollideAbove { get; private set; }
        public bool CollideBelow { get; private set; }

        private RaycastBody2D _rb;

        private void Start()
        {
            _rb = GetComponent<RaycastBody2D>();
        }

        /// <summary>
        /// Move Character according to provided displacement. If Character collides with another object, move only up to that collision.
        /// </summary>
        public void Move(Vector2 displacement, bool onPlatform = false)
        {
            _rb.UpdateRaycastOrigins();
            ResetCollisionStatus();

            bool adjustedX, adjustedY;
            adjustedX = adjustedY = false;

            if (displacement.x != 0)
            {
                adjustedX = AdjustX(ref displacement);
            }
            if (displacement.y != 0)
            {
                adjustedY = AdjustY(ref displacement);
            }
            if (displacement.x != 0 && displacement.y != 0 && !adjustedX && !adjustedY)
            {
                AdjustCorner(ref displacement);
            }

            if (onPlatform)
            {
                CollideBelow = true;
            }

            transform.Translate(displacement);
        }

        private bool AdjustX(ref Vector2 displacement)
        {
            bool adjusted = false;
            float directionX = Mathf.Sign(displacement.x);
            float rayLength = Mathf.Abs(displacement.x) + RaycastBody2D.SkinWidth;

            Vector2 origin = (directionX < 0) ? _rb.BottomLeft : _rb.BottomRight;
            for (int i = 0; i < _rb.HoriRayCount; i++)
            {
                if (i > 0)
                {
                    origin += Vector2.up * _rb.HoriRaySpacing;
                }
                    
                RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * directionX, rayLength, _collidableLayer);
                if (hit && hit.distance != 0)
                {
                    rayLength = hit.distance;
                    displacement.x = (rayLength - RaycastBody2D.SkinWidth) * directionX;

                    if (directionX < 0)
                    {
                        CollideLeft = true;
                    }
                    else
                    {
                        CollideRight = true;
                    }

                    adjusted = true;
                }
            }

            return adjusted;
        }

        private bool AdjustY(ref Vector2 displacement)
        {
            bool adjusted = false;
            float directionY = Mathf.Sign(displacement.y);
            float rayLength = Mathf.Abs(displacement.y) + RaycastBody2D.SkinWidth;

            Vector2 origin = (directionY < 0) ? _rb.BottomLeft : _rb.TopLeft;
            // origin += Vector2.right * displacement.x;
            for (int i = 0; i < _rb.VertRayCount; i++)
            {
                if (i > 0)
                {
                    origin += Vector2.right * _rb.VertRaySpacing;
                }
                    
                RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up * directionY, rayLength, _collidableLayer);
                if (hit && hit.distance != 0)
                {
                    rayLength = hit.distance;
                    displacement.y = (rayLength - RaycastBody2D.SkinWidth) * directionY;

                    if (directionY < 0)
                    {
                        CollideBelow = true;
                    }
                    else
                    {
                        CollideAbove = true;
                    }

                    adjusted = true;
                }
            }

            return adjusted;
        }

        private void AdjustCorner(ref Vector2 displacement)
        {
            float direcitonX = Mathf.Sign(displacement.x);
            float directionY = Mathf.Sign(displacement.y);
            float rayLength = displacement.magnitude;

            Vector2 origin;
            if (direcitonX > 0 && directionY > 0)
            {
                origin = _rb.TopRight;
            }
            else if (direcitonX > 0 && directionY < 0)
            {
                origin = _rb.BottomRight;
            }
            else if (direcitonX < 0 && directionY > 0)
            {
                origin = _rb.TopLeft;
            }
            else
            {
                origin = _rb.BottomLeft;
            }

            RaycastHit2D hit = Physics2D.Raycast(origin, displacement.normalized, rayLength, _collidableLayer);
            if (hit && hit.distance != 0)
                displacement = displacement.normalized * ((hit.distance - RaycastBody2D.SkinWidth) / rayLength);
        }

        private void ResetCollisionStatus()
        {
            CollideLeft = CollideRight = CollideAbove = CollideBelow = false;
        }
    }
}
