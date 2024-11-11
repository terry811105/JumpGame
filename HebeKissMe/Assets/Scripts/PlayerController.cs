using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f; // 設置移動速度
    public float jumpForce = 10f; // 跳躍力量
    private Rigidbody2D rb2D;
    
    public GameObject originalObject; // 保存原物件以便於切換
    

    void Start()
    {
        rb2D = originalObject.GetComponent<Rigidbody2D>();
        
        
        
        
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

       
    }

   

 

    
}
