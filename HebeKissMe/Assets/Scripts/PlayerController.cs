using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerScripts : MonoBehaviour
{
    public GameObject playerObject; // 玩家物件的引用
    private Rigidbody2D playerRb;   // 玩家剛體
    private Collider2D playerCollider; // 玩家碰撞體
    public GameObject controlledObject; // 當前控制的物體
    public int hp = 0;
    public GameObject collidedAnimal = null; // 當前碰撞的動物物體
    public float switchCooldown = 1f; // 切換動物的時限
    public float switchTimer = 0f; // 切換動物的計時器
    public float switchBackCooldown = 3f; // 切換回玩家後的冷卻時間
    public float switchBackTimer = 0f; // 切換回玩家的冷卻計時器
    public float animalSwitchCooldown = 3f; // 動物之間切換的冷卻時間
    public float animalSwitchTimer = 0f; // 動物之間切換的冷卻計時器

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
            collidedAnimal = null;

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
    }

    void PlayerInput()
    {
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
                case "Bird":
                    //BirdJump(rb);
                    break;
                // 可以在這裡添加更多動物的跳躍行為
                default:
                    Debug.Log("未知動物，無法跳躍");
                    break;
            }
        }
    }

    void FixedUpdate()
    {
        // 檢測水平移動輸入
        float horizontalInput = Input.GetAxis("Horizontal");
        controlledObject.GetComponent<Rigidbody2D>().velocity = new Vector2(horizontalInput * 5f, controlledObject.GetComponent<Rigidbody2D>().velocity.y);
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
            Debug.Log("玩家碰到了動物物體：" + collidedObject.name);
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
        
        collidedAnimal = null; // 重置碰撞的動物
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
            collidedAnimal = null; // 重置碰撞的動物
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
            rb.AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
            Debug.Log("玩家跳躍");
        }
    }

    private void RabbitJump(Rigidbody2D rb)
    {
        bool canDoubleJump = Mathf.Abs(rb.velocity.y) > 0.01f;
        if (canDoubleJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
            Debug.Log("兔子二段跳");
        }
        else
        {
            rb.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
            Debug.Log("兔子第一次跳躍");
        }
    }
}

