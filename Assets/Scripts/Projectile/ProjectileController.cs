using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [SerializeField] private float _speed = 12f;
    private Vector2 _direction;

    public void SetDirection(Vector2 direction)
    {
        _direction = direction;
    }
}
