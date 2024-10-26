using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f; // 移動速度
    private Rigidbody2D rb;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // 取得 Rigidbody2D 組件
    }

    void Update()
    {
        // 取得水平輸入 (A, D 鍵或左、右方向鍵)
        moveInput = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        // 移動角色
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }
}