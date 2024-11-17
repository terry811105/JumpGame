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
    private GameObject collidedAnimal = null; // 當前碰撞的動物物體
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
        HandlePlayerInput();
        HandleSwitchBack();
        HandleAnimalSwitch();

        // 更新切換動物的計時器
        if (switchTimer > 0)
        {
            switchTimer -= Time.deltaTime;
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

    void HandlePlayerInput()
    {
        // 檢測跳躍輸入
        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(controlledObject.GetComponent<Rigidbody2D>().velocity.y) < 0.01f)
        {
            controlledObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
            Debug.Log("跳躍：" + controlledObject.name);
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
    private void HandlePlayerCollision(GameObject player, GameObject collidedObject)
    {
        // 檢查被碰撞物體的標籤是否是 "Animal"
        if (collidedObject.CompareTag("Animal") && collidedObject != controlledObject)
        {
            Debug.Log("玩家碰到了動物物體：" + collidedObject.name);
            collidedAnimal = collidedObject;
        }
    }

    // 切換控制對象為動物
    private void SwitchToAnimal(GameObject animalObject)
    {
        controlledObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero; // 重置當前控制對象的速度
        if (controlledObject == playerObject) { playerObject.SetActive(false); } // 玩家隱藏，當玩家切換成動物
        controlledObject = animalObject; // 切換控制對象為動物
        // 動物切換控制權，不隱藏動物
        Debug.Log("控制切換到動物：" + controlledObject.name);
        animalSwitchTimer = animalSwitchCooldown; // 重置動物之間的切換冷卻計時器
    }

    // 切換回玩家
    private void HandleSwitchBack()
    {
        if (controlledObject.CompareTag("Animal") && Input.GetKeyDown(KeyCode.R))
        {
            Vector3 animalPosition = controlledObject.transform.position;
            Vector2 animalVelocity = controlledObject.GetComponent<Rigidbody2D>().velocity;

            playerObject.transform.position = animalPosition; // 更新玩家位置
            playerRb.velocity = animalVelocity; // 更新玩家動量

            playerObject.SetActive(true); // 玩家顯示，當按 R 回復控制玩家
            controlledObject = playerObject; // 切換控制對象回玩家

            switchBackTimer = switchBackCooldown; // 重置切換回玩家的冷卻計時器
            collidedAnimal = null; // 重置碰撞的動物
            Debug.Log("控制切換回玩家：" + controlledObject.name);
        }
    }

    // 處理切換到動物的邏輯
    private void HandleAnimalSwitch()
    {
        if (collidedAnimal != null && Input.GetKeyDown(KeyCode.T) && switchBackTimer <= 0 && animalSwitchTimer <= 0 && switchTimer <= 0)
        {
            SwitchToAnimal(collidedAnimal);
            collidedAnimal = null; // 重置碰撞的動物
            switchTimer = switchCooldown; // 重置切換計時器
            animalSwitchTimer = animalSwitchCooldown; // 重置動物之間的切換冷卻計時器
        }
    }
}
