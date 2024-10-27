using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;       // 移動速度
    public float jumpForce = 10f;  // 跳躍力
    private Rigidbody2D rb;
    private float moveInput;
    private bool isGrounded;

    public Transform groundCheck;           // 用於檢查是否在地面
    public float checkRadius = 0.2f;        // 地面檢查半徑
    public LayerMask groundLayer;           // 地面圖層

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // 取得 Rigidbody2D 組件
    }

    void Update()
    {
        // 取得水平輸入 (A, D 鍵或左、右方向鍵)
        moveInput = Input.GetAxis("Horizontal");

        // 檢查是否在地面並按下跳躍鍵
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = Vector2.up * jumpForce;
        }
    }

    void FixedUpdate()
    {
        // 移動角色
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        // 檢查是否在地面
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // 調試用：顯示 isGrounded 的狀態
        Debug.Log("Is Grounded: " + isGrounded);
    }
}
