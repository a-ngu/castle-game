using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CastleGame
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Camera Behavior")]
        [SerializeField] private float bias = 1;

        [Header("Smoothing Settings")]
        [SerializeField] private float _smoothTimeSlow = 0.3f;
        [SerializeField] private float _smoothTimeFast = 0.07f;
        private float _currPosX, _currPosY;

        private PlayerController player;

        private void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            transform.position = player.transform.position + Vector3.forward * -10 + bias * player.FacingDirection * Vector3.right;
        }

        private void FixedUpdate()
        {
            Vector3 currPosition = transform.position;
            Vector3 targetPosition = player.transform.position + Vector3.forward * -10 + bias * player.FacingDirection * Vector3.right;

            // X
            currPosition.x = Mathf.SmoothDamp(
                currPosition.x,
                targetPosition.x,
                ref _currPosX,
                _smoothTimeSlow
                );

            // Y
            float directionY = targetPosition.y - currPosition.y;
            currPosition.y = Mathf.SmoothDamp(
                currPosition.y,
                targetPosition.y,
                ref _currPosY,
                (directionY > 0) ? _smoothTimeSlow : _smoothTimeFast);

            transform.position = currPosition;
        }
    }
}
