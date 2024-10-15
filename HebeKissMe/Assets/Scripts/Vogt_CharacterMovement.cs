using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vogt_CharacterMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;

    private Vector2 _moveInput;
    private Vector2 _smoothMovent;
    private Vector2 _smootthVelocity;
    private Rigidbody2D _rigidbody;

    private void Start()
    {
        _rigidbody=GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        _smoothMovent = Vector2.SmoothDamp(_smoothMovent, _moveInput, ref _smootthVelocity, 0.5f);
        _rigidbody.velocity = _moveInput * _speed;
    }

    public void OnMove(InputValue Value)
    {
        _moveInput = Value.Get<Vector2>();
    }
}
