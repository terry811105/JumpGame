using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        // 獲取水平輸入軸的值
        float horizontalInput = Input.GetAxis("Horizontal");

        // 根據輸入移動
        transform.Translate(Vector3.right * horizontalInput * moveSpeed * Time.deltaTime);
    }

    private void HandleJump()
    {
        // 檢查是否在地面上
        isGrounded = CheckIfGrounded();

        // 如果按下跳躍鍵且在地面上
        if (Input.GetKeyDown(KeyCode.Space) )
        {
            // 施加向上的力
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private bool CheckIfGrounded()
    {
        // 使用射線檢測是否在地面上
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, LayerMask.GetMask("Ground"));
        return hit.collider != null;
    }
}
