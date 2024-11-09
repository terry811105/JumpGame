using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f; // 設置移動速度
    public float jumpForce = 10f; // 跳躍力量
    private Rigidbody2D rb2D;
    public Transform characterTransform;
    void Start()
    {
      

         rb2D = GetComponent<Rigidbody2D>();
        // 確認 characterTransform 不為空
        if (characterTransform == null)
        {
            Debug.LogError("請指定主要角色的 Transform。");
        }
    }

    void FixedUpdate()
    {
        // 獲取水平輸入
        float horizontalInput = Input.GetAxis("Horizontal");

        // 調試訊息，顯示水平輸入值
        Debug.Log("Horizontal Input: " + horizontalInput);

        // 設定水平速度
        rb2D.velocity = new Vector2(horizontalInput * speed, rb2D.velocity.y);

        // 調試訊息，顯示當前速度
        Debug.Log("Rigidbody2D velocity: " + rb2D.velocity);
    }

    void Update()
    {
        // 檢查跳躍輸入並確保 Y 速度接近 0 才能跳躍
        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(rb2D.velocity.y) < 0.01f)
        {
            rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        if (characterTransform != null)
        {
            characterTransform.position = transform.position;
        }
    }
}
