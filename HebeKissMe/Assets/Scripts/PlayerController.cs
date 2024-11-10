using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f; // 設置移動速度
    public float jumpForce = 10f; // 跳躍力量
    private Rigidbody2D rb2D;
    private bool canTransform = false; // 用於標記是否可以變形
    public GameObject targetObject; // 變身的目標物件
    private bool isTransformed = false; // 標誌目前是否已變形

    public GameObject originalObject; // 保存原物件以便於切換
    

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();

        
        
        
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

        // 按下 R 鍵時，隨時恢復為原物件，不繼承當前動量及位置
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
            // 隱藏原物件並啟用目標物件
            originalObject.SetActive(false);
            targetObject.SetActive(true);

            isTransformed = true;
            Debug.Log("已變形為目標物件");
        }
    }

    void RevertToOriginal()
    {
        // 隱藏目標物件並啟用原物件
        targetObject.SetActive(false);
        originalObject.SetActive(true);

        isTransformed = false;
        Debug.Log("已恢復為原物件");
    }
}
