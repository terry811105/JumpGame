using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerScripts : MonoBehaviour
{
    
    public GameObject playerObject; // 玩家物件的引用
    private Rigidbody2D playerRb;   // 玩家刚體
    private Collider2D playerCollider; // 玩家碰撞體
    public GameObject controlledObject; // 當前控制的物體
    public int hp = 0;
    [Header("Possessed Settings")]
    [SerializeField] public GameObject collidedAnimal = null; // 當前碰撞的動物物件
    [SerializeField] public float switchCooldown = 1f; // 切換動物的時限
    [SerializeField] public float switchTimer = 0f; // 切換動物的計時器
    [SerializeField] public float switchBackCooldown = 3f; // 切換回玩家後的冷卻時間
    [SerializeField] public float switchBackTimer = 0f; // 切換回玩家的冷卻計時器
    [SerializeField] public float animalSwitchCooldown = 3f; // 動物之間切換的冷卻時間
    [SerializeField] public float animalSwitchTimer = 0f; // 動物之間切換的冷卻計時器
    [Header("Jump Settings")]
    [SerializeField] private bool rabbitCanJump = true; // 追蹤兔子是否可以跳跃
    [SerializeField] private int rabbitJumpCount = 0;   // 追蹤兔子跳跃次數
    [SerializeField] public float playerJumpforce = 3f;
    [SerializeField] public float rabbitJumpforce = 5f;
    [Header("Knockout Settings")]
    [SerializeField] public GameObject currentKnockbackTarget = null; // 当前击退的目标
    [SerializeField] public float knockbackCooldown = 3f; // 击退冷却时间
    [SerializeField] public float knockbackTimer = 0f; // 击退计时器
    [SerializeField] public float knockbackForce = 5f;
    [Header("Dash Settings")]
    [SerializeField] public float dashDistance = 5f; // 冲刺的距离
    [SerializeField] public float dashDuration = 0.2f; // 冲刺持续时间
    [SerializeField] public bool isDashing; // 是否正在冲刺
    [SerializeField] public float localScaleX;
   
    [Header("Camera Settings")]
    [SerializeField] public Transform cameraTransform; // 鏡頭的引用
    [SerializeField] public Vector3 cameraOffset = new Vector3(0, 0, -10); // 鏡頭與玩家的偏移
    [SerializeField] public float cameraFollowSpeed = 2f; // 鏡頭追蹤的速度
    [SerializeField] public Vector2 cameraBoundsMin; // 鏡頭的最小邊界 (世界坐標)
    [SerializeField]public Vector2 cameraBoundsMax; // 鏡頭的最大邊界 (世界坐標)        
    void Start()
    {
        // 獲取玩家的 Rigidbody2D 和 Collider2D
        playerRb = playerObject.GetComponent<Rigidbody2D>();
        playerCollider = playerObject.GetComponent<Collider2D>();
        controlledObject = playerObject; // 初始控制對象為玩家
    }

    void Update()
    {
        

        // 在 Update 中檢測玩家的操作和狀態
        PlayerInput();
        SwitchBacktoPlayer();
        AnimalSwitch();

        // 更新切換動物的計時器
        if (switchTimer > 0)
        {
            switchTimer -= Time.deltaTime;
        }
        else
        {
            // 計時結束，重置碰撞的動物
            if (collidedAnimal != null)
            {
                collidedAnimal = null;
            }
        }

        // 更新切換回玩家後的冷卻計時器
        if (switchBackTimer > 0)
        {
            switchBackTimer -= Time.deltaTime;
        }

        // 更新動物之間切換的冷卻計時器
        if (animalSwitchTimer > 0)
        {
            animalSwitchTimer -= Time.deltaTime;
        }

        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.deltaTime;

            // 冷却结束，清除击退目标
            if (knockbackTimer <= 0)
            {
                currentKnockbackTarget = null;
                Debug.Log("熊的击退冷却结束，可以再次击退目标");
            }
        }

        // 更新鏡頭位置，追蹤控制的物件
        Vector3 targetPosition = controlledObject.transform.position + cameraOffset;

        // 限制鏡頭在邊界範圍內
        targetPosition.x = Mathf.Clamp(targetPosition.x, cameraBoundsMin.x, cameraBoundsMax.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, cameraBoundsMin.y, cameraBoundsMax.y);

        // 平滑移動攝影機
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, cameraFollowSpeed * Time.deltaTime);
    }

    void PlayerInput()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            localScaleX = 1; // 角色朝右
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            localScaleX = -1; // 角色朝左
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Rigidbody2D rb = controlledObject.GetComponent<Rigidbody2D>();
            switch (controlledObject.name)
            {
                case "Sour":
                    PlayerJump(rb);
                    break;
                case "Rabbit":
                    RabbitJump(rb);
                    break;
                case "Bear":
                    HandleDash(rb); // 处理冲刺
                    break;
                // 可以在這裡添加更多動物的跳跃行為
                default:
                    Debug.Log("未知動物，無法跳跃");
                    break;
            }
        }
    }

    void FixedUpdate()
    {
        if (!isDashing) // 仅在不冲刺时处理正常移动
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            controlledObject.GetComponent<Rigidbody2D>().velocity = new Vector2(horizontalInput * 5f, controlledObject.GetComponent<Rigidbody2D>().velocity.y);
        }
    }

    void OnEnable()
    {
        // 訂閱玩家的碰撞事件
        PlayerCollisionNotifier.OnPlayerCollision += HandlePlayerCollision;
    }

    void OnDisable()
    {
        // 取消訂閱玩家的碰撞事件
        PlayerCollisionNotifier.OnPlayerCollision -= HandlePlayerCollision;
    }

    // 當玩家發生碰撞時，處理碰撞事件
    private void HandlePlayerCollision(GameObject playerOb, GameObject collidedObject)
    {
        // 檢查被碰撞物體的標籤是否是 "Animal"
        if (collidedObject.CompareTag("Animal") && collidedObject != controlledObject)
        {
            Debug.Log("玩家碰到了動物物件：" + collidedObject.name);
            collidedAnimal = collidedObject;
            Debug.Log("當前儲存碰撞物體：" + collidedAnimal);
            switchTimer = switchCooldown; // 重置切換計時器
        }
    }

    // 從玩家附身成動物或動物附身到動物
    private void SwitchToAnimal(GameObject animalObject)
    {
        controlledObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero; // 重置當前控制對象的速度

        if (controlledObject == playerObject) { playerObject.SetActive(false); } // 玩家隱藏，當玩家附身成動物

        controlledObject = animalObject; // 動物附身到動物
        Debug.Log("附身到動物：" + controlledObject.name);

        if (collidedAnimal != null)
        {
            collidedAnimal = null;
        }
        animalSwitchTimer = animalSwitchCooldown; // 重置動物之間的切換冷卻計時器
    }

    // 恢復成玩家狀態
    private void SwitchBacktoPlayer()
    {
        if (controlledObject.CompareTag("Animal") && Input.GetKeyDown(KeyCode.R))
        {
            Vector3 animalPosition = controlledObject.transform.position;
            Vector2 animalVelocity = controlledObject.GetComponent<Rigidbody2D>().velocity;

            playerObject.transform.position = animalPosition; // 更新玩家位置
            playerRb.velocity = animalVelocity; // 更新玩家動量

            playerObject.SetActive(true); // 玩家顯示，當按 R 回復控制玩家
            controlledObject = playerObject; // 切換控制對象回玩家

            switchBackTimer = switchBackCooldown; // 重置切回玩家的冷卻計時器

            if (collidedAnimal != null)
            {
                collidedAnimal = null;
            }

            Debug.Log("控制切換回玩家：" + controlledObject.name);
        }
    }

    // 按T附身到動物
    private void AnimalSwitch()
    {
        if (collidedAnimal != null && Input.GetKeyDown(KeyCode.T) && switchBackTimer <= 0 && animalSwitchTimer <= 0 && switchTimer > 0)
        {
            SwitchToAnimal(collidedAnimal);
            collidedAnimal = null; // 重置碰撞的動物

            animalSwitchTimer = animalSwitchCooldown; // 重置動物之間的切換冷卻計時器
        }
    }
    private void PlayerJump(Rigidbody2D rb)
    {
        if (Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            rb.AddForce(Vector2.up * playerJumpforce, ForceMode2D.Impulse);
            Debug.Log("玩家跳跃");
        }
    }

    private void RabbitJump(Rigidbody2D rb)
    {
        if (controlledObject.name == "Rabbit" && Mathf.Abs(rb.velocity.y) < 0.01f)
        {
            rabbitCanJump = true; // 允許再次跳跃
            rabbitJumpCount = 0; // 重置跳跃次數
            Debug.Log("兔子觸地，跳跃重置");
        }

        // 判斷兔子是否可以跳跃
        if (rabbitCanJump)
        {
            if (rabbitJumpCount < 1) // 第一次跳跃
            {
                rb.AddForce(Vector2.up * rabbitJumpforce, ForceMode2D.Impulse);
                rabbitJumpCount++;
                Debug.Log("兔子第一次跳跃");
            }
            else if (rabbitJumpCount == 1 && Mathf.Abs(rb.velocity.y) > 0.01f) // 第二次跳跃（需要在空中）
            {
                rb.velocity = new Vector2(rb.velocity.x, 0); // 重置垂直速度
                rb.AddForce(Vector2.up * rabbitJumpforce, ForceMode2D.Impulse);
                rabbitJumpCount++;
                Debug.Log("兔子第二次跳跃");
            }
        }
    }

    private void Bearattack(Rigidbody2D rb)
    {
        // 如果已经有目标，且在冷却时间内，不再处理新的目标
        if (currentKnockbackTarget != null && knockbackTimer > 0)
        {
            Debug.Log("熊击退冷却中，无法重复击退");
            return;
        }

        // 获取熊的碰撞器
        Collider2D bearCollider = rb.GetComponent<Collider2D>();

        if (bearCollider == null)
        {
            Debug.LogError("熊没有附加 Collider2D！");
            return;
        }

        // 定义接触过滤器，排除 Ground 标签
        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = false; // 不检测触发器
        filter.useLayerMask = false; // 不启用层级筛选

        // 存储结果的容器
        List<Collider2D> results = new List<Collider2D>();

        // 检测碰触的对象
        int contactCount = bearCollider.OverlapCollider(filter, results);

        for (int i = 0; i < contactCount; i++)
        {
            Collider2D contact = results[i];

            // 排除自身和地面
            if (contact.gameObject != controlledObject && !contact.CompareTag("Ground"))
            {
                // 设置当前击退目标
                currentKnockbackTarget = contact.gameObject;

                // 获取目标的 Rigidbody2D
                Rigidbody2D targetRb = contact.attachedRigidbody;

                if (targetRb != null)
                {
                    // 计算击退方向
                    Vector2 knockbackDirection = (contact.transform.position - rb.transform.position).normalized;

                    // 施加击退力量
                    targetRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);

                    Debug.Log($"熊击退了目标: {contact.name}");
                }

                // 启动冷却计时器
                knockbackTimer = knockbackCooldown;
                break; // 只处理一个目标
            }
        }
    }

    private void HandleDash(Rigidbody2D rb)
    {
        if (isDashing) return;

        isDashing = true;

        // 根据方向键输入设置冲刺方向
        float direction = 0;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            direction = 1; // 向右
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            direction = -1; // 向左
        }

        // 如果没有方向键输入，则不进行冲刺
        if (direction == 0)
        {
            Debug.Log("没有检测到方向键输入，无法冲刺");
            isDashing = false;
            return;
        }

        Vector2 dashDirection = new Vector2(direction, 0);
        rb.velocity = dashDirection * dashDistance;

        Debug.Log($"冲刺方向: {dashDirection}, 速度: {rb.velocity}");

        // 在冲刺持续时间后停止冲刺
        Invoke("EndDash", dashDuration);
    }

    private void EndDash()
    {
        isDashing = false;
        Debug.Log("冲刺结束");
    }






}

