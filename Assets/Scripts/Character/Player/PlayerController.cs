using UnityEngine;

namespace Game.Character.Player
{
    [RequireComponent(typeof(CharacterController2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        // Horizontal Movement
        [SerializeField] private float _movementSpeed = 6f;
        // Vertical Movement
        [SerializeField] private float _maxHeight = 4f;
        [SerializeField] private float _timeToMaxHeight = 0.4f;
        [Range(0, 1)] [SerializeField] private float _endJumpDamping = 0.5f;
        [Range(0, 1)] [SerializeField] private float _wallSlideDamping = 0.8f;
        [Range(0, 90)] [SerializeField] private float _wallJumpAngleIn = 80f;
        [Range(0, 90)] [SerializeField] private float _wallJumpAngleOut = 60f;
        private float _gravity;
        private float _initJumpVelocity;
        // Movement Smoothing
        [SerializeField] private float _smoothTimeAirborne = 0.2f;
        [SerializeField] private float _smoothTimeGrounded = 0.1f;
        private float _currSmoothVelocity;

        [Header("Input Sensitivity")]
        [SerializeField] private float _persistanceTime = 0.2f;

        // State tracking
        private Vector2 _input;

        private float _groundedTimer;

        private float _jumpingTimer;
        private bool _jumping, _canDoubleJump, _endJump;

        private float _directionToWall;
        private bool _onWall;

        private Vector2 _prevVelocity, _velocity;

        private CharacterController2D _controller;

        private void Start()
        {
            _controller = GetComponent<CharacterController2D>();
            ComputePhysicalSettings();
        }

        private void ComputePhysicalSettings()
        {
            _gravity = -2 * _maxHeight / Mathf.Pow(_timeToMaxHeight, 2);
            _initJumpVelocity = 2 * _maxHeight / _timeToMaxHeight;
        }

        private void Update()
        {
            ProcessInput();
            CheckState();

            // Jumping
            if (_jumpingTimer > 0 && (_groundedTimer > 0 || _onWall))
            {
                _jumping = _canDoubleJump = true;
                _jumpingTimer = 0;
                _groundedTimer = 0;
            }
            if (_jumpingTimer > 0 && _groundedTimer <= 0 && !_onWall && _canDoubleJump)
            {
                _jumping = true;
                _canDoubleJump = false;
                _jumpingTimer = 0;
            }
            if (_jumpingTimer > 0)
                _jumpingTimer -= Time.deltaTime;
            if (_groundedTimer > 0)
                _groundedTimer -= Time.deltaTime;
        }

        private void FixedUpdate()
        {
            CalculateVelocity();
            _controller.Move(0.5f * Time.fixedDeltaTime * (_velocity + _prevVelocity));
            _prevVelocity = _velocity;
        }

        private void ProcessInput()
        {
            _input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (Input.GetKeyDown(KeyCode.Space))
                _jumpingTimer = _persistanceTime;
            if (Input.GetKeyUp(KeyCode.Space))
                _endJump = true;
        }

        private void CheckState()
        {
            if (_controller.CollideBelow)
            {
                _groundedTimer = _persistanceTime;
                _onWall = false;
            }
            else
            {
                if (_controller.CollideLeft && _velocity.y < 0)
                {
                    _onWall = true;
                    _directionToWall = -1;
                }
                else if (_controller.CollideRight && _velocity.y < 0)
                {
                    _onWall = true;
                    _directionToWall = 1;
                }
                else _onWall = false;
            }
        }

        private void CalculateVelocity()
        {
            // Horizontal Velocity
            if (!_onWall)
            {
                _velocity.x = Mathf.SmoothDamp(
                    _velocity.x,
                    _input.x * _movementSpeed,
                    ref _currSmoothVelocity,
                    (_controller.CollideBelow) ? _smoothTimeGrounded : _smoothTimeAirborne);
            }
            // Vertical Velocity
            if (_controller.CollideBelow || _controller.CollideAbove)
                _velocity.y = 0;
            if (_jumping)
            {
                _jumping = false;
                if (_onWall)
                {
                    _onWall = false;
                    float activeAngle = (_input.x == _directionToWall) ? _wallJumpAngleIn : _wallJumpAngleOut;
                    _velocity.x = _initJumpVelocity * Mathf.Cos(Mathf.Deg2Rad * activeAngle) * _directionToWall * -1;
                    _velocity.y = _initJumpVelocity * Mathf.Sin(Mathf.Deg2Rad * activeAngle);
                }
                else _velocity.y = _initJumpVelocity;
            }
            if (_endJump)
            {
                _endJump = false;
                if (_velocity.y > 0)
                    _velocity.y *= _endJumpDamping;
            }

            _velocity.y += _gravity * Time.fixedDeltaTime;
            if (_onWall && _velocity.y < 0)
                _velocity.y *= _wallSlideDamping;
        }
    }
}
