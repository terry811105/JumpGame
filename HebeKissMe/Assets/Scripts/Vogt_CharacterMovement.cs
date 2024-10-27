using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject normalPlayer;           // 普通形態的角色物件
    [SerializeField] private GameObject transformedPlayer;      // 變身形態的角色物件

    public float normalMoveSpeed = 5f;           // 普通形態的移動速度
    public float transformedMoveSpeed = 8f;      // 變身形態的移動速度
    public float jumpForce = 10f;                // 跳躍力
    private Transform groundCheck;                // 檢查地面的 Transform
    public float checkRadius = 0.2f;             // 地面檢查半徑
    public LayerMask groundLayer;                // 地面圖層

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isTransformed = false;          // 是否變身的狀態
    private float moveInput;
    private float moveSpeed;                     // 當前移動速度

    void Start()
    {
        // 僅啟用普通形態，並禁用變身形態
        normalPlayer.SetActive(true);
        transformedPlayer.SetActive(false);

        // 設置剛體為普通形態的剛體
        rb = normalPlayer.GetComponent<Rigidbody2D>(); // 取得 Rigidbody2D 組件
        groundCheck = normalPlayer.transform.Find("GroundCheck");  // 初始化 groundCheck
        moveSpeed = normalMoveSpeed;                   // 初始化為普通形態的移動速度
    }

    void Update()
    {
        moveInput = Input.GetAxis("Horizontal");

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.velocity = Vector2.up * jumpForce;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleTransformation();
        }
    }

    void FixedUpdate()
    {
        Debug.DrawRay(groundCheck.position, Vector2.down * checkRadius, Color.red);

        if (groundCheck == null)
        {
            Debug.LogError("groundCheck is null! Make sure GroundCheck is properly assigned.");
            return;
        }

        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);

        // 增加 Debug 輸出
        Debug.Log("Is Grounded: " + isGrounded + ", Transformation State: " + (isTransformed ? "Transformed" : "Normal"));
    }

    void ToggleTransformation()
    {
        Vector2 currentVelocity = rb.velocity;
        isTransformed = !isTransformed;

        if (isTransformed)
        {
            transformedPlayer.transform.position = normalPlayer.transform.position;
            transformedPlayer.SetActive(true);
            normalPlayer.SetActive(false);

            rb = transformedPlayer.GetComponent<Rigidbody2D>() ?? transformedPlayer.AddComponent<Rigidbody2D>();
            groundCheck = transformedPlayer.transform.Find("GroundCheck") ?? throw new System.NullReferenceException("GroundCheck not found in transformedPlayer!");
            rb.velocity = currentVelocity;
            moveSpeed = transformedMoveSpeed;
        }
        else
        {
            normalPlayer.transform.position = transformedPlayer.transform.position;
            normalPlayer.SetActive(true);
            transformedPlayer.SetActive(false);

            rb = normalPlayer.GetComponent<Rigidbody2D>();
            groundCheck = normalPlayer.transform.Find("GroundCheck") ?? throw new System.NullReferenceException("GroundCheck not found in normalPlayer!");
            rb.velocity = currentVelocity;
            moveSpeed = normalMoveSpeed;
        }
    }
}
