using System.Collections.Generic;
using UnityEngine;
using Game.Collision;
using Game.Character;

namespace Game.Environment
{
    [RequireComponent(typeof(RaycastBody2D))]
    public class PlatformController2D : MonoBehaviour
    {
        [SerializeField] private LayerMask _moveableLayer;

        [Header("Waypoint Settings")]
        [SerializeField] private bool _showWaypoints = false;
        [SerializeField] private Vector2[] _relativePositions;
        private Vector2[] _absolutePositions;
        [SerializeField] private bool _cyclic = true;
        [SerializeField] private float _waitTime = 0.5f;
        public bool Enabled = true;

        [Header("Movement Settings")]
        [SerializeField] private float _movementSpeed = 3f;
        private int _currentWaypoint;
        private float _movementProgress;
        private float _currentWaitTime;

        private Dictionary<Transform, CharacterController2D> _cached;
        private RaycastBody2D _rb;

        private void Start()
        {
            _cached = new Dictionary<Transform, CharacterController2D>();
            _rb = GetComponent<RaycastBody2D>();

            _absolutePositions = new Vector2[_relativePositions.Length];

            for (int i = 0; i < _relativePositions.Length; i++)
            {
                _absolutePositions[i] = _relativePositions[i] + new Vector2(transform.position.x, transform.position.y);
            }
        }

        private void FixedUpdate()
        {
            if (Enabled && Time.time >= _currentWaitTime)
            {
                _rb.UpdateRaycastOrigins();

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
            int waypointCount = _absolutePositions.Length;

            _currentWaypoint %= waypointCount;
            int nextWaypoint = (_currentWaypoint + 1) % waypointCount;

            Vector2 currentPosition = _absolutePositions[_currentWaypoint];
            Vector2 nextPosition = _absolutePositions[nextWaypoint];

            float distance = Vector2.Distance(currentPosition, nextPosition);
            _movementProgress += _movementSpeed * Time.fixedDeltaTime / distance;

            Vector2 position = Vector2.Lerp(currentPosition, nextPosition, _movementProgress);

            if (_movementProgress >= 1)
            {
                _movementProgress = 0;
                _currentWaypoint++;

                if (!_cyclic && _currentWaypoint >= waypointCount - 1)
                {
                    _currentWaypoint = 0;
                    System.Array.Reverse(_absolutePositions);
                }

                _currentWaitTime = Time.time + _waitTime;
            }

            return position - new Vector2(transform.position.x, transform.position.y);
        }

        private void MoveOthers(Vector2 displacement)
        {
            HashSet<Transform> hasDetected = new HashSet<Transform>();

            if (displacement.x != 0 || displacement.y != 0)
            {
                float rayLength = Mathf.Abs(displacement.y) + RaycastBody2D.SkinWidth * 2f;
                Vector2 origin = _rb.TopLeft;

                for (int i = 0; i < _rb.VertRayCount; i++)
                {
                    if (i > 0)
                    {
                        origin += Vector2.right * _rb.VertRaySpacing;
                    }

                    RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.up, rayLength, _moveableLayer);
                    if (hit && !hasDetected.Contains(hit.transform) && hit.distance != 0)
                    {
                        hasDetected.Add(hit.transform);

                        CharacterController2D controller;
                        if (_cached.ContainsKey(hit.transform))
                        {
                            controller = _cached[hit.transform];
                        }
                        else
                        {
                            controller = hit.transform.GetComponent<CharacterController2D>();
                            _cached.Add(hit.transform, controller);
                        }
                        controller.Move(displacement, true);
                    }
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (_showWaypoints && _relativePositions != null)
            {
                Gizmos.color = Color.red;
                for (int i = 0; i < _relativePositions.Length; i++)
                {
                    Vector2 position = (Application.isPlaying) ? _absolutePositions[i] : _relativePositions[i] + new Vector2(transform.position.x, transform.position.y);
                    Gizmos.DrawSphere(position, 0.1f);
                }
            }
        }
    }
}