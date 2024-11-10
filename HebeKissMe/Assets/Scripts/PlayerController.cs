using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f; // 設置移動速度
    public float jumpForce = 10f; // 跳躍力量
    private Rigidbody2D rb2D;
    private bool canTransform = false; // 用於標記是否可以變形
    public GameObject targetObject; // 要變成的目標物件
    private bool isTransformed = false; // 標誌目前是否已變形

    // 原物件的屬性
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 originalScale;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();

        // 保存原物件的初始屬性
        originalPosition = transform.position;
        originalRotation = transform.rotation;
        originalScale = transform.localScale;
    }

    void FixedUpdate()
    {
        // 獲取水平輸入
        float horizontalInput = Input.GetAxis("Horizontal");

        // 設定水平速度
        rb2D.velocity = new Vector2(horizontalInput * speed, rb2D.velocity.y);
    }

    void Update()
    {
        // 檢查跳躍輸入並確保 Y 速度接近 0 才能跳躍
        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(rb2D.velocity.y) < 0.01f)
        {
            rb2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // 當可以變形且按下 T 鍵時，進行變形
        if (canTransform && Input.GetKeyDown(KeyCode.T) && !isTransformed)
        {
            TransformIntoTarget();
        }

        // 按下 R 鍵時，隨時恢復為原物件並繼承當前動量及位置
        if (isTransformed && Input.GetKeyDown(KeyCode.R))
        {
            RevertToOriginal();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 假設當碰到特定物件（如 "TransformableObject"）才允許變形
        if (collision.gameObject.CompareTag("TransformableObject"))
        {
            canTransform = true;
            Debug.Log("可以變形");
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // 離開碰撞後取消變形能力
        if (collision.gameObject.CompareTag("TransformableObject"))
        {
            canTransform = false;
            Debug.Log("無法變形");
        }
    }

    void TransformIntoTarget()
    {
        // 變形為目標物件
        if (targetObject != null)
        {
            transform.position = targetObject.transform.position;
            transform.rotation = targetObject.transform.rotation;
            transform.localScale = targetObject.transform.localScale;
            isTransformed = true;

            // 隱藏或停用目標物件
            targetObject.SetActive(false);
            Debug.Log("已變形為目標物件，目標物件已隱藏");
        }
    }

    void RevertToOriginal()
    {
        // 保存當前的動量和位置
        Vector2 currentVelocity = rb2D.velocity;
        Vector3 currentPosition = transform.position;

        // 恢復到原物件狀態（外觀）
        transform.position = currentPosition; // 保留當前位置
        transform.rotation = originalRotation;
        transform.localScale = originalScale;

        // 恢復原狀態的動量
        rb2D.velocity = currentVelocity;
        isTransformed = false;

        // 重新顯示目標物件並設定位置和動量
        if (targetObject != null)
        {
            targetObject.transform.position = currentPosition;
            targetObject.SetActive(true);

            // 如果目標物件有 Rigidbody2D，繼承當前動量
            Rigidbody2D targetRb = targetObject.GetComponent<Rigidbody2D>();
            if (targetRb != null)
            {
                targetRb.velocity = currentVelocity;
            }
        }

        Debug.Log("已恢復為原物件並保留動量，目標物件已顯示在當前位置並繼承動量");
    }
}
