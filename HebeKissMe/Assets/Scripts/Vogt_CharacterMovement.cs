using UnityEngine;

public class PlayerController : MonoBehaviour
{
[SerializeField] private GameObject normalPlayer;           // 普通形態的角色物件
[SerializeField] private GameObject transformedPlayer;      // 變身形態的角色物件


    public float moveSpeed = 5f;              // 移動速度
    public float jumpForce = 10f;             // 跳躍力
    public Transform groundCheck;             // 檢查地面的 Transform
    public float checkRadius = 0.2f;          // 地面檢查半徑
    public LayerMask groundLayer;             // 地面圖層

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isTransformed = false;       // 是否變身的狀態
    private float moveInput;

    void Start()
    {

        // 初始化為普通形態，禁用變身形態
        normalPlayer.SetActive(true);
        transformedPlayer.SetActive(false);

        // 設置剛體為普通形態的剛體

        rb = normalPlayer.GetComponent<Rigidbody2D>(); // 取得 Rigidbody2D 組件
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

        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleTransformation(); // 切換變身狀態
        }
    }

    void FixedUpdate()
    {
        // 移動角色
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // 檢查是否在地面
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // 調試用：顯示 isGrounded 的狀態
        Debug.Log("Is Grounded: " + isGrounded);
    }

    void ToggleTransformation()
    {
        isTransformed = !isTransformed;

        if (isTransformed)
        {
            transformedPlayer.transform.position = normalPlayer.transform.position;
            transformedPlayer.SetActive(true);
            normalPlayer.SetActive(false);

            rb = transformedPlayer.GetComponent<Rigidbody2D>();
            groundCheck = transformedPlayer.transform.Find("GroundCheck");
        }
        else
        {
            normalPlayer.transform.position = transformedPlayer.transform.position;
            normalPlayer.SetActive(true);
            transformedPlayer.SetActive(false);

            rb = normalPlayer.GetComponent<Rigidbody2D>();
            groundCheck = normalPlayer.transform.Find("GroundCheck");
        }
    }
}
