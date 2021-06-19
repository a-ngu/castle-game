using UnityEngine;

namespace CastleGame
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class RaycastBody2D : MonoBehaviour
    {
        [SerializeField] private float _raySpacing = 0.25f;
        public const float SkinWidth = 0.015f;
        
        // Ray setup
        public int HoriRayCount { get; private set; }
        public int VertRayCount { get; private set; }
        public float HoriRaySpacing { get; private set; }
        public float VertRaySpacing { get; private set; }

        // Raycast origins
        public Vector2 TopLeft { get; private set; }
        public Vector2 TopRight { get; private set; }
        public Vector2 BottomLeft { get; private set; }
        public Vector2 BottomRight { get; private set; }

        private BoxCollider2D _collider;

        private void Start()
        {
            _collider = GetComponent<BoxCollider2D>();
            Calibrate();
        }

        private void Calibrate()
        {
            Bounds bounds = _collider.bounds;
            bounds.Expand(SkinWidth * -2);

            HoriRayCount = (int)(bounds.size.y / _raySpacing) + 2;
            HoriRaySpacing = bounds.size.y / (HoriRayCount - 1);

            VertRayCount = (int)(bounds.size.x / _raySpacing) + 2;
            VertRaySpacing = bounds.size.x / (VertRayCount - 1);
        }

        public void UpdateRaycastOrigins()
        {
            Bounds bounds = _collider.bounds;
            bounds.Expand(SkinWidth * -2);

            TopLeft = new Vector2(bounds.min.x, bounds.max.y);
            TopRight = new Vector2(bounds.max.x, bounds.max.y);
            BottomLeft = new Vector2(bounds.min.x, bounds.min.y);
            BottomRight = new Vector2(bounds.max.x, bounds.min.y);
        }
    }
}
