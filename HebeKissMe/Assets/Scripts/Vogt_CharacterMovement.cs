using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Vogt_CharacterMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _damping = 0.1f;
    [SerializeField] private float _jumpForce = 1f;

    private Vector2 _moveInput;
    private Vector2 _smoothMovent;
    private Vector2 _smoothVelocity;
    private Rigidbody2D _rigidbody;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 跳躍邏輯
    private void Jump()
    {
        Debug.Log("Jumping"); // 輸出跳躍的訊息
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _jumpForce);
    }

    private void SetMoveInput()
    {
        _smoothMovent = Vector2.SmoothDamp(_smoothMovent, _moveInput, ref _smoothVelocity, _damping);
        _rigidbody.velocity = new Vector2(_smoothMovent.x * _speed, _rigidbody.velocity.y);  // 垂直速度保持不變
    }

    private void FixedUpdate()
    {
        SetMoveInput();
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    // 跳躍輸入邏輯
    public void OnJump(InputValue value)
    {
        if (value.isPressed)  // 當按下跳躍按鍵時
        {
            Debug.Log("Jump Pressed"); // 輸出按下跳躍按鍵的訊息
            Jump();
        }
    }
}
