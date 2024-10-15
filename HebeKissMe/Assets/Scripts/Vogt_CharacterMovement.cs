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

    // ���D�޿�
    private void Jump()
    {
        Debug.Log("Jumping"); // ��X���D���T��
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _jumpForce);
    }

    private void SetMoveInput()
    {
        _smoothMovent = Vector2.SmoothDamp(_smoothMovent, _moveInput, ref _smoothVelocity, _damping);
        _rigidbody.velocity = new Vector2(_smoothMovent.x * _speed, _rigidbody.velocity.y);  // �����t�׫O������
    }

    private void FixedUpdate()
    {
        SetMoveInput();
    }

    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    // ���D��J�޿�
    public void OnJump(InputValue value)
    {
        if (value.isPressed)  // ����U���D�����
        {
            Debug.Log("Jump Pressed"); // ��X���U���D���䪺�T��
            Jump();
        }
    }
}
